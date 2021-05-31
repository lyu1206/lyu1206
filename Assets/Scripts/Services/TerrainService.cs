using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Eos.Service
{
    using Ore;
    public class TerrainService : EosService , ITransform
    {
        private Transform _transform;
        public Transform Transform=>_transform;
        public TerrainService()
        {
            Name = "Terrain";
            _transform = new GameObject(Name).transform;
        }
        public PVMOre _pvmOre;
        protected override void OnActivate(bool active)
        {
            base.OnActivate(active);
            _pvmOre.Instantiate(_transform);
        } 
        public Transform FindNode(string name)
        {
            return _transform.FindDeepChild(name);
        }
    }
}