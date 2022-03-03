using System.Collections.Generic;
using ProtoBuf;
using Battlehub.RTSL;
using Eos.Objects;
using Eos.Objects.Battlehub.SL2;

using UnityObject = UnityEngine.Object;
namespace Eos.Objects.Battlehub.SL2
{
    [ProtoContract]
    public partial class PersistentEosPawnActor<TID> : PersistentSurrogate<TID>
    {
        
        public static implicit operator EosPawnActor(PersistentEosPawnActor<TID> surrogate)
        {
            if(surrogate == null) return default(EosPawnActor);
            return (EosPawnActor)surrogate.WriteTo(new EosPawnActor());
        }
        
        public static implicit operator PersistentEosPawnActor<TID>(EosPawnActor obj)
        {
            PersistentEosPawnActor<TID> surrogate = new PersistentEosPawnActor<TID>();
            surrogate.ReadFrom(obj);
            return surrogate;
        }
    }
}

