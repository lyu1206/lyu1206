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
    public partial class PersistentPlayer<TID> : PersistentEosObjectBase<TID>
    {
        
        public static implicit operator Player(PersistentPlayer<TID> surrogate)
        {
            if(surrogate == null) return default(Player);
            return (Player)surrogate.WriteTo(new Player());
        }
        
        public static implicit operator PersistentPlayer<TID>(Player obj)
        {
            PersistentPlayer<TID> surrogate = new PersistentPlayer<TID>();
            surrogate.ReadFrom(obj);
            return surrogate;
        }
    }
}

