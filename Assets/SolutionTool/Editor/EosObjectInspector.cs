using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Eos.Objects.Editor
{
    [CustomEditor(typeof(EosEditorObject), true)]
    [CanEditMultipleObjects]

    public class EosObjectInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
        }
    }
}