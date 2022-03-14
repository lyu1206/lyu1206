using System.Collections.Generic;
using ProtoBuf;
using Battlehub.RTSL;
using Eos.Objects;
using Eos.Objects.Battlehub.SL2;

using UnityObject = UnityEngine.Object;
namespace Eos.Objects.Battlehub.SL2
{
    [ProtoContract]
    public partial class PersistentEosGear<TID> : PersistentEosObjectBase<TID>
    {
        
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

