using System.Collections.Generic;
using ProtoBuf;
using Battlehub.RTSL;
using UnityEngine;
using UnityEngine.Battlehub.SL2;

using UnityObject = UnityEngine.Object;
namespace UnityEngine.Battlehub.SL2
{
    [ProtoContract]
    public partial class PersistentAnimationEvent<TID> : PersistentSurrogate<TID>
    {
        
        public static implicit operator AnimationEvent(PersistentAnimationEvent<TID> surrogate)
        {
            if(surrogate == null) return default(AnimationEvent);
            return (AnimationEvent)surrogate.WriteTo(new AnimationEvent());
        }
        
        public static implicit operator PersistentAnimationEvent<TID>(AnimationEvent obj)
        {
            PersistentAnimationEvent<TID> surrogate = new PersistentAnimationEvent<TID>();
            surrogate.ReadFrom(obj);
            return surrogate;
        }
    }
}

