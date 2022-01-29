using System.Collections.Generic;
using ProtoBuf;
using Battlehub.RTSL;
using Eos.Objects;
using Eos.Objects.Battlehub.SL2;
using System;

using UnityObject = UnityEngine.Object;
namespace Eos.Objects.Battlehub.SL2
{
    [ProtoContract]
    public partial class PersistentEosHumanoid<TID> : PersistentSurrogate<TID>
    {
        [ProtoMember(256)]
        public int Level;

        [ProtoMember(257)]
        public float _radius;

        [ProtoMember(261)]
        public float _angularspeed;

        [ProtoMember(262)]
        public float _speed;

        [ProtoMember(263)]
        public float _accelation;

        protected override void ReadFromImpl(object obj)
        {
            base.ReadFromImpl(obj);
            EosHumanoid uo = (EosHumanoid)obj;
            Level = uo.Level;
            _radius = uo._radius;
            _angularspeed = GetPrivate<EosHumanoid,float>(uo, "_angularspeed");
            _speed = GetPrivate<EosHumanoid,float>(uo, "_speed");
            _accelation = GetPrivate<EosHumanoid,float>(uo, "_accelation");
        }

        protected override object WriteToImpl(object obj)
        {
            obj = base.WriteToImpl(obj);
            EosHumanoid uo = (EosHumanoid)obj;
            uo.Level = Level;
            uo._radius = _radius;
            SetPrivate(uo, "_angularspeed", _angularspeed);
            SetPrivate(uo, "_speed", _speed);
            SetPrivate(uo, "_accelation", _accelation);
            return uo;
        }

        public static implicit operator EosHumanoid(PersistentEosHumanoid<TID> surrogate)
        {
            if(surrogate == null) return default(EosHumanoid);
            return (EosHumanoid)surrogate.WriteTo(new EosHumanoid());
        }
        
        public static implicit operator PersistentEosHumanoid<TID>(EosHumanoid obj)
        {
            PersistentEosHumanoid<TID> surrogate = new PersistentEosHumanoid<TID>();
            surrogate.ReadFrom(obj);
            return surrogate;
        }
    }
}

