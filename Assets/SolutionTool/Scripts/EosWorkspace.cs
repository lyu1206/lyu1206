using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Battlehub.RTEditor
{
    using Eos.Objects;
    using Eos.Service;
    using RTCommon;
    using MessagePack;
    using UIControls.Dialogs;
    using UIControls;

    public class ScriptEditor
    {
        private static FileSystemWatcher _watcher;
        public static void OpenScript(string path)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.FileName = $"cmd.exe";
            startInfo.Arguments = $"/k code -r {path}";
            Process.Start(startInfo);

            //startInfo.CreateNoWindow = true;
            //startInfo.UseShellExecute = false;
            //startInfo.FileName = $"cmd.exe";
            //startInfo.Arguments = $"/k code -g {Path.Combine(Application.persistentDataPath, "Solution/Test3.lua")}";
            //Process.Start(startInfo);
        }
        public static void Open(string path)
        {
            _watcher?.Dispose();
            var watcher = _watcher = new FileSystemWatcher(path);

            watcher.NotifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Security
                                 | NotifyFilters.Size;

            watcher.Changed += OnChanged; 
            watcher.Created += OnCreated;
            watcher.Deleted += OnDeleted;
            watcher.Renamed += OnRenamed;
            watcher.Error += OnError; ;

            watcher.Filter = "*.lua";
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;

            UnityEngine.Debug.Log("script editer opened..");


            //            var processInfo = new ProcessStartInfo("cmd.exe", @"/c code.exe");
            //            var processInfo = new ProcessStartInfo("cmd.exe");
            //            processInfo.CreateNoWindow = true;
            //            processInfo.UseShellExecute = false;

            //            var process = Process.Start(processInfo);

            //process.WaitForExit();
            //process.Close();
        }

        private static void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }
            UnityEngine.Debug.Log($"Changed: {e.FullPath}");
        }
        private static void OnCreated(object sender, FileSystemEventArgs e)
        {
        }
        private static void OnDeleted(object sender, FileSystemEventArgs e)
        {
        }
        private static void OnRenamed(object sender, FileSystemEventArgs e)
        {
        }
        private static void OnError(object sender, ErrorEventArgs e)
        {
        }
    }
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
            _editor = IOC.Resolve<IRTE>();
            _editor.PlaymodeStateChanging += OnPlaymodeStateChange;
            _editor.PlaymodeStateChanged += OnPlaymodeStateChanged;
        }

        private void ItemDoubleClicked(VirtualizingItemContainer sender, UnityEngine.EventSystems.PointerEventData eventData)
        {
            if (!(sender.Item is ExposeToEosEditor item))
                return;
            if (item.Owner is EosScript script)
            {
                var ws = IOC.Resolve<EosWorkspace>();
                script.ReadyForEdit();
                ScriptEditor.Open(ws.GetScriptRootPath(script));
                
                //var temppath = System.IO.Path.Combine(Application.persistentDataPath, $"{this.Name}.lua");

                //File.WriteAllText(temppath, script.LuaScript);
                //void LoadYourPhysicsProgram()
                //{
                //    ProcessStartInfo startInfo = new ProcessStartInfo();
                //    //startInfo.FileName = "PHYSICSPROGRAM.EXE";
                //    //startInfo.FileName = $"C:/Program Files (x86)/Notepad++/notepad++.exe";
                //    startInfo.FileName = $"C:/Users/kmioc/AppData/Local/Programs/Microsoft VS Code/Code.exe";
                //    startInfo.Arguments = $"-r {temppath}";
                //    Process.Start(startInfo);
                //}
                //LoadYourPhysicsProgram();
                //Debug.Log(script.Name);
            }
        }
        public string GetScriptRootPath(EosObjectBase obj)
        {
            var path = Path.Combine(Path.Combine(Application.persistentDataPath, "Scripts"), obj.Ref.Solution.MeataData.Guid);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }
        public string GetScriptPath(EosObjectBase obj)
        {
            var path = GetScriptRootPath(obj);
            path = Path.Combine(path,obj.MeataData.Guid);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;        
        }
        public void OpenSolution()
        {
            IOC.Resolve<IWindowManager>().CreateDialogWindow("OpenSolution", "Open Solution", OnOpenOk);
            var opendialog = IOC.Resolve<IOpenFileDialog>();
            opendialog.Extensions = new string[] { ".solution" };
        }
        public void OpenCodeEditor()
        {

        }
        public void NewSolution()
        {
            IOC.Resolve<IWindowManager>().CreateDialogWindow("OpenSolution", "Newn Solution", OnNewSolutionOk);
            var opendialog = IOC.Resolve<IOpenFileDialog>();
            opendialog.Extensions = new string[] { ".solution" };
        }
        private void OnNewSolutionOk(Dialog dialog_, DialogCancelArgs arg)
        {

        }
        private void OnPlaymodeStateChange()
        {
            //Debug.Log($"Player state changed:{_editor.IsPlaying}");
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
            var savedialog = IOC.Resolve<ISaveFileDialog>();
            savedialog.Extensions = new string[] { ".solution" };
        }
        private void OnSaveOk(Dialog dialog_, DialogCancelArgs arg)
        {
            if (arg.Cancel)
                return;
            var dialog = dialog_.Content.GetComponent<SaveFileDialog>();
            var path = dialog.Path;
            var msgpackData = MessagePack.MessagePackSerializer.Serialize(_solution);
            File.WriteAllBytes(path, msgpackData);

            var metadatafilename = dialog.Path.Replace(".solution", ".metadata");
            var metaData = MessagePackSerializer.Serialize(((Solution)_solution).SolutionMetaData);
            File.WriteAllBytes(metadatafilename, metaData);

            UnityEngine.Debug.Log($"{Path.GetFileName(path)} save done..");
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
                EosPlayer.EosPlayer.Instance.SetSolution(desolution as Solution);
                //            var desolution = MessagePackSerializer.Deserialize<EosObjectBase>(msgpackData, MessagePackSerializerOptions.Standard);
                SolutionChanged?.Invoke(desolution);
            }
            catch (System.Exception ex)
            {

            }
            var metadatafilename = dialog.Path.Replace(".solution", ".metadata");
            if (File.Exists(metadatafilename))
            {
                var metaDataBytes = File.ReadAllBytes(metadatafilename);
                var metadata = MessagePackSerializer.Deserialize<SolutionMetaData>(metaDataBytes, MessagePackSerializerOptions.Standard);
                ((Solution)_solution).SolutionMetaData = metadata;
            }
        }
    }
}