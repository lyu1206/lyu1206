using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MessagePack;

public interface ITransform
{
    Eos.Objects.EosTransform  Transform {get;}
}

namespace Eos.Service
{
    using Eos.Objects;
    [System.Serializable]
    [NoCreated]
    public class Workspace : EosService , ITransform
    {
        private EosTransform _workspace;
        [IgnoreMember]public  EosTransform Transform => _workspace;
        public Workspace()
        {
            Name = "Workspace";
            _workspace = ObjectFactory.CreateInstance<EosTransform>();
            _workspace.Create(Name);
        }
    }
}