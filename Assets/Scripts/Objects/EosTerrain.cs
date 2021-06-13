using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MessagePack;

namespace Eos.Objects
{
    using Ore;
    using Service;
    public partial class EosTerrain : EosObjectBase
    {
        [RequireMold("TerrainMold")]
        [Inspector("Terrain", "Ore")]
        [Key(41)] public Ore.OreReference TerrainOre { get; set; } = new OreReference();
        protected override void OnActivate(bool active)
        {
            if (!(_parent is TerrainService ts))
                return;
            var terrain = TerrainOre.Instantiate();
            terrain.transform.SetParent(ts.Transform, true);
        }
    }
}