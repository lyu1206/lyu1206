#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MessagePack;
using Battlehub.RTCommon;

namespace Eos.Objects
{
    using Eos.Objects.Editor;
    using Ore;
    using Service;
    [Serializable]
    public partial class EosTransformActor
    {
        public EosTransformActor(bool editor)
        {

        }
        public override void PropertyChanged(EosObjectBase parent)
        {
        }
        public override void CreatedEditor(EosEditorObject obj)
        {
            //obj.transform.localPosition = Transform.LocalPosition;
            //obj.transform.localRotation = Quaternion.Euler(Transform.LocalRotation);
            //obj.transform.localScale = Transform.LocalScale;
        }
        public override EosTransform EditorTrasnform => _transform;
        //public override EosObjectBase CreateCloneObjectForEditor(ExposeToEosEditor editorobject)
        //{
        //    var copy =  base.CreateCloneObjectForEditor(editorobject) as EosTransformActor;
        //    copy._transform.Transform = editorobject.transform;
        //    editorobject.Owner = copy;
        //    return copy;
        //}
        public override void SetExposeToEditor(ExposeToEosEditor editorobject)
        {
            _rteditObject = editorobject;
            _transform.Transform = editorobject.transform;
        }
        public override void RTEOnCreated(IRTE editor)
        {
            base.RTEOnCreated(editor);
            _rteditObject.transform.localPosition = Transform.LocalPosition;
            _rteditObject.transform.localScale = Transform.LocalScale;
            _rteditObject.transform.localRotation = Quaternion.Euler(Transform.LocalRotation);
            /*
            _transform.AddComponent<ExposeToEditor>();
            if (Parent!=null)
            {
                if (Parent is ITransform trans)
                {
                    _transform.SetParent(trans.Transform);
                }
                else
                {
                    _transform.Transform.SetParent(Parent.EditorTrasnform);
                }
            }
            _transform.Transform.localPosition = Transform.LocalPosition;
            _transform.Transform.localScale = Transform.LocalScale;
            _transform.Transform.localRotation = Quaternion.Euler(Transform.LocalRotation);
            editor.RegisterCreatedObjects(new[] { _transform.Transform.gameObject }, true);
            */
        }
    }
}
namespace Eos.Objects.Editor
{
    public partial class EosTransformActorEditor : EosEditorObject
    {
        [SerializeField]
        public EosTransformActor _transformactor;
        public override EosObjectBase Owner => _transformactor;
    }
}
#endif