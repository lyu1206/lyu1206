using System.Collections.Generic;
using ProtoBuf;
using Battlehub.RTSL;
using UnityEngine;
using UnityEngine.Battlehub.SL2;

using UnityObject = UnityEngine.Object;
namespace UnityEngine.Battlehub.SL2
{
    [ProtoContract]
    public partial class PersistentAnimatorControllerParameter<TID> : PersistentSurrogate<TID>
    {
        
        public static implicit operator AnimatorControllerParameter(PersistentAnimatorControllerParameter<TID> surrogate)
        {
            if(surrogate == null) return default(AnimatorControllerParameter);
            return (AnimatorControllerParameter)surrogate.WriteTo(new AnimatorControllerParameter());
        }
        
        public static implicit operator PersistentAnimatorControllerParameter<TID>(AnimatorControllerParameter obj)
        {
            PersistentAnimatorControllerParameter<TID> surrogate = new PersistentAnimatorControllerParameter<TID>();
            surrogate.ReadFrom(obj);
            return surrogate;
        }
    }
}

