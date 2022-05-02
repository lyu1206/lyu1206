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
    public partial class PersistentNodeChannel<TID> : PersistentObject<TID>
    {
        [ProtoMember(256)]
        public string path;

        [ProtoMember(257)]
        public string propertyname;

        [ProtoMember(258)]
        public PersistentKeyframe<TID>[] keys;

        protected override void ReadFromImpl(object obj)
        {
            base.ReadFromImpl(obj);
            NodeChannel uo = (NodeChannel)obj;
            path = uo.path;
            propertyname = uo.propertyname;
            keys = Assign(uo.keys, v_ => (PersistentKeyframe<TID>)v_);
        }

        protected override object WriteToImpl(object obj)
        {
            obj = base.WriteToImpl(obj);
            NodeChannel uo = (NodeChannel)obj;
            uo.path = path;
            uo.propertyname = propertyname;
            uo.keys = Assign(keys, v_ => (Keyframe)v_);
            return uo;
        }
    }
}

