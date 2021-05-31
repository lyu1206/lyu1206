using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Eos.Objects
{
    using Ore;
    public class EosLight : EosTransformActor
    {
        public OreBase Light;
        public LightController _lightcontroller;
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