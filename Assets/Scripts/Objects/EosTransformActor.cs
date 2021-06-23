using System;
using UnityEngine;

namespace Eos.Objects
{
    using MessagePack;
    [MessagePackObject]
    public class EosTransform
    {
        private GameObject _unityobject;
        protected Transform _transform;
        [IgnoreMember] public Transform Transform { get => _transform; set => _transform = value; }
        public string Name
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
            
            _unityobject = _unityobject??ObjectFactory.CreateUnityInstance(name).gameObject;
            _transform = _unityobject.transform;
            _transform.localPosition = Vector3.zero;
            _transform.localRotation = Quaternion.identity;
            _transform.localScale = Vector3.one;
            return _transform;
        }
        [Key(1)]
        public Vector3 LocalPosition
        {
            set
            {
                _transform.localPosition = value;
            }
            get => _transform.localPosition;
        }
        [Key(2)]
        public Vector3 LocalRotation
        {
            set
            {
                _transform.localRotation = Quaternion.Euler(value);
            }
            get => _transform.localRotation.eulerAngles;
        }
        [Key(3)]
        public Vector3 LocalScale
        {
            set
            {
                _transform.localScale = value;
            }
            get => _transform.localScale;
        }
        public void SetParent(EosTransform parent)
        {
            var lp = LocalPosition;
            var lr = LocalRotation;
            Transform.SetParent(parent.Transform);
            Transform.localPosition = lp;
            Transform.localRotation = Quaternion.Euler(lr);
        }
        public void Destroy()
        {
            GameObject.Destroy(_transform.gameObject);
        }
        public T AddComponent<T>() where T : Component
        {
            var comp = _unityobject.AddComponent<T>();
            _transform = _unityobject.transform;
            return comp;
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
        protected EosTransform _transform;
        private EventHandler<float> _components;
        [IgnoreMember] public virtual EosTransform Transform => _transform;
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
        [Key(22)]
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
        [Key(23)]
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
        public EosTransformActor()
        {
            _transform = ObjectFactory.CreateInstance<EosTransform>();
            _transform.Create(Name);
        }
        public override void OnCopyTo(EosObjectBase target)
        {
            if (!(target is EosTransformActor targetactor))
                return;
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