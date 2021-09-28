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
            OnAncestryChanged();
            foreach (var child in _childrens)
                child?.CreatedOnEditor();
        }
        protected ExposeToEditor _rteditObject;
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
            _rteditObject = trans.gameObject.AddComponent<ExposeToEditor>();
            editor.RegisterCreatedObjects(new[] { _rteditObject.gameObject }, true);
            if (Parent != null)
            {
                _rteditObject.transform.SetParent(Parent.EditorTrasnform);
                //_rteditObject.transform.localPosition = Vector3.zero;
                //_rteditObject.transform.localRotation = Quaternion.identity;
                //_rteditObject.transform.localScale = Vector3.one;
            }

            /*
            if (this is ITransform transform)
            {
//                _rteditObject = transform.Transform.AddComponent<ExposeToEditor>();
                if (Parent != null)
                {
                    trans.SetParent(Parent.EditorTrasnform);
                    trans.localPosition = Vector3.zero;
                    trans.localRotation = Quaternion.identity;
                    trans.localScale = Vector3.one;
                }
            }
            else
            {
                //var go = new GameObject(Name);
                //_rteditObject = go.AddComponent<ExposeToEditor>();
                if (Parent != null)
                {
                    //if (Parent is ITransform parenttrans)
                    //{
                    //    _rteditObject.transform.SetParent(parenttrans.Transform.Transform);
                    //}
                    //else
                    {
                        _rteditObject.transform.SetParent(Parent.EditorTrasnform);
                    }
                }
                _rteditObject.transform.localPosition = Vector3.zero;
                _rteditObject.transform.localRotation = Quaternion.identity;
                _rteditObject.transform.localScale = Vector3.one;
                editor.RegisterCreatedObjects(new[] { _rteditObject.gameObject }, true);
            }
            */
            if (this is EosService)
                _rteditObject.CanTransform = false;
        }
    }
    public partial class EosModel
    {
        public override void RTEOnCreated(IRTE editor)
        {
            //base.RTEOnCreated(editor);
            var ed = _transform.Transform.gameObject.AddComponent<ExposeToEditor>();
            ed.CanTransform = false;
            editor.RegisterCreatedObjects(new[] { _transform.Transform.gameObject },false);
            if (Parent != null)
            {
                if (Parent is ITransform parenttrans)
                {
                    _transform.SetParent(parenttrans.Transform);
                }
                else
                {
                    _transform.Transform.SetParent(Parent.EditorTrasnform);
                }
            }
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