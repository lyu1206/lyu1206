#if UNITY_EDITOR
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.IMGUI.Controls;

namespace Eos.Service
{
    using Battlehub.RTCommon;
    public partial class GUIService
    {
        public override Transform EditorTrasnform => _guiroot.Transform;
        public override void RTEOnCreated(Battlehub.RTCommon.IRTE editor)
        {
            _guiroot.AddComponent<ExposeToEditor>();
        }
    }
}
namespace Eos.Objects.Editor
{
    using Service;
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
    using Service;
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
            _children.Add(obj);
        }
        public void ClearRelationa()
        {
            _parent = null;
            _children.Clear();
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
            foreach (var child in _children)
                child?.CreateForEditor(ret);
            return ret;
        }
        public virtual void CreatedOnEditor()
        {
            OnAncestryChanged();
            foreach (var child in _children)
                child?.CreatedOnEditor();
        }
        protected ExposeToEditor _rteditObject;
        [MessagePack.IgnoreMember]
        public ExposeToEditor RTEditObject => _rteditObject;
        [MessagePack.IgnoreMember]
        public virtual Transform EditorTrasnform
        {
            get
            {
                if (this is ITransform trans)
                {
                    return trans.Transform.Transform;
                }
                else if (Parent == null)
                    return null;
                else if (_rteditObject != null)
                    return _rteditObject.transform;
                var go = new GameObject(Name);
                return go.transform;
            }
        }
        public virtual void RTEOnCreated(IRTE editor)
        {
            var trans = EditorTrasnform;
            var rteditorobject = trans.gameObject.AddComponent<ExposeToEosEditor>();
            rteditorobject.Owner = this;
            _rteditObject = rteditorobject;
            //var inspect = trans.gameObject.AddComponent<EosObjectInspector>();
            //if (this is EosTransformActor)
            //    editor.RegisterCreatedObjects(new[] { _rteditObject.gameObject }, true);
            if (Parent != null)
            {
                _rteditObject.transform.SetParent(Parent.EditorTrasnform);
            }
            if (this is EosService)
                _rteditObject.CanTransform = false;
        }
    }
    public partial class EosModel
    {
        public override void RTEOnCreated(IRTE editor)
        {
            base.RTEOnCreated(editor);
            _transform.LocalPosition = Vector3.zero;
            _transform.Transform.localRotation = Quaternion.identity;
            _transform.LocalScale = Vector3.one;
        }
    }
    public partial class EosLight
    {
        public override void CreatedOnEditor()
        {
            base.CreatedOnEditor();
            BuildLight();
        }
    }
    public partial class EosMeshObject
    {
        public override void RTEOnCreated(IRTE editor)
        {
            base.RTEOnCreated(editor);
        }
    }


}
#endif