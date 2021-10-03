using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Battlehub.UIControls.MenuControl;
namespace Battlehub.RTEditor
{
    using Eos.Objects;
    using Eos.Service;
    using RTCommon;
    using MessagePack;
    using UIControls.Dialogs;
    public class EosWorkSpace : MonoBehaviour
    {
        public delegate void EosWorkspaceEventHandler(Solution solution);
        public static event EosWorkspaceEventHandler SolutionChanged;
        private Solution _solution;

        private void Awake()
        {
//            IOC.Register<EosWorkSpace>(this);
        }
        public void OpenSolution()
        {
            IOC.Resolve<IWindowManager>().CreateDialogWindow("OpenSolution", "ddd", OnOk);
        }
        private void OnOk(Dialog dialog_,DialogCancelArgs arg)
        {
            if (arg.Cancel)
                return;
            _solution?.Destroy();
            var editor = IOC.Resolve<IRTE>();
            var dialog = dialog_.Content.GetComponent<OpenFileDialog>();
            var msgpackData = File.ReadAllBytes(dialog.Path);
            var desolution = _solution = MessagePackSerializer.Deserialize<Solution>(msgpackData, MessagePackSerializerOptions.Standard);
            SolutionChanged?.Invoke(desolution);
        }
    }
}