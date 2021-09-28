#if UNITY_EDITOR
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.IMGUI.Controls;

namespace Eos.Objects.Editor
{
    public class ObjectTreeviewItem : TreeViewItem
    {
        private EosObjectBase _owner;
        public ObjectTreeviewItem(EosObjectBase obj)
        {
            _owner = obj;
            var idepth = -1;
            var parent = obj.Parent;
            while (parent != null)
            {
                parent = parent.Parent;
                idepth++;
            }
            depth = idepth;
        }
        public override string displayName { get => _owner.Name ; set => base.displayName = value; }
        public override int id { get => (int)_owner.ObjectID ; set => base.id = value; }
    }
}
namespace Eos.Objects
{
    using Editor;
    using Battlehub.RTEditor;
    using Battlehub.RTCommon;
    using Battlehub.RTHandles;
    public partial class EosObjectBase
    {
        public virtual EosEditorObject CreatedEditorImpl()
        {
            return EosEditorObject.Create(this);
        }
        public virtual void PropertyChanged(EosObjectBase parent)
        {
        }
        public void AddChildEditorImpl(EosObjectBase obj)
        {
            obj._parent = this;
            _childrens.Add(obj);
        }
        public void ClearRelationa()
        {
            _parent = null;
            _childrens.Clear();
        }
        public virtual void CreatedEditor(EosEditorObject obj)
        {

        }
        public EosEditorObject CreateForEditor(EosEditorObject parent)
        {
            var ret = EosEditorObject.Create(this);
            if (parent != null)
            {
                _parent = parent.Owner;
                OnAncestryChanged();
                PropertyChanged(_parent);
                parent.AddChild(ret);
            }
            foreach (var child in _childrens)
                child?.CreateForEditor(ret);
            return ret;
        }
        public virtual void CreatedOnEditor()
        {

        }
        protected ExposeToEditor _rteditObject;
        public virtual void RTEOnCreated(IRTE editor)
        {
            var go = new GameObject(Name);
            _rteditObject = go.AddComponent<ExposeToEditor>();
            if (Parent != null && Parent._rteditObject!=null)
                _rteditObject.transform.SetParent(Parent._rteditObject.transform);
        }
    }
    public partial class EosModel
    {
        public override void RTEOnCreated(IRTE editor)
        {
            base.RTEOnCreated(editor);
            _transform.Transform.gameObject.AddComponent<ExposeToEditor>();
            editor.RegisterCreatedObjects(new[] { _transform.Transform.gameObject });
        }
    }
}
#endif