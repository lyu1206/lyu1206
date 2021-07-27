#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MessagePack;

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
            obj.transform.localPosition = Transform.LocalPosition;
            obj.transform.localRotation = Quaternion.Euler(Transform.LocalRotation);
            obj.transform.localScale = Transform.LocalScale;
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