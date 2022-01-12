using Battlehub.RTCommon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Battlehub.RTEditor;

namespace Eos.Objects
{
    public partial class EosCamera
    {
        public override void SetExposeToEditor(ExposeToEosEditor editorobject)
        {
            var camera = UnityEngine.Object.FindObjectOfType<GameViewCamera>();
            _transform.Transform = camera.transform;
        }
    }
}