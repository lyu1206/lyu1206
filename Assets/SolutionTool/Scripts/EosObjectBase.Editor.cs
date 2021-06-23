#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Eos.Objects
{
    using Editor;
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
    }
}
#endif