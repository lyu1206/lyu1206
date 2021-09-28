#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MessagePack;

namespace Eos.Objects
{
    using Battlehub.RTCommon;
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
        public override void RTEOnCreated(IRTE editor)
        {
            base.RTEOnCreated(editor);
            _rteditObject.transform.localPosition = Transform.LocalPosition;
            _rteditObject.transform.localRotation = Quaternion.Euler(Transform.LocalRotation);
            _rteditObject.transform.localScale = Transform.LocalScale;
            _transform.Transform.SetParent(_rteditObject.transform, false);
            _transform.Transform.localPosition = Vector3.zero;
            _transform.Transform.localScale = Vector3.one;
            _transform.Transform.localRotation = Quaternion.identity;

            editor.RegisterCreatedObjects(new[] { _rteditObject.gameObject }, true);

            //_transform.AddComponent<ExposeToEditor>();
            //editor.RegisterCreatedObjects(new[] { _transform.Transform.gameObject }, true);
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