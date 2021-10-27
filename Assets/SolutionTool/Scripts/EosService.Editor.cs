using Battlehub.RTCommon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Eos.Service
{

    using Objects;
    public partial class EosService
    {
        public override EosTransform EditorTrasnform => _transform;
        public override void SetExposeToEditor(ExposeToEosEditor editorobject)
        {
            _transform.Transform = editorobject.transform;
        }
    }
    public partial class Workspace 
    {
        //public override void RTEOnCreated(IRTE editor)
        //{
        //    //base.RTEOnCreated(editor);
        //    _workspace.AddComponent<ExposeToEditor>();
        //}
    }
    public partial class TypeService
    {
        public override void RTEOnCreated(IRTE editor)
        {
            base.RTEOnCreated(editor);
            _rteditObject.gameObject.SetActive(false);
        }
        public override EosObjectBase CreateCloneObjectForEditor(ExposeToEosEditor editorobject)
        {
            editorobject.gameObject.SetActive(false);
            return base.CreateCloneObjectForEditor(editorobject);
        }
    }
}