using System.Collections.Generic;
using ProtoBuf;
using Battlehub.RTSL;
using Eos.Objects;
using Eos.Objects.Battlehub.SL2;

using UnityObject = UnityEngine.Object;
namespace Eos.Objects.Battlehub.SL2
{
    [ProtoContract]
    public partial class PersistentEosScript<TID> : PersistentSurrogate<TID>
    {
        
        public static implicit operator EosScript(PersistentEosScript<TID> surrogate)
        {
            if(surrogate == null) return default(EosScript);
            return (EosScript)surrogate.WriteTo(new EosScript());
        }
        
        public static implicit operator PersistentEosScript<TID>(EosScript obj)
        {
            PersistentEosScript<TID> surrogate = new PersistentEosScript<TID>();
            surrogate.ReadFrom(obj);
            return surrogate;
        }
    }
}

