using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITransform
{
    Transform Transform {get;}
}

namespace Eos.Service
{
    public class Workspace : EosService , ITransform
    {
        private GameObject _workspace;
        public Transform Transform => _workspace.transform;
        public Workspace()
        {
            Name = "Workspace";
            _workspace = new GameObject(Name);
        }
    }
}