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
    using UIControls;

    public class EosWorkspace : MonoBehaviour
    {
        private IRTE _editor;
        public delegate void EosWorkspaceEventHandler(EosObjectBase solution);
        public static event EosWorkspaceEventHandler SolutionChanged;
        [System.NonSerialized]
        private EosObjectBase _solution;
        public EosObjectBase Solution => _solution;

        private void Awake()
        {
            IOC.Register<EosWorkspace>(this);
            VirtualizingItemContainer.DoubleClick += ItemDoubleClicked;
        }

        private void ItemDoubleClicked(VirtualizingItemContainer sender, UnityEngine.EventSystems.PointerEventData eventData)
        {
            if (!(sender.Item is ExposeToEosEditor item))
                return;
            if (item.Owner is EosScript script)
            {
                Debug.Log(script.Name);
            }
        }

        public void OpenSolution()
        {
            IOC.Resolve<IWindowManager>().CreateDialogWindow("OpenSolution", "Open Solution", OnOpenOk);
            _editor = IOC.Resolve<IRTE>();
            _editor.PlaymodeStateChanging += OnPlaymodeStateChange;
            _editor.PlaymodeStateChanged += OnPlaymodeStateChanged;
        }
        public void OpenCodeEditor()
        {

        }
        private void OnPlaymodeStateChange()
        {
            Debug.Log($"Player state changed:{_editor.IsPlaying}");
            //EosPlayer.EosPlayer.Instance.SetSolution(_solution as Solution);
            //EosPlayer.EosPlayer.Instance.Play();
        }
        private void OnPlaymodeStateChanged()
        {
            var eosplayer = EosPlayer.EosPlayer.Instance;
            if (_editor.IsPlaying)
            {
                eosplayer.PreviewSetup();
                var objmng = EosPlayer.EosPlayer.Instance.ObjectManager;
                var previewsolution = _solution.CreateCloneObjectForEditor(null) as Solution;
                objmng.RegistObject(previewsolution);
                var objects = _editor.Object.Get(false);
                foreach (var it in objects)
                {
                    if (!(it is ExposeToEosEditor eobj))
                        continue;
                    var eosobject = eosplayer.GetObjectFromEditObject(eobj.OwnerID);
                    var editeosobject = eosobject.CreateCloneObjectForEditor(eobj);
//                    editeosobject.OnCreate();
                }
                eosplayer.SetSolution(previewsolution);
                eosplayer.Play();
            }
            else
            {
                EosPlayer.EosPlayer.Instance.StopPreview();
            }
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
            catch (System.Exception ex)
            {

            }
        }
    }
}