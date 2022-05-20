using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MessagePack;
namespace Eos.Objects
{
    [EosObject]
    public partial class EosCollider : EosObjectBase , ITransform
    {
        private EosTransform _transform;
        private ColliderType _colliderType;
        private eosCollider _collider;
        private eosColliderAdaptor _collideradapter;
        private bool _istrigger;
        private bool _enable = true;
        [Inspector("Basic", "DetectType")]
        [Key(101)]public DetectType DetectType { get; set; } = DetectType.All;
        [IgnoreMember]public EosTransform Transform => _transform;
        [IgnoreMember] public EventHandler<Collision> OnCollisionEnter;
        [IgnoreMember] public EventHandler<Collision> OnCollisionExit;
        [IgnoreMember] public EventHandler<Collider> OnTriggerEnter;
        [IgnoreMember] public EventHandler<Collider> OnTriggerExit;
        public EosCollider()
        {
            _transform = ObjectFactory.CreateInstance<EosTransform>();
        }
        [Key(102)]
        [Inspector("Basic", "ColliderType")]
        public ColliderType ColliderType
        {
            set
            {
                if (_colliderType != value && _collider != null && _collider.Collider!=null)
                {
                    _collider.Destroy();
                    _collider = null;
                }
                _colliderType = value;
                _collider = eosCollider.Create(_colliderType);
            }
            get => _colliderType;
        }
        [Key(103)]
        public bool Enable
        {
            set
            {
                _enable = value;
                if (_collider.Collider != null)
                    _collider.Collider.enabled = value;
            }
            get =>_enable;
        }
        [Key(104)]
        public bool IsTrigger 
        { 
            set => _collider.IsTrigger = _istrigger = value;
            get => _istrigger;
        }
        [Key(105)]
        public eosCollider Collider
        {
            get=> _collider;
            set => _collider = value;
        }
        public override void OnCreate()
        {
            base.OnCreate();
            _transform.Create(Name);
        }
        public override void OnCopyTo(EosObjectBase target)
        {
            if (!(target is EosCollider targetcollider))
                return;
            targetcollider.ColliderType = _colliderType;
            targetcollider.IsTrigger = _istrigger;
            _collider.CopyTo(targetcollider._collider);
            base.OnCopyTo(target);
        }

        protected override void OnActivate(bool active)
        {
            AttachCollider();
        }
        public override void OnAncestryChanged()
        {
            _transform.SetParent((Parent as ITransform).Transform);
            AttachCollider();
        }
        private void AttachCollider()
        {
            if (!ActiveInHierachy || !Ref.IsPlaying)
                return;
            if (_collider!=null && _collider.Collider != null)
                return;
            if (!(_parent is EosTransformActor actor))
                return;
            _collider?.Destroy();
            _collider.Attach(actor);
            _collider.Layer = actor.Layer;
            _collider.IsTrigger = _istrigger;
            _collider.Collider.enabled = _enable;
            _collideradapter = eosColliderAdaptor.RegistCollisionEvent(actor,this);
        }
        public override void OnDestroy()
        {
            _collider?.Destroy();
            if (_collideradapter != null) GameObject.Destroy(_collideradapter);
        }
    }
    public enum ColliderType
    {
        Box,
        Capsule,
        Sphere,
    }
    public enum DetectType
    {
        All,
        Other
    }
    public class eosColliderAdaptor : MonoBehaviour
    {
        private EosCollider _colider;
        private EosTransformActor _actor;
        public EosTransformActor Actor => _actor;
        public static eosColliderAdaptor RegistCollisionEvent(EosTransformActor actor,EosCollider collider)
        {
            var collidertrans = actor  as ITransform;
            var adapter = collidertrans.Transform.Transform.gameObject.GetComponent<eosColliderAdaptor>();
            if (adapter==null)
                adapter = collidertrans.Transform.Transform.gameObject.AddComponent<eosColliderAdaptor>();
            adapter._colider = collider;
            adapter._actor = actor;
            return adapter;
        }
        private void OnTriggerEnter(Collider other)
        {
            var income = other.GetComponent<eosColliderAdaptor>();
            if (income == null || income._colider==null || income._colider.DetectType == DetectType.Other)
                return;
            if (_colider.FindInParent<EosModel>() == income._colider.FindInParent<EosModel>())
                return;
            _colider.OnTriggerEnter?.Invoke(income._actor, other);
        }
        private void OnTriggerExit(Collider other)
        {
            var income = other.GetComponent<eosColliderAdaptor>();
            if (income==null)
                return;
            _colider.OnTriggerExit?.Invoke(income._actor, other);
        }
        private void OnCollisionEnter(Collision collision)
        {
            _colider.OnCollisionEnter?.Invoke(_actor, collision);
        }
        private void OnCollisionExit(Collision collision)
        {
            var income = collision.transform.GetComponent<eosColliderAdaptor>();
            if (income==null)
                return;
            _colider.OnCollisionExit?.Invoke(income._actor, collision);
        }
    }
    [MessagePackObject]
    [Union(101, typeof(eosBoxCollider))]
    [Union(102, typeof(eosCapsuleCollider))]
    [Union(103, typeof(eosSphereCollider))]
    public abstract class eosCollider
    {
        protected int _layer;
        protected Collider _collider;
        protected Vector3 _center = new Vector3(0, 17, 0);
        protected bool _istrigger = false;
        [Key(301)]
        public ColliderType ColiderType;
        [Key(302)]public abstract Vector3 Center { set; get; }
        [IgnoreMember] public Collider Collider => _collider;
        [Key(303)]
        public bool IsTrigger 
        {
            set
            {
                if (_collider == null)
                    return;
                _collider.isTrigger = value;
                _istrigger = value;
            }
            get => _istrigger;
        }

