using System.Collections.Generic;
using ProtoBuf;
using Battlehub.RTSL;
using Eos.Service;
using Eos.Service.Battlehub.SL2;

using UnityObject = UnityEngine.Object;
namespace Eos.Service.Battlehub.SL2
{
    [ProtoContract]
    public partial class PersistentWorkspace<TID> : PersistentSurrogate<TID>
    {
        
        public static implicit operator Workspace(PersistentWorkspace<TID> surrogate)
        {
            if(surrogate == null) return default(Workspace);
            return (Workspace)surrogate.WriteTo(new Workspace());
        }
        
        public static implicit operator PersistentWorkspace<TID>(Workspace obj)
        {
            PersistentWorkspace<TID> surrogate = new PersistentWorkspace<TID>();
            surrogate.ReadFrom(obj);
            return surrogate;
        }
    }
}

