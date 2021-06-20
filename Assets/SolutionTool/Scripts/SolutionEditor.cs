#if UNITY_EDITOR
using System;
using System.IO;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Ludiq;
using MessagePack;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using Eos.Service;

namespace Eos.Editor
{
    using Objects.Editor;
    using EosPlayer;
    using Objects;
    [ExecuteInEditMode]
    public class SolutionEditor : MonoBehaviour
    {
        private static EosEditorObject _solution;
        [SerializeField]
        private Camera _editorcamera;
        public static SolutionEditor Instance;

        // Start is called before the first frame update
        void Awake()
        {
            Instance = this;
        }
        private void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }
        void OnSceneGUI(SceneView view)
        {
            //Event e = Event.current;
            //if (e != null)
            //{
            //    switch (e.type)
            //    {
            //        case EventType.KeyDown:
            //            view.camera.transform.localPosition += new Vector3(2f, 0, 0);
            //            break;
            //    }
            //}
        }
        public static void CreateSolution()
        {
            _solution = Service.Solution.CreateEditorImpl();
        }
        //public static EosObjectBase SetupMap(GameObject mapobj)
        //{
        //    if (_solution != null)
        //        GameObject.DestroyImmediate(_solution.gameObject);
        //    var solution = CreateObject<EosSolution>(null);

        //    var workspace = solution.AddChild<WorkSpace>();

        //    var voxel = workspace.AddChild<Voxel>();
        //    (voxel.Object as Voxel).SetMap(mapobj);

        //    solution.AddChild<GUIService>();
        //    solution.AddChild<InputService>();
        //    solution.Object.Name = "Map01";
        //    _solution = solution;
        //    Selection.activeObject = _solution.gameObject;
        //    return _solution.Object;
        //}

        //private static void ReadyUI()
        //{
        //    if (_guiservice != null)
        //        GameObject.DestroyImmediate(_guiservice.gameObject);
        //    if (_solution != null)
        //        GameObject.DestroyImmediate(_solution.gameObject);
        //    _guiservice = CreateObject<GUIService>();
        //    var cameraobj = new GameObject("UICamera");
        //    cameraobj.transform.SetParent(_guiservice.transform);
        //    var camera = cameraobj.AddComponent<Camera>();
        //    camera.clearFlags = CameraClearFlags.Color;
        //    camera.backgroundColor = Color.black;
        //}
        //public static EosObjectBase SetupUI()
        //{
        //    // ReadyUI();
        //    // var solution = _guiservice.AddChild<EosUISolution>();
        //    var solution = CreateObject<EosUISolution>();
        //    solution.Object.Name = "UI01";
        //    _solution = solution;
        //    Selection.activeObject = _solution.gameObject;
        //    return _solution.Object;
        //}

        public static void OpenSolution(string path)
        {
            var msgpackData = File.ReadAllBytes(path);
            var desolution = MessagePackSerializer.Deserialize<EosObjectBase>(msgpackData, MessagePackSerializerOptions.Standard);
            _solution = desolution.CreateForEditor(null);

            //if (_solution != null)
            //    GameObject.DestroyImmediate(_solution.gameObject);
            //_objectlist.Clear();
            //_solution = CreateObject(desolution);
            //desolution.IterChildRecursive((parent, obj) =>
            //{
            //    var mono_parenteosobject = _objectlist.Find(o => o.Object == parent);
            //    CreateObject(obj, mono_parenteosobject);
            //    return true;
            //});
            //_objectlist.ForEach(o => o.Object.Created());

            //// _objectlist.ForEach(o=>o.Object.ID = o.Object.GetHashCode());

            //// _objectlist.ForEach(o => o.Object.EditorCreate(o));
        }
        public static void SaveSolution(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;
            _solution.ApplyObject(null);
            var msgpackData = MessagePack.MessagePackSerializer.Serialize(_solution.Owner);
            File.WriteAllBytes(path, msgpackData);
            AssetDatabase.Refresh();
        }
        public void Preview()
        {
            //_objectlist.ForEach(o =>
            //{
            //    o.Object.ClearRelation();
            //});
            //_objectlist.Clear();
            //var sol = _solution.Object as Eos.Objects.Solution;
            //GameObject.Destroy(sol.SolutionRoot.gameObject);
            //var solution = GetEditorSolution();
            //var msgpackData = MessagePack.MessagePackSerializer.Serialize(solution);
            //solution = MessagePack.MessagePackSerializer.Deserialize<EosSolution>(msgpackData);
            //gameObject.SetActive(false);
            //EoPlayer.Play(solution as Solution);
        }
        public void StopPreview()
        {
            //gameObject.SetActive(true);
            //EoPlayer.Stop();
        }

    }
}
#endif
