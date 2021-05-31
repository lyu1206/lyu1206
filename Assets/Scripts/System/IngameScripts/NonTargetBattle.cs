using Eos.Objects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Eos.Script
{
    [InGameScript]

    public class NonTargetBattle : Eos.Script.IScript
    {
        private EosCollider _attackrange;
        private EosFsm _fsm;
        public EosObjectBase script { get; set; }
        public bool Enable 
        {
            set
            {
            }
            
        }

        public IEnumerator Body()
        {
            var hostilelayer = LayerMask.NameToLayer("Hostile");
            var model = script.Parent.FindInParent<EosModel>();
            var humanoid = model.FindDeepChild<EosHumanoid>();
            humanoid.Speed = 30;

            yield return new WaitCondition(() => humanoid.Humanoidroot != null);
            humanoid.AIStart();
            var attackrange = _attackrange = humanoid.SetupAttackRange(humanoid.Radius,3);
            var fsm = _fsm = humanoid.FSM;
            attackrange.OnTriggerEnter += (sender, collidee) =>
            {
                var adapter = collidee.GetComponent<eosColliderAdaptor>();
                if (adapter == null)
                    return;
                var collidehumanoid = adapter.Actor.Parent.FindChild<EosHumanoid>();
                if (collidehumanoid.Humanoidroot.Layer != hostilelayer)
                    return;
                fsm.SetFsmValue("attack", true);
                fsm.SetFsmValue("trace", false);
                fsm.SetFsmValue("attacktarget", collidehumanoid);
                fsm.FsmTransition("FSMTransit");
                attackrange.Enable = false;
            };
            humanoid.OnAIEvent += AttackFinish;
            humanoid.OnMoveStateChanged += (sender, state) =>
            {
                if (state)
                {
                    attackrange.Enable = true;
                }
            };

            yield return 0;
        }
        private void AttackFinish(object sender, EosHumanoid.AIEventParameter evinfo)
        {
            var eventname = ((EosHumanoid.AIEventParameter)evinfo).name;
            if (eventname == "attackfinish")
            {
                var humanoid = sender as EosHumanoid;
                _fsm.SetFsmValue("attack", true);
                //_attackrange.Enable = true;

                //                humanoid.Ref.Coroutine.OnCoroutineStart(AttackProc(humanoid));
                //Debug.Break();
            }
        }


        public void Finish()
        {
        }

        public void Stop()
        {
        }
    }
}