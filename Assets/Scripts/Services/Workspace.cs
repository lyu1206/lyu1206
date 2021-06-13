using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MessagePack;

public interface ITransform
{
    Transform Transform {get;}
}

namespace Eos.Service
{
    [NoCreated]
    public class Workspace : EosService , ITransform
    {
        private GameObject _workspace;
        [IgnoreMember]public Transform Transform => _workspace.transform;
        public Workspace()
        {
            Name = "Workspace";
            _workspace = ObjectFactory.CreateUnityInstance(Name).gameObject;
        }
    }
}