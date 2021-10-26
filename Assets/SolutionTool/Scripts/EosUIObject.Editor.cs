using Battlehub.RTCommon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Eos.Objects
{
    public partial class EosUIObject
    {
        public override void RTEOnCreated(IRTE editor)
        {
            base.RTEOnCreated(editor);
            OnActivate(true);
        }
    }
}