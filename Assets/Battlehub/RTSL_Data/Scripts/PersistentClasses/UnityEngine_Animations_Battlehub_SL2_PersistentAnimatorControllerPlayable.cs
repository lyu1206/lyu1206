using System.Collections.Generic;
using ProtoBuf;
using Battlehub.RTSL;
using UnityEngine.Animations;
using UnityEngine.Animations.Battlehub.SL2;

using UnityObject = UnityEngine.Object;
namespace UnityEngine.Animations.Battlehub.SL2
{
    [ProtoContract]
    public partial class PersistentAnimatorControllerPlayable<TID> : PersistentSurrogate<TID>
    {
        
        public static implicit operator AnimatorControllerPlayable(PersistentAnimatorControllerPlayable<TID> surrogate)
        {
            if(surrogate == null) return default(AnimatorControllerPlayable);
            return (AnimatorControllerPlayable)surrogate.WriteTo(new AnimatorControllerPlayable());
        }
        
        public static implicit operator PersistentAnimatorControllerPlayable<TID>(AnimatorControllerPlayable obj)
        {
            PersistentAnimatorControllerPlayable<TID> surrogate = new PersistentAnimatorControllerPlayable<TID>();
            surrogate.ReadFrom(obj);
            return surrogate;
        }
    }
}

