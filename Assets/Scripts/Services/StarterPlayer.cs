using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MessagePack;

namespace Eos.Service
{
    using Objects;

    [NoCreated]
    [MessagePackObject]
    public class TypeService : EosService, ITransform
    {
        protected Transform _transform;
        [IgnoreMember] public Transform Transform => _transform;
        public TypeService()
        {
            _transform = ObjectFactory.CreateUnityInstance();
            _transform.gameObject.SetActive(false);
        }

        public override void OnCreate()
        {
            Active = false;
        }
        public override void Activate(bool active, bool recursivechild = true)
        {
        }
        public override void StartPlay()
        {
        }
    }

    [NoCreated]
    [MessagePackObject]
    public class StarterPlayer : TypeService
    {
        public StarterPlayer()
        {
            _transform.name = "StarterPlayer";
        }
    }
    [NoCreated]
    [MessagePackObject]
    public class StarterPack : TypeService
    {
        public StarterPack()
        {
            _transform.name = "StarterPack";
        }
    }
}