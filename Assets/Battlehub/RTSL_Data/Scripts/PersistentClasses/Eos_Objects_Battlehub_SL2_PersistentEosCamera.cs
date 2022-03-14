using System.Collections.Generic;
using ProtoBuf;
using Battlehub.RTSL;
using Eos.Objects;
using Eos.Objects.Battlehub.SL2;

using UnityObject = UnityEngine.Object;
namespace Eos.Objects.Battlehub.SL2
{
    [ProtoContract]
    public partial class PersistentEosCamera<TID> : PersistentEosObjectBase<TID>
    {
        
        public static implicit operator EosCamera(PersistentEosCamera<TID> surrogate)
        {
            if(surrogate == null) return default(EosCamera);
            return (EosCamera)surrogate.WriteTo(new EosCamera());
        }
        
        public static implicit operator PersistentEosCamera<TID>(EosCamera obj)
        {
            PersistentEosCamera<TID> surrogate = new PersistentEosCamera<TID>();
            surrogate.ReadFrom(obj);
            return surrogate;
        }
    }
}

