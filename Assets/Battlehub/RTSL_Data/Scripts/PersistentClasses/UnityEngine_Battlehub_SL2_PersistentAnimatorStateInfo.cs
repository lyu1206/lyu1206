using System.Collections.Generic;
using ProtoBuf;
using Battlehub.RTSL;
using UnityEngine;
using UnityEngine.Battlehub.SL2;

using UnityObject = UnityEngine.Object;
namespace UnityEngine.Battlehub.SL2
{
    [ProtoContract]
    public partial class PersistentAnimatorStateInfo<TID> : PersistentSurrogate<TID>
    {
        
        public static implicit operator AnimatorStateInfo(PersistentAnimatorStateInfo<TID> surrogate)
        {
            if(surrogate == null) return default(AnimatorStateInfo);
            return (AnimatorStateInfo)surrogate.WriteTo(new AnimatorStateInfo());
        }
        
        public static implicit operator PersistentAnimatorStateInfo<TID>(AnimatorStateInfo obj)
        {
            PersistentAnimatorStateInfo<TID> surrogate = new PersistentAnimatorStateInfo<TID>();
            surrogate.ReadFrom(obj);
            return surrogate;
        }
    }
}

