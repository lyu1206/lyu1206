using Battlehub.RTCommon;
using Battlehub.RTSL.Interface;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.Battlehub.SL2;
using UnityObject = UnityEngine.Object;
using UnityEngine;
using System.IO;
namespace Battlehub.RTSL.Battlehub.SL2
{
    [ProtoContract]
    public class PersistentRuntimeSolution<TID> : PersistentObject<TID>, ICustomSerialization
    {
        public bool AllowStandardSerialization => throw new NotImplementedException();

        public void Deserialize(Stream stream, BinaryReader reader)
        {
            throw new NotImplementedException();
        }

        public void Serialize(Stream stream, BinaryWriter writer)
        {
            throw new NotImplementedException();
        }
        protected override void ReadFromImpl(object obj)
        {
            base.ReadFromImpl(obj);
        }
        protected override object WriteToImpl(object obj)
        {
            return base.WriteToImpl(obj);
        }
    }
}
