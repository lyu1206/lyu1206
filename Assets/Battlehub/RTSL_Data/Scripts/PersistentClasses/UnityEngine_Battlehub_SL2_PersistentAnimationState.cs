using System.Collections.Generic;
using ProtoBuf;
using Battlehub.RTSL;
using UnityEngine;
using UnityEngine.Battlehub.SL2;

using UnityObject = UnityEngine.Object;
namespace UnityEngine.Battlehub.SL2
{
    [ProtoContract]
    public partial class PersistentAnimationState<TID> : PersistentSurrogate<TID>
    {
        
        public static implicit operator AnimationState(PersistentAnimationState<TID> surrogate)
        {
            if(surrogate == null) return default(AnimationState);
            return (AnimationState)surrogate.WriteTo(new AnimationState());
        }
        
        public static implicit operator PersistentAnimationState<TID>(AnimationState obj)
        {
            PersistentAnimationState<TID> surrogate = new PersistentAnimationState<TID>();
            surrogate.ReadFrom(obj);
            return surrogate;
        }
    }
}

