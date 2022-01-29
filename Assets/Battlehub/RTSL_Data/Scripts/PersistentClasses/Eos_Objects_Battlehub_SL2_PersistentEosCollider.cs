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
    public partial class PersistentEosCollider<TID> : PersistentSurrogate<TID>
    {
        [ProtoMember(257)]
        public ColliderType _colliderType;

        [ProtoMember(260)]
        public bool _istrigger;

        protected override void ReadFromImpl(object obj)
        {
            base.ReadFromImpl(obj);
            EosCollider uo = (EosCollider)obj;
            _colliderType = GetPrivate<EosCollider,ColliderType>(uo, "_colliderType");
            _istrigger = GetPrivate<EosCollider,bool>(uo, "_istrigger");
        }

        protected override object WriteToImpl(object obj)
        {
            obj = base.WriteToImpl(obj);
            EosCollider uo = (EosCollider)obj;
            SetPrivate(uo, "_colliderType", _colliderType);
            SetPrivate(uo, "_istrigger", _istrigger);
            return uo;
        }

        public static implicit operator EosCollider(PersistentEosCollider<TID> surrogate)
        {
            if(surrogate == null) return default(EosCollider);
            return (EosCollider)surrogate.WriteTo(new EosCollider());
        }
        
        public static implicit operator PersistentEosCollider<TID>(EosCollider obj)
        {
            PersistentEosCollider<TID> surrogate = new PersistentEosCollider<TID>();
            surrogate.ReadFrom(obj);
            return surrogate;
        }
    }
}

