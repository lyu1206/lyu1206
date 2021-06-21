using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MessagePack;

namespace Eos.Objects
{
    using Ore;
    [System.Serializable]
    public class EosLight : EosTransformActor
    {
        [IgnoreMember]public OreBase Light;
        [IgnoreMember]public LightController _lightcontroller;
        protected override void OnActivate(bool active)
        {
            var light = Light.Instantiate();
            _lightcontroller = light.GetComponent<LightController>();
        }
        public override void OnCopyTo(EosObjectBase target)
        {
            if (!(target is EosLight targetlight))
                return;
            targetlight.Light = Light;
            base.OnCopyTo(target);
        }
    }
}