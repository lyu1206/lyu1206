using System.Collections.Generic;
using ProtoBuf;
using Battlehub.RTSL;
using Eos.Objects;
using Eos.Objects.Battlehub.SL2;

using UnityObject = UnityEngine.Object;
namespace Eos.Objects.Battlehub.SL2
{
    [ProtoContract]
    public partial class PersistentColliderType<TID> : PersistentSurrogate<TID>
    {
        
        public static implicit operator ColliderType(PersistentColliderType<TID> surrogate)
        {
            if(surrogate == null) return default(ColliderType);
            return (ColliderType)surrogate.WriteTo(new ColliderType());
        }
        
        public static implicit operator PersistentColliderType<TID>(ColliderType obj)
        {
            PersistentColliderType<TID> surrogate = new PersistentColliderType<TID>();
            surrogate.ReadFrom(obj);
            return surrogate;
        }
    }
}

