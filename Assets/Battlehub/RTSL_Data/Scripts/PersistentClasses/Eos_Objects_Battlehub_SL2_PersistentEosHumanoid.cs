using System.Collections.Generic;
using ProtoBuf;
using Battlehub.RTSL;
using Eos.Objects;
using Eos.Objects.Battlehub.SL2;

using UnityObject = UnityEngine.Object;
namespace Eos.Objects.Battlehub.SL2
{
    [ProtoContract]
    public partial class PersistentEosHumanoid<TID> : PersistentSurrogate<TID>
    {
        
        public static implicit operator EosHumanoid(PersistentEosHumanoid<TID> surrogate)
        {
            if(surrogate == null) return default(EosHumanoid);
            return (EosHumanoid)surrogate.WriteTo(new EosHumanoid());
        }
        
        public static implicit operator PersistentEosHumanoid<TID>(EosHumanoid obj)
        {
            PersistentEosHumanoid<TID> surrogate = new PersistentEosHumanoid<TID>();
            surrogate.ReadFrom(obj);
            return surrogate;
        }
    }
}

