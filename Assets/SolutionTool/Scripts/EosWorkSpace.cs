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
            IOC.Resolve<IWindowManager>().CreateDialogWindow("OpenSolution", "ddd", OnOk);
        }
        private void OnOk(Dialog dialog_, DialogCancelArgs arg)
        {
            if (arg.Cancel)
                return;
            _solution?.Destroy();
            var editor = IOC.Resolve<IRTE>();
            var dialog = dialog_.Content.GetComponent<OpenFileDialog>();
            var msgpackData = File.ReadAllBytes(dialog.Path);
            var desolution = _solution = MessagePackSerializer.Deserialize<EosObjectBase>(msgpackData, MessagePackSerializerOptions.Standard);
//            var desolution = MessagePackSerializer.Deserialize<EosObjectBase>(msgpackData, MessagePackSerializerOptions.Standard);
            SolutionChanged?.Invoke(desolution);
        }
    }
}