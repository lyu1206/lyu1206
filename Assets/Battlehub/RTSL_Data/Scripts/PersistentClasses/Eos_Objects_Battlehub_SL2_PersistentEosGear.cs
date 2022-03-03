using System.Collections.Generic;
using ProtoBuf;
using Battlehub.RTSL;
using Eos.Objects;
using Eos.Objects.Battlehub.SL2;
using UnityEngine;

using UnityObject = UnityEngine.Object;
namespace Eos.Objects.Battlehub.SL2
{
    [ProtoContract]
    public partial class PersistentEosGear<TID> : PersistentSurrogate<TID>
    {
        [ProtoMember(256)]
        public TID Part;

        protected override void ReadFromImpl(object obj)
        {
            base.ReadFromImpl(obj);
            EosGear uo = (EosGear)obj;
            Part = ToID(uo.Part);
        }

        protected override object WriteToImpl(object obj)
        {
            obj = base.WriteToImpl(obj);
            EosGear uo = (EosGear)obj;
            uo.Part = FromID(Part, uo.Part);
            return uo;
        }

        protected override void GetDepsImpl(GetDepsContext<TID> context)
        {
            base.GetDepsImpl(context);
            AddDep(Part, context);
        }

        protected override void GetDepsFromImpl(object obj, GetDepsFromContext context)
        {
            base.GetDepsFromImpl(obj, context);
            EosGear uo = (EosGear)obj;
            AddDep(uo.Part, context);
        }

        public static implicit operator EosGear(PersistentEosGear<TID> surrogate)
        {
            if(surrogate == null) return default(EosGear);
            return (EosGear)surrogate.WriteTo(new EosGear());
        }
        
        public static implicit operator PersistentEosGear<TID>(EosGear obj)
        {
            PersistentEosGear<TID> surrogate = new PersistentEosGear<TID>();
            surrogate.ReadFrom(obj);
            return surrogate;
        }
    }
}

