using System;
using System.Collections.Generic;

namespace Eos.Objects
{
    using MessagePack;

    [Serializable]
    [MessagePackObject]
    [Union(0,typeof(EosCamera))]
    public class EosObjectBase 
    {
        [Key(1)]public string Name;
        [IgnoreMember]
        protected EosObjectBase _parent;
        protected List<EosObjectBase> _childrens = new List<EosObjectBase>();
        public void AddChild(EosObjectBase obj)
        {
            if (obj._parent !=null)
                obj._parent.RemoveChild(obj);
            obj._parent = this;
            _childrens.Add(obj);
        }
        public void RemoveChild(EosObjectBase obj)
        {
            _childrens.Remove(obj);
        }
    }
}
