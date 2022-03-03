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
    public partial class PersistentEosTransform<TID> : PersistentSurrogate<TID>
    {
        [ProtoMember(262)]
        public PersistentVector3<TID> LocalPosition;

        [ProtoMember(263)]
        public PersistentVector3<TID> LocalRotation;

        [ProtoMember(264)]
        public PersistentVector3<TID> LocalScale;

        protected override void ReadFromImpl(object obj)
        {
            base.ReadFromImpl(obj);
            EosTransform uo = (EosTransform)obj;
            LocalPosition = uo.LocalPosition;
            LocalRotation = uo.LocalRotation;
            LocalScale = uo.LocalScale;
        }

        protected override object WriteToImpl(object obj)
        {
            obj = base.WriteToImpl(obj);
            EosTransform uo = (EosTransform)obj;
            uo.LocalPosition = LocalPosition;
            uo.LocalRotation = LocalRotation;
            uo.LocalScale = LocalScale;
            return uo;
        }

        public static implicit operator EosTransform(PersistentEosTransform<TID> surrogate)
        {
            if(surrogate == null) return default(EosTransform);
            return (EosTransform)surrogate.WriteTo(new EosTransform());
        }
        
        public static implicit operator PersistentEosTransform<TID>(EosTransform obj)
        {
            PersistentEosTransform<TID> surrogate = new PersistentEosTransform<TID>();
            surrogate.ReadFrom(obj);
            return surrogate;
        }
    }
}

