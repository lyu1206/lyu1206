using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
namespace Eos.Objects
{
    using Service.AI;
    using MessagePack;

    [MessagePackObject]
    public partial class EosHumanoid : EosTransformActor// EosObjectBase ,ITransform
    {
        public int Level = 1;

        private float _angularspeed = 1000;
        private float _speed = 35;
        private float _accelation = 1000;
        [Key(1)]
        public float Angularspeed 
        {
            get=>_angularspeed;
            set
            {
                _angularspeed = value;
                if (_navagent!=null)
                    _navagent.angularSpeed = value;
            }
        }
        [Key(2)]
        public float Speed
        {
            get =>_speed;
            set
            {
                _speed = value;
                if (_navagent!=null)
                    _navagent.speed = value;
            }
        }
        [Key(3)]
        public float Accelation
        {
            get=>_accelation;
            set
            {
                _accelation = value;
                if (_navagent!=null)
                    _navagent.acceleration = value;
            }
        }
        public float StopDistance { set => _navagent.stoppingDistance = value; }
        public Vector3 MoveDirection
        {
            set
            {
                if (_navagent != null)
                {
                    UpdateHumanoidPosition();
                    _transform.forward = value;
                    OnMoveStateChanged?.Invoke(this,true);
                    _navagent.isStopped = false;
                    _navagent.SetDestination(_navagent.transform.localPosition + value*_radius*2);
                }
            }
        }
        public void UpdateHumanoidPosition()
        {
            _transform.localPosition = _humanoidroot.LocalPosition;
            _transform.forward = _humanoidroot.Transform.forward;
        }
        public bool IsStop => (!_navagent.pathPending && _navagent.remainingDistance == 0);
        public const string humanoidroot = "HumanoidRoot";
        private float _radius;
        private NavMeshAgent _navagent;
        private EosPawnActor _humanoidroot;
        private AccessPlate _accesplate;
        public NavMeshAgent NavAgent => _navagent;
        public EosPawnActor Humanoidroot => _humanoidroot;
        public AccessPlate AccesPlate => _accesplate;
        public float Radius => _radius;
        public override void OnCopyTo(EosObjectBase target)
        {
            if (!(target is EosHumanoid targethumanoid))
                return;
            targethumanoid._radius = _radius;
            targethumanoid._angularspeed = _angularspeed;
            targethumanoid._speed = _speed;
            targethumanoid._accelation = _accelation;
            base.OnCopyTo(target);
        }

        protected override void OnActivate(bool active)
        {
            base.OnActivate(active);
            var root = _parent.FindChild<EosPawnActor>(humanoidroot);
            if (root==null)
                return;
            var rootobject = root.Transform.gameObject;
            _humanoidroot = root;

            _radius = (_humanoidroot.FindChild<EosCollider>().Collider as eosCapsuleCollider).Radius;

            _navagent = rootobject.AddComponent<NavMeshAgent>();
//            _navagent.radius = _radius;


            Angularspeed = _angularspeed;
            Speed = _speed;
            Accelation = _accelation;
        }
        public void SetPosition(Vector3 to)
        {
            UpdateHumanoidPosition();
            _navagent.enabled = false;
            _humanoidroot.Transform.position = to;
            _navagent.enabled = true;
        }
        public void MoveTo(Vector3 dest)
        {
            UpdateHumanoidPosition();
            OnMoveStateChanged?.Invoke(this, true);
            _navagent.isStopped = false;
            _navagent.SetDestination(dest);
        }
        public void Stop()
        {
            UpdateHumanoidPosition();
            OnMoveStateChanged?.Invoke(this, false);
            _navagent.ResetPath();
            _navagent.isStopped = true;
        }
        public override void OnChildAdded(EosObjectBase child)
        {
            if (child is AccessPlate plate)
                _accesplate = plate;
            if (child is EosCollider collider)
            {
                var rigidbody = Transform.gameObject.AddComponent<Rigidbody>();
                rigidbody.isKinematic = true;
            }
        }
        public override void OnChildRemoved(EosObjectBase child)
        {
            if (child is AccessPlate plate)
                _accesplate = null;
        }

        public override string Name 
        { 
            get => base.Name; 
            set
            {
                base.Name = value;
                _transform.name = value;
            }
        }
        public override void OnAncestryChanged()
        {
            base.OnAncestryChanged();
            if (!(_parent is ITransform transactor))
                return;
            _transform.parent = transactor.Transform;
            _transform.localPosition = Vector3.zero;
            _transform.localRotation = Quaternion.identity;
        }
        public override string ToString()
        {
            return $"{_parent.Name}'s humanoid";
        }
    }
}
