using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Eos.Objects
{
    public partial class EosHumanoid
    {
        public struct AIEventParameter
        {
            public string name;
            public object[] args;
        }
        private EosFsm _fsm;
        private EosCollider _attackrange;
        private EosCollider _tracerange;
        public EosFsm FSM => _fsm = _fsm ?? FindChild<EosFsm>();
        public EosCollider AttackRange => _attackrange;
        public EosCollider TraceRange => _tracerange;
        public EosHumanoid TraceTarget { get; set; }
        public EventHandler<AIEventParameter> OnAIEvent { get; set; }
        public EventHandler<bool> OnMoveStateChanged { get; set; }
        public void AIStart()
        {
            _humanoidroot.OnAnimationEvent += (sender, name) =>
            {
                _fsm.FsmTransition("AniEvent", name);
            };
        }
        public EosCollider SetupAttackRange(float radius,float forward)
        {
            if (_attackrange == null)
            {
                var attackrange = ObjectFactory.CreateInstance<EosCollider>();
                attackrange.ColliderType = ColliderType.Sphere;
                attackrange.DetectType = DetectType.Other;
                attackrange.IsTrigger = true;
                _attackrange = attackrange;
                _attackrange.Enable = true;
                //Humanoidroot.AddChild(attackrange);
                AddChild(attackrange);
            }
            var collider = _attackrange.Collider as eosSphereCollider;
            collider.Radius = radius;
            collider.Collider.transform.localPosition = new Vector3(0, 0, forward);
            collider.Center = new Vector3(0, 0, 0);
            _fsm = _fsm ?? FindChild<EosFsm>();
            return _attackrange;
        }
        public void SetupTraceRange(float radius)
        {
            var aiservice = Ref.Solution.AIService;
            var notifyrange = ObjectFactory.CreateInstance<EosCollider>();
            notifyrange.ColliderType = ColliderType.Sphere;
            notifyrange.DetectType = DetectType.Other;
            notifyrange.IsTrigger = true;
            _humanoidroot.AddChild(notifyrange);
            var collider = notifyrange.Collider as eosSphereCollider;
            collider.Radius = radius;
            collider.Center = Vector3.zero;
            notifyrange.OnTriggerEnter += (income, other) =>
            {
                if (!(income is EosTransformActor target))
                    return;
                var targethumanoid = target.Parent.FindChild<EosHumanoid>();
                if (!aiservice.RegistEngage(this, targethumanoid))
                    return;
                var myaiobj = aiservice.GetAIEngageObject(this.ObjectID);
                notifyrange.Enable = false;
                myaiobj.Engage();
            };
            _tracerange = notifyrange;
        }
    }
}