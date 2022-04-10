using System.Collections.Generic;
using ProtoBuf;
using Battlehub.RTSL;
using UnityEditorInternal;
using UnityEditorInternal.Battlehub.SL2;

using UnityObject = UnityEngine.Object;
namespace UnityEditorInternal.Battlehub.SL2
{
    [ProtoContract]
    public partial class PersistentAnimatorControllerLayer<TID> : PersistentSurrogate<TID>
    {
        
        public static implicit operator UnityEditor.Animations.AnimatorControllerLayer(PersistentAnimatorControllerLayer<TID> surrogate)
        {
            if(surrogate == null) return default(UnityEditor.Animations.AnimatorControllerLayer);
            return (UnityEditor.Animations.AnimatorControllerLayer)surrogate.WriteTo(new UnityEditor.Animations.AnimatorControllerLayer());
        }
        
        public static implicit operator PersistentAnimatorControllerLayer<TID>(UnityEditor.Animations.AnimatorControllerLayer obj)
        {
            PersistentAnimatorControllerLayer<TID> surrogate = new PersistentAnimatorControllerLayer<TID>();
            surrogate.ReadFrom(obj);
            return surrogate;
        }
    }
}

