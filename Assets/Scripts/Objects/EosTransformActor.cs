using System;
using UnityEngine;

namespace Eos.Objects
{
    using MessagePack;
    [MessagePackObject]
    public class EosTransform
    {
        [Key(1)]public Vector3 _localposition;
        [Key(2)]public Vector3 _localrotation;
        [Key(3)]public Vector3 _localscale = Vector3.one;

        private GameObject _unityobject;
        protected Transform _transform;
        [IgnoreMember] public bool HasChild => _transform != null && _transform.childCount > 0;
        public Transform GetChild(int index)
        {
            if (_transform == null)
                return null;
            if (index >= _transform.childCount)
                return null;
            return _transform.GetChild(index);
        }
        [IgnoreMember] public Transform Transform
        { 
            get => _transform;
            set
            {
                _transform = value;
                _transform.localPosition = _localposition;
                _transform.localRotation = Quaternion.Euler(_localrotation);
                _transform.localScale = _localscale;
                _unityobject = _transform.gameObject;
            }
        }
        [IgnoreMember]public string Name
        {
            set
            {
                if (_transform == null)
                    return;
                _transform.name = value;
            }
        }
        public Transform Create(string name)
        {
            if (_transform != null)
                return _transform;
            _unityobject = _unityobject??ObjectFactory.CreateUnityInstance(name).gameObject;
            _transform = _unityobject.transform;
            _transform.localPosition = _localposition;
            _transform.localRotation = Quaternion.Euler(_localrotation);
            _transform.localScale = _localscale;
            return _transform;
        }

        [IgnoreMember]
        public Vector3 WorldPosition
        {
            get
            {
                if (_transform != null)
                    return _transform.position;
                return Vector3.zero;
            }
            set
            {
                if (_transform)
                    _transform.position = value;
            }
        }
        [IgnoreMember]
        public Vector3 LocalPosition
        {
            set
            {
                _localposition = value;
                if (_transform!=null)
                    _transform.localPosition = value;
            }
            get
            {
                if (_transform!=null)
                    return _transform.localPosition;
                return _localposition;
            }
        }
        [IgnoreMember]
        public Vector3 LocalRotation
        {
            set
            {
                _localrotation = value;
                if (_transform!=null)
                    _transform.localRotation = Quaternion.Euler(value);
            }
            get
            {
                if (_transform!=null)
                    return _transform.localRotation.eulerAngles;
                return _localrotation;
            }
        }
        [IgnoreMember]
        public Vector3 LocalScale
        {
            set
            {
                _localscale = value;
                if (_transform!=null)
                    _transform.localScale = value;
            }
            get
            {
                if (_transform!=null)
                    return _transform.localScale;
                return _localscale;
            }
        }
        public void SetParent(EosTransform parent,bool stayworld = true)
        {
            Transform?.SetParent(parent.Transform, stayworld);
            Transform.localPosition = _localposition;
            Transform.localRotation = Quaternion.Euler(_localrotation);
            Transform.localScale = _localscale;
        }
        public void Destroy()
        {
            if (_unityobject == null)
                return;
            GameObject.Destroy(_unityobject);
        }
        public T AddComponent<T>() where T : Component
        {
            T comp = _unityobject.AddComponent<T>();
            _transform = _unityobject.transform;
            return comp;
        }
        public T GetComponent<T>() where T : Component
        {
            if (_unityobject == null)
                return null;
            var component = _unityobject.GetComponent<T>();
            return component;
        }
        public void CopyTo(EosTransform target)
        {
            target.Create(_transform.name);
            target.LocalPosition = LocalPosition;
            target.LocalRotation = LocalRotation;
            target.LocalScale = LocalScale;
        }

    }
    public partial class EosTransformActor : EosObjectBase , ITransform
    {
        [Key(20)]public EosTransform _transform;
        private EventHandler<float> _components;
        [IgnoreMember]public virtual EosTransform Transform => _transform;
        [Key(21)]public int Layer;
        [IgnoreMember]public int LayerMask => 1 << Layer;
        [IgnoreMember]
        public override string Name 
        { 
            get => base.Name; 
            set
            {
                base.Name = value;
                if (_transform == null)
                    return;
                _transform.Name = value;
            } 
        }

        [IgnoreMember]
        public virtual Vector3 WorldPosition
        {
            get { return _transform.WorldPosition; }
            set { _transform.WorldPosition = value; }
                
        }
        [IgnoreMember]
        [Inspector("Basic", "LocalPosition")]
        public virtual Vector3 LocalPosition
        {
            get
            {
                return _transform.LocalPosition;
            }
            set
            {
                _transform.LocalPosition = value;
            }
        }
        [IgnoreMember]
        [Inspector("Basic", "LocalRotation")]
        public virtual Vector3 LocalRotation
        {
            get
            {
                return _transform.LocalRotation;
            }
            set
            {
                _transform.LocalRotation = value;
            }
        }
        [IgnoreMember]
        [Inspector("Basic", "LocalScale")]
        public virtual Vector3 LocalScale
        {
            get
            {
                return _transform.LocalScale;
            }
            set
            {
                _transform.LocalScale = value;
            }
        }

        public void SetLocalScale(float x, float y, float z)
        {
            LocalScale = new Vector3(x,y,z);
        }
        public void SetLocalPosition(float x, float y, float z)
        {
            LocalPosition = new Vector3(x,y,z);
        }
        public void SetWorldPosition(float x, float y, float z)
        {
            WorldPosition = new Vector3(x,y,z);
        }
        public EosTransformActor()
        {
            _transform = ObjectFactory.CreateInstance<EosTransform>();
            //            _transform.Create(Name);
        }
        public override void OnCreate()
        {
            base.OnCreate();
            _transform.Create(Name);
        }
        public override void OnCopyTo(EosObjectBase target)
        {
            if (!(target is EosTransformActor targetactor))
                return;
//            targetactor._transform = ObjectFactory.CreateInstance<EosTransform>();
            Transform.CopyTo(targetactor.Transform);
            targetactor.Layer = Layer;
            base.OnCopyTo(target);
        }
        //protected override void OnActivate()
        //{
        //    base.OnActivate();
        //}
        public void RegistComponent(EventHandler<float> component)
        {
            _components -= component;
            _components += component;
            if (_components.GetInvocationList().Length == 1)
                Ref.ObjectManager.RegistUpdateObject(this);
        }
        public void UnRegistComponent(EventHandler<float> component)
        {
            _components -= component;
            if (component.GetInvocationList().Length == 0)
                Ref.ObjectManager.UnRegistUpdateObject(this);
        }
        public override void OnAncestryChanged()
        {
            base.OnAncestryChanged();
            if (!(_parent is ITransform transactor))
                return;
            _transform.SetParent(transactor.Transform);
        }
        public override void OnDestroy()
        {
            _transform?.Destroy();
        }
        public override void Update(float delta)
        {
            _components?.Invoke(this,delta);
        }
    }
}