        public abstract void Attach(ITransform actor);
        [Key(304)]
        public int Layer
        {
            set
            {
                _layer = value;
                if (_collider == null)
                    return;
                _collider.gameObject.layer = value;
            }
            get => _layer;
        }
        public virtual void CopyTo(eosCollider targetcollider) 
        {
            targetcollider._center = _center;
            targetcollider._istrigger = _istrigger;
            targetcollider.ColiderType = ColiderType;
        }
        public void Destroy()
        {
            if (Application.isPlaying)
                GameObject.Destroy(_collider);
            else
                GameObject.DestroyImmediate(_collider);
        }
        public static eosCollider Create(ColliderType type)
        {
            switch (type)
            {
                case ColliderType.Box:
                    return new eosBoxCollider { ColiderType = ColliderType.Box };
                case ColliderType.Sphere:
                    return new eosSphereCollider { ColiderType = ColliderType.Sphere};
                case ColliderType.Capsule:
                    return new eosCapsuleCollider { ColiderType = ColliderType.Capsule};
            }
            return null;
        }
    }
    public class eosBoxCollider : eosCollider
    {
        private Vector3 _size = new Vector3(30, 2, 30);
        [Key(401)]
        public Vector3 Size { set { ((BoxCollider)_collider).size = value;_size = value; } }
        public override Vector3 Center { set { ((BoxCollider)_collider).center = value; _center = value; } get => _center; }
        public override void CopyTo(eosCollider targetcollider)
        {
            if (!(targetcollider is eosBoxCollider target))
                return;
            target._size = _size;
            base.CopyTo(targetcollider);
        }
        public override void Attach(ITransform actor)
        {
            var actorobject = actor.Transform.Transform.gameObject;
            var collider = actorobject.AddComponent<BoxCollider>();
            collider.size = _size;
            collider.center = _center;
            _collider = collider;
            _collider.isTrigger = _istrigger;
        }
    }
    public class eosCapsuleCollider : eosCollider
    {
        private float _radius = 6;
        private float _height = 30;
        public override void CopyTo(eosCollider targetcollider)
        {
            if (!(targetcollider is eosCapsuleCollider target))
                return;
            target._radius = _radius;
            target._height = _height;
            base.CopyTo(targetcollider);
        }
        [Key(401)]
        public float Radius
        {
            get => _radius;
            set
            {
                _radius = value;
                if (_collider == null)
                    return;
                ((CapsuleCollider)_collider).radius = value;
            }
        }
        [Key(402)]
        public float Height
        {
            set
            {
                _height = value;
                if (_collider == null)
                    return;
                ((CapsuleCollider)_collider).height = value;
            }
            get => _height;
        }
        [Key(403)]
        public override Vector3 Center
        {
            set
            {
                _center = value;
                if (_collider == null)
                    return;
                ((CapsuleCollider)_collider).center = value;
            }
            get => _center;
        }
        public override void Attach(ITransform actor)
        {
            var actorobject = actor.Transform.Transform.gameObject;
            var collider = actorobject.AddComponent<CapsuleCollider>();
            collider.radius = _radius;
            collider.height = _height;
            collider.center = _center;
            _collider = collider;
            _collider.isTrigger = _istrigger;
        }
    }
    public class eosSphereCollider : eosCollider
    {
        private float _radius = 30;
        [Key(401)]
        public float Radius
        {
            set
            {
                _radius = value;
                ((SphereCollider)_collider).radius = value;
            }
            get => _radius;
        }
        [Key(402)]
        public override Vector3 Center { set { ((SphereCollider)_collider).center = value; _center = value; } get => _center; }
        public override void CopyTo(eosCollider targetcollider)
        {
            if (!(targetcollider is eosSphereCollider target))
                return;
            target._radius = _radius;
            base.CopyTo(targetcollider);
        }
        public override void Attach(ITransform actor)
        {
            var actorobject = actor.Transform.Transform.gameObject;
            var collider = actorobject.AddComponent<SphereCollider>();
            collider.radius = _radius;
            collider.center = _center;
            _collider = collider;
            _collider.isTrigger = _istrigger;
        }
    }

}