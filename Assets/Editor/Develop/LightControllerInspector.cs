using GuCore.Editors;
using UnityEditor;
using UnityEngine;
using Eos.Ore;

[CustomEditor(typeof(LightController))]
public class LightControllerInspector : Editor
{
	private string _prfabName;
	LightController _myScript;
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		
		_myScript = (LightController)target;
		GUILayout.BeginHorizontal();
		var type = PrefabUtility.GetPrefabType(_myScript.gameObject);
		if (type == PrefabType.Prefab || type == PrefabType.ModelPrefab)
		{
			GUILayout.EndHorizontal();

			return;
		}
		_prfabName = GUILayout.TextField(_prfabName);
		if(GUILayout.Button("CreatePrefab"))
		{
			GEditorUtil.ReplacePrefab(_myScript.gameObject, string.Format("Assets/Resources/Game/Env/Light/{0}.prefab", _prfabName));
		}
		GUILayout.EndHorizontal();
		
		_myScript.UpdateColor();
	}
}
