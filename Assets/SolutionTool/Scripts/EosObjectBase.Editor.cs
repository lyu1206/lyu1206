#if UNITY_EDITOR
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.IMGUI.Controls;

namespace Eos.Service
{
    using Objects;
    using Battlehub.RTCommon;
    using Battlehub.RTEditor;
    public partial class GUIService
    {
        public override EosTransform EditorTrasnform => _guiroot;
        public override void RTEOnCreated(Battlehub.RTCommon.IRTE editor)
        {
            base.RTEOnCreated(editor);
            CreateCanvas();
            var camera =  UnityEngine.Object.FindObjectOfType<GameViewCamera>();
            _canvas.renderMode = RenderMode.ScreenSpaceCamera;
            _canvas.planeDistance = 1;
            _canvas.worldCamera = camera.GetComponent<Camera>();
        }
        public override void SetExposeToEditor(ExposeToEosEditor editorobject)
        {
            _guiroot.Transform = editorobject.transform;
            var camera = UnityEngine.Object.FindObjectOfType<GameViewCamera>();
            CreateCanvas();
            _canvas.renderMode = RenderMode.ScreenSpaceCamera;
            //_canvas.planeDistance = 1;
            //_canvas.worldCamera = cam;
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
        protected EosTransform _rtTransform;
        [MessagePack.IgnoreMember]
        public ExposeToEditor RTEditObject => _rteditObject;
        [MessagePack.IgnoreMember]
        public virtual EosTransform EditorTrasnform
        {
            get
            {
                if (this is ITransform trans)
                {
                    return trans.Transform;
                }
                else if (this is Eos.Service.Solution)
                    return null;
                else if (_rtTransform != null)
                    return _rtTransform;
                _rtTransform = ObjectFactory.CreateInstance<EosTransform>();
                var go = new GameObject(Name);
                _rtTransform.Transform = go.transform;
                return _rtTransform;
            }
        }
        public virtual void SetExposeToEditor(ExposeToEosEditor editorobject)
        {
            if (this is Eos.Service.Solution)
                return;
            _rtTransform = ObjectFactory.CreateInstance<EosTransform>();
            _rtTransform.Transform = editorobject.transform;
            _rteditObject = editorobject;
        }
        public virtual EosObjectBase CreateCloneObjectForEditor(ExposeToEosEditor editorobject)
        {
            var copy = ObjectFactory.CreateInstance(this.GetType());
            copy.Name = Name;
            copy.ObjectID = ObjectID;
            copy.SetExposeToEditor(editorobject);
            editorobject.Owner = copy;
            OnCopyTo(copy);
            Ref.ObjectManager.RegistObject(copy);
            var parent = Ref.ObjectManager[_parent.ObjectID];
            parent?.AddChild(copy);
            return copy;
        }
        public virtual void RTEOnCreated(IRTE editor)
        {
            var trans = EditorTrasnform;
            var rteditorobject = EditorTrasnform.AddComponent<ExposeToEosEditor>();
            rteditorobject.Owner = this;
            _rteditObject = rteditorobject;
            //var inspect = trans.gameObject.AddComponent<EosObjectInspector>();
            //if (this is EosTransformActor)
            //    editor.RegisterCreatedObjects(new[] { _rteditObject.gameObject }, true);
            if (Parent != null && Parent.EditorTrasnform!=null)
            {
                _rteditObject.transform.SetParent(Parent.EditorTrasnform.Transform);
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
    public partial class EosCollider
    {
        public override void SetExposeToEditor(ExposeToEosEditor editorobject)
        {
            _transform.Transform = editorobject.transform;
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