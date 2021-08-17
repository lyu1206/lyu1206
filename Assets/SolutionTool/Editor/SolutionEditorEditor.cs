using System;
using System.IO;
using System.Net;
using Bolt;
using UnityEditor;
using UnityEngine;
using Eos.Objects;
using UnityEditor.Experimental;
using UnityEditor.SceneManagement;
using UnityEngine.AI;
using Object = UnityEngine.Object;

using Eos.Service;
using Eos.Editor;

public class SolutionEditorEditor : EditorWindow
{
    private const string str_tablepath = "Assets/Resources/Tables/";
    
    private static string[] _eosobjecttypenames;
    private static Type[] _eosobjecttypefullnames;
    
    public static string[] EosObjectNames => _eosobjecttypenames;
    public static Type[] EosObjectFullNames => _eosobjecttypefullnames;
    
    private static SolutionEditorEditor _window;
    //private SolutionEditor _editor;
    private static string _name = "Solution";
    private bool _ispreview;
    //private Solution _solution;
    private Object _mapobj;
    private string _currentloadpath;
    private GameObject _solutiongameobject;
    private Action _onStopAction;

    [MenuItem("Tools/Eos/Soltion Editor")]
    private static void Open()
    {
        if (_window!=null)
            _window.Close();

        GetWindow<SolutionEditorEditor>("Solution Editor").Show();
    }

    private void OnEnable()
    {
        _window = this; // set singleton.
        if (_eosobjecttypenames != null)
            return;
        var types = AttributeCaches.GetAvailableTypeNames<EosObjectBase>(null);

        _eosobjecttypenames = new string[types.Length];
        _eosobjecttypefullnames = new Type[types.Length];
        for (var i = 0; i < types.Length; i++)
        {
            _eosobjecttypenames[i] = types[i].Name;
            _eosobjecttypefullnames[i] = types[i];
        }

        //var eosNpcTable = Resources.Load<EosNPCTable>("Tables/NpcTable");
        //eosNpcTable.InitTable();
        //var itemTable = Resources.Load<ItemTable>("Tables/ItemTable");
        //itemTable.InitTable();
        //var characterTable = Resources.Load<CharacterTable>("Tables/CharacterTable");
        //characterTable.InitTable();
    }
    private void SetEditorviewRoot()
    {
        var root = new GameObject("SolutionView");
        ObjectFactory.UnityInstanceRoot = root.transform;
    }
    private void OnGUI()
    {
        if (GUILayout.Button("Tool Scene"))
        {
            EditorSceneManager.OpenScene("Assets/SolutionTool/SolutionEditor.unity");
            SetEditorviewRoot();
            SolutionEditor.CreateSolution();
        }
        if (GUILayout.Button("Game Scene"))
        {
            EditorSceneManager.OpenScene("Assets/Scenes/App.unity");
        }
        if (GUILayout.Button("Open Solution"))
        {
            var path = _currentloadpath = EditorUtility.OpenFilePanel("Solution", $"{Application.streamingAssetsPath}/Solutions", "solution");
            if (string.IsNullOrEmpty(path))
                return;
            EditorSceneManager.OpenScene("Assets/SolutionTool/SolutionEditor.unity");
            SetEditorviewRoot();
            SolutionEditor.OpenSolution(path);
            var we = EditorWindow.GetWindow<ObjectHierachy>();
            we.Show();


            //_solution = SolutionEditor.Solution.Object as Solution;
            //_name = SolutionEditor.Solution.name;
        }
        GUILayout.BeginHorizontal();
        _name = GUILayout.TextField(_name);
        if (GUILayout.Button("Save Solution"))
        {
            var path = EditorUtility.SaveFilePanel("Solution", $"{Application.streamingAssetsPath}/Solutions", _name,"solution");
            SolutionEditor.SaveSolution(path);
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        _mapobj = EditorGUILayout.ObjectField(_mapobj,typeof(GameObject),false);
        if (GUILayout.Button("Map Solution"))
        {
            //if (_mapobj==null)
            //    return;
            //var mapsolution = _solution = SolutionEditor.SetupMap(_mapobj as GameObject) as Solution;
            //_name = mapsolution.Name;
        }
        GUILayout.EndHorizontal();
        if (GUILayout.Button("UI Solution"))
        {
            var scenecam = SceneView.lastActiveSceneView.camera.transform;
            NavMeshHit hit;
            var succ = NavMesh.Raycast(scenecam.position, scenecam.position + scenecam.forward * 1000,out hit, NavMesh.AllAreas);
            var testobj = new GameObject("TestObject");
            Debug.DrawRay(scenecam.position, scenecam.forward);
            testobj.transform.position = scenecam.position + scenecam.forward * 100;
            if (succ)
                Debug.Log("Succ");
            //_name = SolutionEditor.SetupUI().Name;            
        }
        if (!_ispreview && GUILayout.Button("Preview Solution"))
        {
            //var temppath = $"{Application.persistentDataPath}/preview.solution";
            //SolutionEditor.SaveSolution(temppath);
            //_solutiongameobject = SolutionEditor.Solution.gameObject;
            //GameObject.DestroyImmediate(_solutiongameobject);
            //EditorApplication.ExecuteMenuItem("Edit/Play");
            //_ispreview = true;
        }
        else if (_ispreview && GUILayout.Button("Stop Preview"))
        {
            //EditorApplication.ExecuteMenuItem("Edit/Play");
            //EosEditorPlayer.Stop();
            //_onStopAction = () =>
            //{
            //    SolutionEditor.OpenSolution(_currentloadpath);
            //    _ispreview = false;
            //};
            //// SolutionEditor.Instance.StopPreview();
        }
        if (_onStopAction!=null && _ispreview && !EditorApplication.isPlaying)
        {
            _onStopAction.Invoke();
            _onStopAction = null;
        }

        if (GUILayout.Button("To json"))
        {
            var json = JsonUtility.ToJson(Selection.activeObject, true);
            System.IO.File.WriteAllText($"{Application.dataPath}/AssetDB/Jsons/{Selection.activeObject.name}.json",json);
            AssetDatabase.Refresh();
        }
        if (GUILayout.Button("Create Table"))
        {
            //var table = ScriptableObject.CreateInstance<ItemRewardTable>();
            //var path = $"{str_tablepath}ItemRewardTable.asset";
            //AssetDatabase.CreateAsset(table , path);
            //AssetDatabase.Refresh();
        }
        if (GUILayout.Button("Create Mold"))
        {
            GuCore.Editors.GEditorUtil.CreateAsset<Eos.Assets.Mold>("Assets/Mold.asset");
            //Debug.Log(EoPlayer.ObjectManager.ObjectCount);
        }
    }
}
