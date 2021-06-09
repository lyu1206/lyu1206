using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Eos.Objects
{
    using Ore;
    public class EosUIObject : EosTransformActor
    {
        public OreBase _uisource;
        protected override void OnActivate(bool active)
        {
            if (_uisource == null)
                return;
            _uisource.Instantiate(_transform);
        }
        public override void OnCopyTo(EosObjectBase target)
        {
            if (!(target is EosUIObject targetui))
                return;
            targetui._uisource = _uisource;
            base.OnCopyTo(target);
        }
        public override void OnAncestryChanged()
        {
            var lp = LocalPosition;
            var lr = LocalRotation;
            base.OnAncestryChanged();
                        LocalPosition = lp;
                        LocalRotation = lr;
        }
    }
}