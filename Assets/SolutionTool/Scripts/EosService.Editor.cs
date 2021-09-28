using Battlehub.RTCommon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Eos.Service
{
    public partial class Workspace 
    {
        public override Transform EditorTrasnform => _workspace.Transform;
        //public override void RTEOnCreated(IRTE editor)
        //{
        //    //base.RTEOnCreated(editor);
        //    _workspace.AddComponent<ExposeToEditor>();
        //}
    }
}