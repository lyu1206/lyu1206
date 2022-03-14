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
    public partial class PersistentEosBone<TID> : PersistentEosObjectBase<TID>
    {
        [ProtoMember(256)]
        public TID Bone;

        protected override void ReadFromImpl(object obj)
        {
            base.ReadFromImpl(obj);
            EosBone uo = (EosBone)obj;
            Bone = ToID(uo.Bone);
        }

        protected override object WriteToImpl(object obj)
        {
            obj = base.WriteToImpl(obj);
            EosBone uo = (EosBone)obj;
            uo.Bone = FromID(Bone, uo.Bone);
            return uo;
        }

        protected override void GetDepsImpl(GetDepsContext<TID> context)
        {
            base.GetDepsImpl(context);
            AddDep(Bone, context);
        }

        protected override void GetDepsFromImpl(object obj, GetDepsFromContext context)
        {
            base.GetDepsFromImpl(obj, context);
            EosBone uo = (EosBone)obj;
            AddDep(uo.Bone, context);
        }

        public static implicit operator EosBone(PersistentEosBone<TID> surrogate)
        {
            if(surrogate == null) return default(EosBone);
            return (EosBone)surrogate.WriteTo(new EosBone());
        }
        
        public static implicit operator PersistentEosBone<TID>(EosBone obj)
        {
            PersistentEosBone<TID> surrogate = new PersistentEosBone<TID>();
            surrogate.ReadFrom(obj);
            return surrogate;
        }
    }
}

