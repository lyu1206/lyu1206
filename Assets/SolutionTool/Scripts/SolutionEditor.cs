//#if UNITY_EDITOR
//using System;
//using System.IO;
//using UnityEngine;
//using System.Linq;
//using System.Collections.Generic;
//using Ludiq;
//using MessagePack;
//using Unity.EditorCoroutines.Editor;
//using UnityEditor;

//namespace Eos.Objects.Editor
//{
//    using EosPlayer;
//    using Service;
//    using Objects;
//    public class SolutionEditor : MonoBehaviour
//    {
//        public static SolutionEditor Instance;
//        public EosPlayer Ref => EosPlayer.Instance;

//        private static EosEditorObject _solution;
//        private static EosEditorObject _guiservice;
//        private CoroutineManager _coroutineManager; 
//        private static List<EosEditorObject> _objectlist = new List<EosEditorObject>();

//        public static EosEditorObject Solution => _solution;
//        public static CoroutineManager CoroutineManager => Instance._coroutineManager;

//        // Start is called before the first frame update
//        void Start()
//        {
//            Instance = this;
//            _coroutineManager = Ref.Coroutine;
            
//            Selection.activeObject = gameObject;
//        }

//        public static EosEditorObject CreateObject<T>(EosEditorObject parent = null) where T : EosObjectBase
//        {
//            return CreateObject(typeof(T),parent);
//        }        
//        public static EosEditorObject CreateObject(Type type, EosEditorObject parent =null)
//        {
//            var eosobject = Activator.CreateInstance(type) as EosObjectBase;
//            eosobject.ObjectID = eosobject.GetHashCode();
//            eosobject.Name = eosobject.GetType().Name;
//            return CreateObject(eosobject, parent);
//        }
//        public static EosEditorObject CreateObject(EosObjectBase eosobject, EosEditorObject parent =null)
//        {
//            if (parent != null && eosobject != null)
//                eosobject.Parent = parent.Object;
//            var eosobejctname = "Missing";
//            if (eosobject != null)
//                eosobejctname = eosobject.Name;
//            var mono = new GameObject(eosobejctname);

//            if (eosobject != null)
//            {
//                var inspectortypename = $"{eosobject.GetType().Name}_Inspector";
//                var inspectortype = Type.GetType(inspectortypename);
//                if (inspectortype == null)
//                    inspectortype = typeof(EosEditorObject);

//                var mono_eosobject = mono.AddComponent(inspectortype) as EosEditorObject;
//                mono_eosobject.Object = eosobject;
//                EditorCoroutineUtility.StartCoroutine(eosobject.EditorCreate(mono_eosobject), eosobject);
//                if (parent != null)
//                    mono.transform.SetParent(parent.transform);
//                mono.transform.localPosition = Vector3.zero;
//                mono.transform.localScale = Vector3.one;
//                _objectlist.Add(mono_eosobject);
//                return mono_eosobject;
//            }
//            return null;
//        }
//        public static EosObjectBase SetupMap(GameObject mapobj)
//        {
//            if (_solution!=null)
//                GameObject.DestroyImmediate(_solution.gameObject);
//            var solution = CreateObject<EosSolution>(null);
            
//            var workspace = solution.AddChild<WorkSpace>();
            
//            var voxel = workspace.AddChild<Voxel>();
//            (voxel.Object as Voxel).SetMap(mapobj);
            
//            solution.AddChild<GUIService>();
//            solution.AddChild<InputService>();
//            solution.Object.Name = "Map01";
//            _solution = solution;
//            Selection.activeObject = _solution.gameObject;
//            return _solution.Object;
//        }

//        private static void ReadyUI()
//        {
//            if (_guiservice!=null)
//                GameObject.DestroyImmediate(_guiservice.gameObject);
//            if (_solution!=null)
//                GameObject.DestroyImmediate(_solution.gameObject);
//            _guiservice = CreateObject<GUIService>();
//            var cameraobj = new GameObject("UICamera");
//            cameraobj.transform.SetParent(_guiservice.transform);
//            var camera = cameraobj.AddComponent<Camera>();
//            camera.clearFlags = CameraClearFlags.Color;
//            camera.backgroundColor = Color.black;
//        }
//        public static EosObjectBase SetupUI()
//        {
//            // ReadyUI();
//            // var solution = _guiservice.AddChild<EosUISolution>();
//            var solution = CreateObject<EosUISolution>();
//            solution.Object.Name = "UI01";
//            _solution = solution;
//            Selection.activeObject = _solution.gameObject;
//            return _solution.Object;
//        }

