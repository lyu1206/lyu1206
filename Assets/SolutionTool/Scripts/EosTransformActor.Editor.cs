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
    public partial class EosTransformActor
    {
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
#endif