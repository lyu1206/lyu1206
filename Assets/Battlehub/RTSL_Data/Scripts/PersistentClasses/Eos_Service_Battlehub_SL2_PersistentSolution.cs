using System.Collections.Generic;
using ProtoBuf;
using Battlehub.RTSL;
using Eos.Service;
using Eos.Service.Battlehub.SL2;
using Eos.Objects.Battlehub.SL2;

using UnityObject = UnityEngine.Object;
namespace Eos.Service.Battlehub.SL2
{
    [ProtoContract]
    public partial class PersistentSolution<TID> : PersistentEosObjectBase<TID>
    {
        
        public static implicit operator Solution(PersistentSolution<TID> surrogate)
        {
            if(surrogate == null) return default(Solution);
            return (Solution)surrogate.WriteTo(new Solution());
        }
        
        public static implicit operator PersistentSolution<TID>(Solution obj)
        {
            PersistentSolution<TID> surrogate = new PersistentSolution<TID>();
            surrogate.ReadFrom(obj);
            return surrogate;
        }
    }
}

