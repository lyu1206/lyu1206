#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using Ludiq;
using UnityEngine;
using UnityEngine.UI;
namespace Eos.Objects.Editor
{
    [ExecuteInEditMode]
    public class EosEditorObject : MonoBehaviour
    {
        private void Reset() { Ensure_EditorTag(); }
        private void OnEnable() { Ensure_EditorTag(); }
        private void Awake() { Ensure_EditorTag(); }
        [SerializeField]
        public EosObjectBase _owner;
        private void Ensure_EditorTag()
        {
            if (gameObject.tag != "EditorOnly")
            {
                UnityEditor.EditorUtility.SetDirty(gameObject);//applies changes, preventing unity-deserializing old data when we exit gameplay mode, etc
            }
            //a special tag, that will ensure that an object won't be included in build
            gameObject.tag = "EditorOnly";
        }
        public EosObjectBase Owner => _owner;
        public static EosEditorObject Create(EosObjectBase ownerobj)
        {
            var obj = new GameObject(ownerobj.Name);
            var eosobj = obj.AddComponent<EosEditorObject>();
            eosobj._owner = ownerobj;
            return eosobj;
        }
        public void AddChild(EosEditorObject obj)
        {
            obj.transform.parent = transform;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
        }
        public void ApplyObject(EosEditorObject parent)
        {
            _owner.ClearRelationa();
            if (parent != null)
                parent._owner.AddChildEditorImpl(_owner);
            for (int i=0;i<transform.childCount;i++)
            {
                var child = transform.GetChild(i).GetComponent<EosEditorObject>();
                if (child == null)
                    continue;
                child.ApplyObject(this);
            }
        }
        private void Update()
        {
            if (transform.hasChanged)
            {
                Debug.Log("transform changed");
                if (_owner is EosTransformActor transactor)
                {
                    transactor.LocalPosition = transform.localPosition;
                    transactor.LocalRotation = transform.localRotation.eulerAngles;
                }
                transform.hasChanged = false;
            }
        }
        //public static List<GameObject> IgnoreSaveObjects { get; set; } = new List<GameObject>();
        //public EosObjectBase Object
        //{
        //    get => Inspector;
        //    set => Inspector = value;
        //}

        //public virtual EosObjectBase Inspector { set; get; }

        //public EosObject AddChild<T>() where T : EosObjectBase
        //{
        //    return AddChild(typeof(T));
        //}

        //public EosObject AddChild(Type type)
        //{
        //    var eosobject = SolutionEditor.CreateObject(type, this);
        //    eosobject.SetParent(this);
        //    return eosobject;
        //}

        //public void SetParent(EosObject parent)
        //{
        //    transform.SetParent(parent.transform, true);
        //    Object.SetParent(parent.Object);
        //}
        //private EosObject CreateUI(Transform uiobj)
        //{
        //    var component = uiobj.GetComponent<Graphic>();
        //    EosObject retobj = null;
        //    if (component is Image)
        //    {
        //        retobj = uiobj.AddComponent<EosUISprite_Inspector>();
        //        retobj.Object = new EosUISprite();
        //        return retobj;
        //    }
        //    else if (component is Text)
        //    {
        //        retobj = uiobj.AddComponent<EosObject>();
        //        retobj.Object = new EosUIText();
        //        return retobj;
        //    }
        //    return null;
        //}

        //public void Iterchild(Action<EosObject> iter)
        //{
        //    for (int i = 0; i < transform.childCount; i++)
        //    {
        //        var child = transform.GetChild(i);
        //        if (IgnoreSaveObjects.Contains(child.gameObject))
        //            continue;
        //        var obj = child.GetComponent<EosObject>();
        //        if (obj == null || obj.Object == null)
        //            obj = CreateUI(child);
        //        if (obj == null || obj.Object == null)
        //            continue;
        //        var eosobject = obj.Object;
        //        eosobject.SetParent(Object);
        //        iter?.Invoke(obj);
        //        obj.Iterchild(iter);
        //    }
        //}
    }
}
#endif