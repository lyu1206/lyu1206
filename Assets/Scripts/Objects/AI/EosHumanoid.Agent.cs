using System;
using System.Collections.Generic;
using Bolt;
using UnityEngine;
using UnityEngine.AI;

namespace Eos.Objects
{
    public enum PropertyIndex
    {
        MoveDirection,
    }
    public partial class EosHumanoid
    {
        private static event EventHandler<Vector3> _movedirectionchanged;
        private static event EventHandler _jumpevent;

        private List<IMoveAgentBehavior> _activeBehaviors = new List<IMoveAgentBehavior>();
        private Dictionary<int, IMoveAgentBehavior> _behaviors;
        private Dictionary<int,string> _behaviorAnimations = new Dictionary<int, string>();

        private Vector3 _movedirection;

        private void InitMoveAgentBehaviros()
        {
            if (_behaviors != null)
                return;
            _behaviors = _moveagent.InitBehaviros();
            foreach (var it in _behaviors)
                it.Value.OnAwake(this);
            RegistComponent(BehaviorUpdate);
            
            SetBehaviorAnimation(MoveAgentState.Idle,"Stand");
            SetBehaviorAnimation(MoveAgentState.Move,"Run");
            SetBehaviorAnimation(MoveAgentState.Jump,"Run");
            SetBehaviorAnimation(MoveAgentState.Fall,"Run");
            SetBehaviorAnimation(MoveAgentState.Die,"Die");
        }

        private void SetBehaviorAnimation(MoveAgentState state, string animation)
        {
            _behaviorAnimations[(int) state] = animation;
        }

        private void PlayBehaviorAnimation(MoveAgentState state)
        {
            _humanoidroot?.PlayNode(_behaviorAnimations[(int)state]);
        }
        private void ActiveBehavior(MoveAgentState state)
        {
            if (_behaviors == null)
                return;
            if (!_behaviors.ContainsKey((int)state))
                return;
            ActiveBehavior(_behaviors[(int)state]);
        }
        private void BehaviorUpdate(object sender, float delta)
        {
            var activebehaviros = new List<IMoveAgentBehavior>(_activeBehaviors);
            activebehaviros.ForEach(it => it.OnUpdate(this, delta));
        }

        private void StartBehavior(IMoveAgentBehavior behavior)
        {
            behavior.OnStart(this);
            PlayBehaviorAnimation(behavior.State);
        }
        private void ActiveBehavior(IMoveAgentBehavior behavior)
        {
            if (_activeBehaviors.Contains(behavior))
                return;
            StartBehavior(behavior);
            if (_activeBehaviors.Count==0)
                _activeBehaviors.Add(behavior);
            else
            {
                bool added = false;
                for (int i=0;i<_activeBehaviors.Count;i++)
                {
                    var it = _activeBehaviors[i];
                    if (it.Priority > behavior.Priority)
                    {
                        _activeBehaviors.Insert(i,behavior);
                        added = true;
                        break;
                    }
                }
                if (!added)
                    _activeBehaviors.Add(behavior);
            }
        }
        private void InActiveBehavior(IMoveAgentBehavior behavior)
        {
            behavior.OnEnd(this);
            var currentbehavior = _activeBehaviors[_activeBehaviors.Count - 1];
            _activeBehaviors.Remove(behavior);
            if (_activeBehaviors.Count == 0)
                ActiveBehavior(MoveAgentState.Idle);
            else
            {
                int highpriority = -1;
                IMoveAgentBehavior nextbehavior = null;
                foreach(var it in _activeBehaviors)
                {
                    if (highpriority < it.Priority)
                    {
                        nextbehavior = it;
                        highpriority = it.Priority;
                    }
                }
                if (nextbehavior != null && currentbehavior!=nextbehavior)
                    StartBehavior(nextbehavior);
            }
        }
        private void RegistMoveDirectionChangedEvent(EventHandler<Vector3> handle)
        {
            _movedirectionchanged += handle;
        }
        public enum MoveAgentState
        {
            Idle,
            Move,
            Jump,
            Fall,
            Die,
        }
        public interface IMoveAgent
        {
            float Angularspeed { set; }
            float Speed { set; }
            float Accelation { set; }
            void MoveDirection(EosHumanoid entity, Vector3 direction);
            Dictionary<int, IMoveAgentBehavior> InitBehaviros();

        }
        public interface IMoveAgentBehavior
        {
            int Priority { get; }
            MoveAgentState State { get; }
            void OnAwake(EosHumanoid entity);
            void OnStart(EosHumanoid entity);
            void OnEnd(EosHumanoid entity);
            void OnUpdate(EosHumanoid entity, float delta);
        }
        public class NavigationAgent : IMoveAgent
        {
            private NavMeshAgent _navagent;
            private static Dictionary<int, IMoveAgentBehavior> _behaviors;
            public float Angularspeed{set => _navagent.angularSpeed = value;}
            public float Speed { set => _navagent.speed = value; }
            public float Accelation { set => _navagent.acceleration = value; }
            public void MoveDirection(EosHumanoid entity, Vector3 direction)
            {
            }
            public Dictionary<int, IMoveAgentBehavior> InitBehaviros()
            {
                return null;
            }
        }
        public class PhysicsAgent : IMoveAgent
        {
            private static Dictionary<int, IMoveAgentBehavior> _behaviors;
            private float _angularSpeed = 1000;
            private float _speed = 0.5f;
            private float _acceleration = 35;
            public float Angularspeed {set => _angularSpeed = value; }
            public float Speed { set => _speed = value; }
            public float Accelation { set => _acceleration = value; }
            public Dictionary<int, IMoveAgentBehavior> InitBehaviros()
            {
                if (_behaviors != null)
                    return _behaviors;
                var behaviros = new Dictionary<int, IMoveAgentBehavior>();
                behaviros[(int)MoveAgentState.Idle] = new Idle();
                behaviros[(int)MoveAgentState.Move] = new Move();
                behaviros[(int)MoveAgentState.Jump] = new Jump();
                behaviros[(int)MoveAgentState.Fall] = new Fall();
                behaviros[(int)MoveAgentState.Die] = new Die();
                _behaviors = behaviros;
                return behaviros;
            }
            public void MoveDirection(EosHumanoid entity, Vector3 direction)
            {
                var root = entity._humanoidroot;
                var newdirection = direction.normalized;
                if (entity._movedirection == newdirection)
                    return;
                entity._movedirection = newdirection;
                EosHumanoid._movedirectionchanged.Invoke(entity, newdirection);
            }
            #region States
            public class Idle : IMoveAgentBehavior
            {
                public int Priority => 0;

