using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battlehub.RTEditor
{
    using Eos.Objects;
    using Eos.Service;
    using RTCommon;
    using MessagePack;
    using UIControls.Dialogs;
    public class EosWorkspace : MonoBehaviour
    {
        public delegate void EosWorkspaceEventHandler(EosObjectBase solution);
        public static event EosWorkspaceEventHandler SolutionChanged;
        [System.NonSerialized]
        private EosObjectBase _solution;
        public EosObjectBase Solution => _solution;

        private void Awake()
        {
            IOC.Register<EosWorkspace>(this);
        }
        public void OpenSolution()
        {
            IOC.Resolve<IWindowManager>().CreateDialogWindow("OpenSolution", "Open Solution", OnOpenOk);
        }
        public void SaveAsSolution()
        {
            IOC.Resolve<IWindowManager>().CreateDialogWindow("SaveSolution", "Save Solution", OnSaveOk);
        }
        private void OnSaveOk(Dialog dialog_, DialogCancelArgs arg)
        {
            if (arg.Cancel)
                return;
            var dialog = dialog_.Content.GetComponent<SaveFileDialog>();
            var path = dialog.Path;
            var msgpackData = MessagePack.MessagePackSerializer.Serialize(_solution);
            File.WriteAllBytes(path, msgpackData);
            Debug.Log($"save done..");
        }
        private void OnOpenOk(Dialog dialog_, DialogCancelArgs arg)
        {
            if (arg.Cancel)
                return;
            _solution?.Destroy();
            var editor = IOC.Resolve<IRTE>();
            var dialog = dialog_.Content.GetComponent<OpenFileDialog>();
            var msgpackData = File.ReadAllBytes(dialog.Path);
            try
            {
                var desolution = _solution = MessagePackSerializer.Deserialize<EosObjectBase>(msgpackData, MessagePackSerializerOptions.Standard);
                //            var desolution = MessagePackSerializer.Deserialize<EosObjectBase>(msgpackData, MessagePackSerializerOptions.Standard);
                SolutionChanged?.Invoke(desolution);
            }
            catch(System.Exception ex)
            {

            }
        }
    }
}