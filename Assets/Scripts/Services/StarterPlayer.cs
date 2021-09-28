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
        protected EosTransform _transform;
        [IgnoreMember] public EosTransform Transform => _transform;
        public TypeService()
        {
            _transform = ObjectFactory.CreateInstance<EosTransform>();
        }

        public override void OnCreate()
        {
            _transform.Create(Name);
            _active = false;
        }
        public override void Activate(bool active, bool recursivechild = true)
        {
            _transform.Transform.gameObject.SetActive(false);
        }
        public override void StartPlay()
        {
        }
    }

    [System.Serializable]
    [NoCreated]
    [MessagePackObject]
    public class StarterPlayer : TypeService
    {
        public StarterPlayer()
        {
            _transform.Name = "StarterPlayer";
        }
    }
    [System.Serializable]
    [NoCreated]
    [MessagePackObject]
    public class StarterPack : TypeService
    {
        public StarterPack()
        {
            _transform.Name = "StarterPack";
        }
    }
}