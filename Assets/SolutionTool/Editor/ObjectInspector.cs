using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using UnityEditor.IMGUI.Controls;

namespace Eos.Objects.Editor
{
    using Ore;
    using Assets;

    public class ObjectInspector : EditorWindow
    {
        private InspectorComponent _addchildcomponent;
        private EosObjectBase _targeteosobject;
        private Dictionary<string, List<InspectorComponent.InspectorSet>> _inspectors;
        [MenuItem("Tools/Inspector")]
        public static void ShowInspector()
        {
            var wnd = GetWindow<ObjectInspector>();
            wnd.titleContent = new GUIContent("Eos Inspector");
        }
        public void SetTarget(int objID)
        {
            var sol = Eos.Editor.SolutionEditor.EosSolution;
            var target = sol.Ref.ObjectManager[(uint)objID];
            _targeteosobject = target;
            _inspectors = InspectorComponent.GetInspectors(_targeteosobject);
            if (_inspectors == null)
                return;
            _addchildcomponent = _addchildcomponent ?? new AddChildCompoent();
            _addchildcomponent.Init(_targeteosobject, null, null);
        }
        void OnGUI()
        {
            GUILayout.Label(_targeteosobject.Name);
            foreach (var inspector in _inspectors)
            {
                GUILayout.Label($"[ {inspector.Key} ]");
                GUILayout.Space(5);
                foreach (var it in inspector.Value)
                    it.OnInspectorGUI();
                CustomLightingMasterGUI.Separator();
            }
            _addchildcomponent.OnInspectorGUI(null, null);
        }
    }
    public abstract class InspectorComponent
    {
        private static Dictionary<Type, Type> _inspectorcomponentsDic = new Dictionary<Type, Type>();
        protected EosObjectBase _target;
        protected EosObjectBase _edittarget;
        protected EosObjectBase _edittargetparent;
        public struct InspectorSet
        {
            public InspectorAttribute Info;
            public PropertyInfo PropertyInfo;
            public InspectorComponent Component;
            public void OnInspectorGUI()
            {
                Component.OnInspectorGUI(Info, PropertyInfo);
            }
        }
        protected void ApplyProperty(PropertyInfo prop, object value)
        {
            prop.SetValue(_edittarget, value);
        }
        public static Dictionary<string, List<InspectorSet>> GetInspectors(EosObjectBase edittarget)
        {
            _inspectorcomponentsDic.Clear();
            _inspectorcomponentsDic.Add(typeof(string), typeof(StringComponent));
            _inspectorcomponentsDic.Add(typeof(OreReference), typeof(OreReferenceComponent));
            _inspectorcomponentsDic.Add(typeof(int), typeof(NotImpl));
            _inspectorcomponentsDic.Add(typeof(float), typeof(NotImpl));
            _inspectorcomponentsDic.Add(typeof(bool), typeof(BoolComponent));
            _inspectorcomponentsDic.Add(typeof(Vector2), typeof(NotImpl));
            _inspectorcomponentsDic.Add(typeof(Vector3), typeof(Vector3Component));
            _inspectorcomponentsDic.Add(typeof(Enum), typeof(EnumComponent));

            var bindflag = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            if (edittarget == null)
                return null;
            var properties = edittarget.GetType().GetProperties(bindflag);
            var inspectorsets = new Dictionary<string, List<InspectorSet>>();
            var inspectproperties = properties.Select((property, index) => new Tuple<PropertyInfo, InspectorAttribute>(property, property.GetCustomAttribute<InspectorAttribute>()))
                    .Where(it =>
                    {
                        if (it.Item2 != null)
                        {
                            var proptype = it.Item1.PropertyType;
                            var checklist = _inspectorcomponentsDic.Keys.Where(key => key.IsAssignableFrom(proptype));
                            var count = checklist.Count<Type>();
                            return count > 0;
                        }
                        return false;
                    })
                    .OrderBy(info => info.Item2.Category)
                    .Select((info, index) =>
                    {
                        var ret = new InspectorSet { Info = info.Item2, PropertyInfo = info.Item1 };
                        var component = _inspectorcomponentsDic.Keys.Where(key => key.IsAssignableFrom(info.Item1.PropertyType));
                        var tt = _inspectorcomponentsDic[component.First<Type>()];
                        ret.Component = Activator.CreateInstance(tt) as InspectorComponent;
                        if (!inspectorsets.ContainsKey(ret.Info.Category))
                            inspectorsets.Add(ret.Info.Category, new List<InspectorSet>());
                        inspectorsets[ret.Info.Category].Add(ret);
                        EosObjectBase parent = null;
                        if (edittarget.Parent != null)
                        {
                            var parenteditobject = edittarget.Parent;
                            if (parenteditobject != null)
                                parent = parenteditobject;
                        }
                        ret.Component.Init(edittarget, ret.PropertyInfo, parent);
                        return ret;
                    })
                    ;

            foreach (var gg in inspectproperties)
            {
            }
            return inspectorsets;
        }
        public virtual void Init(EosObjectBase target, PropertyInfo property, EosObjectBase parent)
        {
            _target = target;
            _edittargetparent = parent;
            _edittarget = target;
        }
        public abstract void OnInspectorGUI(InspectorAttribute info, PropertyInfo property);
    }
    public class NotImpl : InspectorComponent
    {
        public override void OnInspectorGUI(InspectorAttribute info, PropertyInfo property)
        {
            GUILayout.Label("Not impl");
        }
    }
    public class Vector3Component : InspectorComponent
    {
        private Vector3 _value;
        public override void Init(EosObjectBase target, PropertyInfo property, EosObjectBase parent)
        {
            base.Init(target, property, parent);
            _value = (Vector3)property.GetValue(_edittarget);
        }
        public override void OnInspectorGUI(InspectorAttribute info, PropertyInfo property)
        {
            GUILayout.BeginHorizontal();
            GUI.changed = false;
            _value = EditorGUILayout.Vector3Field(info.Name, _value);
            if (GUI.changed)
            {
                ApplyProperty(property, _value);
                _target.PropertyChanged(_edittargetparent);
            }
            GUILayout.EndHorizontal();
        }

    }
    public class BoolComponent : InspectorComponent
    {
        private bool _value;
        public override void Init(EosObjectBase target, PropertyInfo property, EosObjectBase parent)
        {
            base.Init(target, property, parent);
            _value = (bool)property.GetValue(_edittarget);
        }
        public override void OnInspectorGUI(InspectorAttribute info, PropertyInfo property)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(info.Name);
            GUI.changed = false;
            _value = GUILayout.Toggle(_value, string.Empty);
            if (GUI.changed)
            {
                ApplyProperty(property, _value);
                _target.PropertyChanged(_edittargetparent);
            }
            GUILayout.EndHorizontal();
        }
    }
    public class StringComponent : InspectorComponent
    {
        private string _value;
        public override void Init(EosObjectBase target, PropertyInfo property, EosObjectBase parent)
        {
            base.Init(target, property, parent);
            _value = property.GetValue(_edittarget) as string;
        }
        public override void OnInspectorGUI(InspectorAttribute info, PropertyInfo property)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(info.Name);
            GUI.changed = false;
            _value = GUILayout.TextField(_value);
            if (GUI.changed)
            {
                ApplyProperty(property, _value);
                _target.PropertyChanged(_edittargetparent);
            }

            GUILayout.EndHorizontal();
        }
    }
    public class EnumComponent : InspectorComponent
    {
        private Enum _value;
        private Type _propertytype;
        public override void Init(EosObjectBase target, PropertyInfo property, EosObjectBase parent)
        {
            base.Init(target, property, parent);
            _propertytype = property.PropertyType;
            _value = (Enum)property.GetValue(_edittarget);
        }
        public override void OnInspectorGUI(InspectorAttribute info, PropertyInfo property)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(info.Name);
            GUI.changed = false;
            _value =  EditorGUILayout.EnumPopup(_value);
            if (GUI.changed)
            {
                ApplyProperty(property, _value);
                _target.PropertyChanged(_edittargetparent);
            }
            GUILayout.EndHorizontal();
        }
    }
    public class OreReferenceComponent : InspectorComponent
    {
        private Mold _mold;
        private bool _disabled = false;
        private OreReference _value;
        private int _oreindexinmold = 0;
        private string[] _namesinmold;
        public override void Init(EosObjectBase target, PropertyInfo property, EosObjectBase parent)
        {
            base.Init(target, property, parent);
            var moldrequire = property.GetCustomAttribute<RequireMoldAttribute>();
            if (moldrequire == null)
            {
                _disabled = true;
                return;
            }
            _mold = Mold.GetMold(moldrequire.MoldName);
            _mold.GetMoldNames();
            _disabled = false;
            _namesinmold = _mold.GetMoldNames();
            _value = (OreReference)property.GetValue(_edittarget);
            _oreindexinmold = Math.Max(0, _mold.GetIndexInMold(_value));
            _value.Mold = moldrequire.MoldName;
            _value.OreID = _mold.GetOreID(_oreindexinmold);
            property.SetValue(_target, _value);
            _target.PropertyChanged(_edittargetparent);
        }

        public override void OnInspectorGUI(InspectorAttribute info, PropertyInfo property)
        {
            if (_disabled)
                return;
            GUILayout.BeginHorizontal();
            GUILayout.Label(info.Name);
            GUI.changed = false;
            _oreindexinmold = EditorGUILayout.Popup(_oreindexinmold, _namesinmold);
            if (GUI.changed)
            {
                _value.OreID = _mold.GetOreID(_oreindexinmold);
                ApplyProperty(property, _value);
                //                property.SetValue(_target.Owner, _value);
                _target.PropertyChanged(_edittargetparent);
            }
            GUILayout.EndHorizontal();
        }
    }
    public class AddChildCompoent : InspectorComponent
    {
        private string _childname;
        private bool _childnamemodifyed;
        private string[] _typenames;
        private static int _currenttype;
        public override void Init(EosObjectBase target, PropertyInfo property, EosObjectBase parent)
        {
            base.Init(target, property, parent);
            _childname = SolutionEditorEditor.EosObjectNames[0].Replace("Eos", "");
            _childnamemodifyed = false;
        }
        public override void OnInspectorGUI(InspectorAttribute info, PropertyInfo property)
        {
            EditorGUILayout.Separator();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Name", GUILayout.Width(40));
            GUI.changed = false;
            _childname = GUILayout.TextField(_childname);
            if (GUI.changed) _childnamemodifyed = true;
            _typenames = _typenames ?? SolutionEditorEditor.EosObjectNames;
//            GUI.changed = false;
//            _currenttype = EditorGUILayout.Popup(_currenttype, _typenames);
//            if (GUI.changed)
//            {
//                if (!_childnamemodifyed)
//                    _childname = SolutionEditorEditor.EosObjectNames[_currenttype].Replace("Eos", "");
//            }
//            if (GUILayout.Button("+", GUILayout.Width(24)) && Selection.activeObject != null)
//            {
//                var child = ObjectFactory.CreateInstance(SolutionEditorEditor.EosObjectFullNames[_currenttype]) as EosObjectBase;
//                child.Name = _childname;
//                var editorobj = child.CreatedEditorImpl();
//                _target.AddChild(child);
////                Selection.activeObject = editorobj;
//            }
            GUILayout.EndHorizontal();
        }
    }

}