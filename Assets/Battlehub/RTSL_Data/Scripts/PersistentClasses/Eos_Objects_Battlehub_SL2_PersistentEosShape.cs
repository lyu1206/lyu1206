using System.Collections.Generic;
using ProtoBuf;
using Battlehub.RTSL;
using Eos.Objects;
using Eos.Objects.Battlehub.SL2;
using UnityEngine;
using UnityEngine.Battlehub.SL2;

using UnityObject = UnityEngine.Object;
namespace Eos.Objects.Battlehub.SL2
{
    [ProtoContract]
    public partial class PersistentEosShape<TID> : PersistentSurrogate<TID>
    {
        [ProtoMember(256)]
        public PrimitiveType Type;

        protected override void ReadFromImpl(object obj)
        {
            base.ReadFromImpl(obj);
            EosShape uo = (EosShape)obj;
            Type = uo.Type;
        }

        protected override object WriteToImpl(object obj)
        {
            obj = base.WriteToImpl(obj);
            EosShape uo = (EosShape)obj;
            uo.Type = Type;
            return uo;
        }

        public static implicit operator EosShape(PersistentEosShape<TID> surrogate)
        {
            if(surrogate == null) return default(EosShape);
            return (EosShape)surrogate.WriteTo(new EosShape());
        }
        
        public static implicit operator PersistentEosShape<TID>(EosShape obj)
        {
            PersistentEosShape<TID> surrogate = new PersistentEosShape<TID>();
            surrogate.ReadFrom(obj);
            return surrogate;
        }
    }
}

