using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Eos.Objects
{
    using Bolt;
    using Ore;
    public class EosFsm : EosObjectBase
    {
        public OreBase FSMore;
        private VariableDeclarations _values;
        private GameObject _boltobject;
        public override void OnCopyTo(EosObjectBase target)
        {
            if (!(target is EosFsm targetfsm))
                return;
            targetfsm.FSMore = FSMore;
            base.OnCopyTo(target);
        }
        protected override void OnActivate(bool active)
        {
            var fsm = FSMore.Instantiate();
            var boltlink = fsm.GetComponent<BoltLinkOre>();
            _values = ObjectVariables.Declarations(boltlink.gameObject, false, false);
            _boltobject = boltlink.gameObject;
            boltlink.SetObject(_parent);
#if UNITY_EDITOR
            if (_parent is ITransform trans)
                fsm.transform.parent = trans.Transform;
#endif
        }
        public void SetFsmValue(string name,object value)
        {
            _values[name] = value;
        }
        public void FsmTransition(string name,params object[]args)
        {
            CustomEvent.Trigger(_boltobject, name, args);
        }
    }
}