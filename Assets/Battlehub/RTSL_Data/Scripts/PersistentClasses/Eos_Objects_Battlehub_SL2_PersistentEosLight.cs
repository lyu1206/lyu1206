using System.Collections.Generic;
using ProtoBuf;
using Battlehub.RTSL;
using Eos.Objects;
using Eos.Objects.Battlehub.SL2;

using UnityObject = UnityEngine.Object;
namespace Eos.Objects.Battlehub.SL2
{
    [ProtoContract]
    public partial class PersistentEosLight<TID> : PersistentSurrogate<TID>
    {
        
        public static implicit operator EosLight(PersistentEosLight<TID> surrogate)
        {
            if(surrogate == null) return default(EosLight);
            return (EosLight)surrogate.WriteTo(new EosLight());
        }
        
        public static implicit operator PersistentEosLight<TID>(EosLight obj)
        {
            PersistentEosLight<TID> surrogate = new PersistentEosLight<TID>();
            surrogate.ReadFrom(obj);
            return surrogate;
        }
    }
}

