using System.Collections.Generic;
using ProtoBuf;
using Battlehub.RTSL;
using UnityEngine;
using UnityEngine.Battlehub.SL2;
using System;

using UnityObject = UnityEngine.Object;
namespace UnityEngine.Battlehub.SL2
{
    [ProtoContract]
    public partial class PersistentAnimationClip<TID> : PersistentObject<TID>
    {
        [ProtoMember(256)]
        public float frameRate;

        [ProtoMember(257)]
        public WrapMode wrapMode;

        [ProtoMember(259)]
        public bool legacy;

        [ProtoMember(260)]
        public PersistentAnimationEvent<TID>[] events;

        protected override void ReadFromImpl(object obj)
        {
            base.ReadFromImpl(obj);
            AnimationClip uo = (AnimationClip)obj;
            frameRate = uo.frameRate;
            wrapMode = uo.wrapMode;
            legacy = uo.legacy;
            events = Assign(uo.events, v_ => (PersistentAnimationEvent<TID>)v_);
        }

        protected override object WriteToImpl(object obj)
        {
            obj = base.WriteToImpl(obj);
            AnimationClip uo = (AnimationClip)obj;
            uo.frameRate = frameRate;
            uo.wrapMode = wrapMode;
            uo.legacy = legacy;
            uo.events = Assign(events, v_ => (AnimationEvent)v_);
            return uo;
        }
    }
}

