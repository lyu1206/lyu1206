using System;
using UnityEngine;

namespace Eos.Objects
{
    using MessagePack;
    public class EosTransformActor : EosObjectBase , ITransform
    {
        protected Transform _transform;
        private EventHandler<float> _components;
        public virtual Transform Transform => _transform;
        public int Layer;
        public int LayerMask => 1 << Layer;
        public override string Name 
        { 
            get => base.Name; 
            set
            {
                base.Name = value;
                _transform.name = value;
            } 
        }
        public Vector3 LocalPosition
        {
            get
            {
                return _transform.localPosition;
            }
            set
            {
                _transform.localPosition = value;
            }
        }
        public Vector3 LocalRotation
        {
            get
            {
                return _transform.localRotation.eulerAngles;
            }
            set
            {
                _transform.localRotation = Quaternion.Euler(value);
            }
        }
        public EosTransformActor()
        {
            var trans = new GameObject(Name);
            _transform = trans.transform;
        }
        public override void OnCopyTo(EosObjectBase target)
        {
            if (!(target is EosTransformActor targetactor))
                return;
            targetactor.LocalPosition = LocalPosition;
            targetactor.LocalRotation = LocalRotation;
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
            var lp = LocalPosition;
            var lr = LocalRotation;
            _transform.SetParent(transactor.Transform);
//            LocalPosition = lp;
//            LocalRotation = lr;
        }
        public override void OnDestroy()
        {
            if (_transform != null)
                GameObject.Destroy(_transform.gameObject);
        }
        public override void Update(float delta)
        {
            _components?.Invoke(this,delta);
        }
    }
}