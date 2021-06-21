using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MessagePack;

namespace Eos.Service
{
    using Ore;
    using Eos.Objects;
    [NoCreated]
    [System.Serializable]
    public class TerrainService : EosService , ITransform
    {
        private EosTransform _transform;
        [IgnoreMember] public EosTransform Transform=>_transform;
        public TerrainService()
        {
            Name = "Terrain";
            _transform = ObjectFactory.CreateInstance<EosTransform>();
            _transform.Create(Name);
        }
        [IgnoreMember] public PVMOre _pvmOre;
        protected override void OnActivate(bool active)
        {
            base.OnActivate(active);
            _pvmOre?.Instantiate(_transform.Transform);
        } 
        public Transform FindNode(string name)
        {
            return _transform.Transform.FindDeepChild(name);
        }
    }
}