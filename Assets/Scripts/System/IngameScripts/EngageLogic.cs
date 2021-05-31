using Eos.Objects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Eos.Script
{
    [InGameScript]
    public class EngageLogic : Eos.Script.IScript
    {
        private EosHumanoid _owner;
        public EosObjectBase script { get; set; }
        public bool Enable 
        {
            set
            {
            }
        }

        public IEnumerator Body()
        {
            var humanoid = _owner = script.Parent.FindChild<EosHumanoid>();
            if (humanoid == null)
                yield break;
            yield return new WaitCondition(() => humanoid.Humanoidroot != null);
            humanoid.OnAIEvent += AIEvent;
        }
        private void AIEvent(object sender,EosHumanoid.AIEventParameter parameter)
        {
            var msg = parameter.name;
            if (!(sender is EosHumanoid sendhumanoid))
                return;
            if (msg=="AniEvent")
            {
                if ((string)parameter.args[0] == "hit")
                {
                    _owner.FSM.SetFsmValue("damage", true);
                    _owner.FSM.FsmTransition("FSMTransit");
                }
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