//        public static void OpenSolution(string path)
//        {
//            if (_solution!=null)
//                GameObject.DestroyImmediate(_solution.gameObject);
//            var msgpackData = File.ReadAllBytes(path);
//            var desolution = MessagePack.MessagePackSerializer.Deserialize<EosObjectBase>(msgpackData,MessagePackSerializerOptions.Standard);
//            _objectlist.Clear();
//            _solution = CreateObject(desolution);
//            desolution.IterChildRecursive((parent,obj) =>
//            {
//                var mono_parenteosobject = _objectlist.Find(o => o.Object == parent);
//                CreateObject(obj,mono_parenteosobject);
//                return true;
//            });
//            _objectlist.ForEach(o=>o.Object.Created());
            
//            // _objectlist.ForEach(o=>o.Object.ID = o.Object.GetHashCode());
            
//            // _objectlist.ForEach(o => o.Object.EditorCreate(o));
//        }
//        public static void SaveSolution(string path)
//        {
//            _objectlist.ForEach(o=>o.Object.ClearRelation());
//            _objectlist.Clear();
//            var solution = GetEditorSolution();
//            var msgpackData = MessagePack.MessagePackSerializer.Serialize(solution);
//            File.WriteAllBytes(path,msgpackData);
//        }

//        public static EosObjectBase GetEosSolution()
//        {
//            return GetEditorSolution();
//        }

//        public void Preview()
//        {
//            _objectlist.ForEach(o=>
//            {
//                o.Object.ClearRelation();
//            });
//            _objectlist.Clear();
//            var sol = _solution.Object as Eos.Objects.Solution;
//            GameObject.Destroy(sol.SolutionRoot.gameObject);
//            var solution = GetEditorSolution();
//            var msgpackData = MessagePack.MessagePackSerializer.Serialize(solution);
//            solution = MessagePack.MessagePackSerializer.Deserialize<EosSolution>(msgpackData);
//            gameObject.SetActive(false);
//            EoPlayer.Play(solution as Solution);
//        }
//        public void StopPreview()
//        {
//            gameObject.SetActive(true);
//            EoPlayer.Stop();
//        }

//        private static EosObjectBase GetEditorSolution()
//        {
//            return GetEditorSolution(_solution);
//            // var solution = _solution.Object as EosSolution;
//            // _objectlist.Add(_solution);
//            // _solution.Iterchild((obj) =>
//            // {
//            //     _objectlist.Add(obj);
//            //     obj.Object.Name = obj.name;
//            //     obj.Object.EditorApply(obj);
//            // });
//            // return solution;
//        }
//        public static EosObjectBase GetEditorSolution(EosObject solutiongameobejct)
//        {
//            var solution = solutiongameobejct.Object as EosObjectBase;
//            _objectlist.Add(solutiongameobejct);
//            solutiongameobejct.Iterchild((obj) =>
//            {
//                _objectlist.Add(obj);
//                obj.Object.Name = obj.name;
//                obj.Object.EditorApply(obj);
//            });
//            return solution;
//        }

//        public static T GetObject<T>() where T : EosObjectBase
//        {
//            var findobj = _objectlist.Find(o => o.Object.GetType() == typeof(T));
//            return findobj.Object as T;
//        }
//        public T GetInspectorObject<T>() where T : EosObject
//        {
//            var findobj = _objectlist.Find(o => o.GetType() == typeof(T));
//            return findobj as T;
//        }
//    }
//}

//public class AttributeCaches  
//{
//    //이건 당장 쓸것이 아니다.다만 미래에 쓸수도 있을지 몰라 예제삼아 넣어 본다.
//    public static void Initialize()
//    {
//        var type = typeof(EosObjectBase);
//        var types = AppDomain.CurrentDomain.GetAssemblies()
//            .SelectMany(x => x.GetTypes())
//            .Where(x => x.IsClass && type.IsAssignableFrom(x));
//    }

//    public static Type[] GetTypeNames<T>()
//    {
//        var type = typeof(T);
//        var types = AppDomain.CurrentDomain.GetAssemblies()
//            .SelectMany(x => x.GetTypes())
//            .Where(x => x.IsClass && type.IsAssignableFrom(x));

//        var names = new List<string>();
//        foreach (var t in types)
//            names.Add(t.Name);
//        return types.ToArray();
//    }
//}
//#endif
