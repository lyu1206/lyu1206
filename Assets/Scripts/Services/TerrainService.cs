using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MessagePack;

namespace Eos.Service
{
    using Ore;
    [NoCreated]
    public class TerrainService : EosService , ITransform
    {
        private Transform _transform;
        [IgnoreMember] public Transform Transform=>_transform;
        public TerrainService()
        {
            Name = "Terrain";
            _transform = ObjectFactory.CreateUnityInstance(Name);
        }
        [IgnoreMember] public PVMOre _pvmOre;
        protected override void OnActivate(bool active)
        {
            base.OnActivate(active);
            _pvmOre?.Instantiate(_transform);
        } 
        public Transform FindNode(string name)
        {
            return _transform.FindDeepChild(name);
        }
    }
}