                public MoveAgentState State => MoveAgentState.Idle;

                public void OnAwake(EosHumanoid entity) { }
                public void OnEnd(EosHumanoid entity) { }

                public void OnStart(EosHumanoid entity) 
                {
                    var velocity = entity._humanoidroot.Rigidbody.velocity;
                    velocity.x = velocity.z = 0;
                    entity._humanoidroot.Rigidbody.velocity = velocity;
                }

                public void OnUpdate(EosHumanoid entity, float delta) { }
            }
            public class Move : IMoveAgentBehavior
            {
                public int Priority => 1;

                public MoveAgentState State => MoveAgentState.Move;

                public void OnAwake(EosHumanoid entity)
                {
                    _movedirectionchanged += MoveDirectionChanged;
                }
                private void MoveDirectionChanged(object sender,Vector3 direction)
                {
                    if (!(sender is EosHumanoid humanoid))
                        return;
                    if (direction == Vector3.zero)
                        humanoid.InActiveBehavior(this);
                    else
                        humanoid.ActiveBehavior(this);
                }

                public void OnEnd(EosHumanoid entity)
                {
                    var velocity = entity._humanoidroot.Rigidbody.velocity;
                    velocity.x = velocity.z = 0;
                    entity._humanoidroot.Rigidbody.velocity = velocity;
                }

                public void OnStart(EosHumanoid entity)
                {
                }

                public void OnUpdate(EosHumanoid entity, float delta)
                {
                    var moveagent = entity._moveagent as PhysicsAgent;
                    var velocity = entity._movedirection * moveagent._speed;
                    velocity.y =  entity._humanoidroot.Rigidbody.velocity.y;

                    entity._humanoidroot.Rigidbody.velocity = velocity;
                    var autorotation = true;
                    if (autorotation && entity._movedirection != Vector3.zero)
                    {
                        var toRotation = Quaternion.LookRotation(entity._movedirection, Vector3.up);
                        entity._humanoidroot._transform.Transform.rotation = Quaternion.RotateTowards(entity._humanoidroot.Transform.Transform.rotation ,toRotation,1024*delta);
                    }
                }
            }
            public class Jump : IMoveAgentBehavior
            {
                public int Priority => 2;

                public MoveAgentState State => MoveAgentState.Jump;

                public void OnAwake(EosHumanoid entity)
                {
                    EosHumanoid._jumpevent += event_Jump;
                }

                private void event_Jump(object sender,EventArgs args)
                {
                    if (!(sender is EosHumanoid humanoid))
                        return;
                    humanoid.ActiveBehavior(this);
                }

                public void OnEnd(EosHumanoid entity)
                {
                    var root = entity._humanoidroot;
                    root.Collders.ForEach(it => it.OnCollisionEnter -= TestLanding);
                }

                public void OnStart(EosHumanoid entity)
                {
                    var root = entity._humanoidroot;
                    var rigidbody = root.Rigidbody;
                    var velocity = rigidbody.velocity;
                    velocity.y = 4;
                    rigidbody.velocity = velocity;
                    root.Collders.ForEach(it => it.OnCollisionEnter += TestLanding);
                }

                private void TestLanding(object sender, Collision collision)
                {
                    if (!(sender is EosObjectBase onwer))
                        return;
                    var humanoid = onwer.Parent.FindChild<EosHumanoid>();
                    if (humanoid==null)
                        return;
                    humanoid.InActiveBehavior(this);                    
//                    Debug.Log("///////////// Landing ..");
                }
                public void OnUpdate(EosHumanoid entity, float delta) { }
            }
            public class Fall : IMoveAgentBehavior
            {
                public int Priority => 2;

                public MoveAgentState State => MoveAgentState.Fall;

                public void OnAwake(EosHumanoid entity) { }
                public void OnEnd(EosHumanoid entity) { }

                public void OnStart(EosHumanoid entity) { }

                public void OnUpdate(EosHumanoid entity, float delta) { }
            }
            public class Die : IMoveAgentBehavior
            {
                public int Priority => 0;

                public MoveAgentState State => MoveAgentState.Die;

                public void OnAwake(EosHumanoid entity) { }
                public void OnEnd(EosHumanoid entity) { }

                public void OnStart(EosHumanoid entity) { }

                public void OnUpdate(EosHumanoid entity, float delta) { }
            }
            #endregion
        }
    }
}