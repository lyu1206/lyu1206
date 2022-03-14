using System.Collections.Generic;
using ProtoBuf;
using Battlehub.RTSL;
using Eos.Objects;
using Eos.Objects.Battlehub.SL2;
using System;

using UnityObject = UnityEngine.Object;
namespace Eos.Objects.Battlehub.SL2
{
    [ProtoContract]
    public partial class PersistentEosObjectBase<TID> : PersistentSurrogate<TID>
    {
        [ProtoMember(257)]
        public List<PersistentEosObjectBase<TID>> _children;

        [ProtoMember(264)]
        public string Name;

        [ProtoMember(265)]
        public bool Active;

        protected override void ReadFromImpl(object obj)
        {
            base.ReadFromImpl(obj);
            EosObjectBase uo = (EosObjectBase)obj;
            _children = Assign(uo._children, v_ => (PersistentEosObjectBase<TID>)v_);
            Name = uo.Name;
            Active = uo.Active;
        }

        protected override object WriteToImpl(object obj)
        {
            obj = base.WriteToImpl(obj);
            EosObjectBase uo = (EosObjectBase)obj;
            uo._children = Assign(_children, v_ => (EosObjectBase)v_);
            uo.Name = Name;
            uo.Active = Active;
            return uo;
        }

        protected override void GetDepsImpl(GetDepsContext<TID> context)
        {
            base.GetDepsImpl(context);
            AddSurrogateDeps(_children, context);
        }

        protected override void GetDepsFromImpl(object obj, GetDepsFromContext context)
        {
            base.GetDepsFromImpl(obj, context);
            EosObjectBase uo = (EosObjectBase)obj;
            AddSurrogateDeps(uo._children, v_ => (PersistentEosObjectBase<TID>)v_, context);
        }

        public static implicit operator EosObjectBase(PersistentEosObjectBase<TID> surrogate)
        {
            if(surrogate == null) return default(EosObjectBase);
            return (EosObjectBase)surrogate.WriteTo(new EosObjectBase());
        }
        
        public static implicit operator PersistentEosObjectBase<TID>(EosObjectBase obj)
        {
            PersistentEosObjectBase<TID> surrogate = new PersistentEosObjectBase<TID>();
            surrogate.ReadFrom(obj);
            return surrogate;
        }
    }
}

