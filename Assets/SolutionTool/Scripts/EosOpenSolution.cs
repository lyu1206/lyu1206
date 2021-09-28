using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battlehub.RTEditor
{
    using MessagePack;
    using Eos.Objects;
    using RTCommon;
    using RTHandles;
    public class EosOpenSolution : OpenFileDialog
    {
        protected override void OnDestroyOverride()
        {
            base.OnDestroyOverride();
            if (!string.IsNullOrEmpty(Path))
            {
                Debug.Log($"Load solution:{Path}");
                //SetEditorviewRoot();
                var msgpackData = File.ReadAllBytes(Path);
                var desolution = MessagePackSerializer.Deserialize<EosObjectBase>(msgpackData, MessagePackSerializerOptions.Standard);
                desolution.CreatedOnEditor();
                desolution.IterChilds((child) =>
                {
//                    child.CreatedOnEditor();
                    child.RTEOnCreated(Editor);
                }, true);
            }
        }
        private void SetEditorviewRoot()
        {
            var root = new GameObject("SolutionView");
            ObjectFactory.UnityInstanceRoot = root.transform;

            /*
            Vector3 pivot = Vector3.zero;
            var editor = Editor;
            IRuntimeSelectionComponent selectionComponent = editor.GetScenePivot();
            if (selectionComponent != null)
            {
                pivot = selectionComponent.SecondaryPivot;
            }
            var go = root;

//            go.transform.position = pivot;
            if (go.GetComponent<ExposeToEditor>() == null)
            {
                go.AddComponent<ExposeToEditor>();
            }

            go.SetActive(true);
            editor.RegisterCreatedObjects(new[] { go }, selectionComponent != null ? selectionComponent.CanSelect : true);

            var child = new GameObject("ddddd");
            child.transform.SetParent(go.transform);
            child.AddComponent<ExposeToEditor>();
            child.SetActive(true);
            editor.RegisterCreatedObjects(new[] { child }, selectionComponent != null ? selectionComponent.CanSelect : true);

            //            Editor.AddGameObjectToScene(root);
            */
        }
    }
}