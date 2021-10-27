#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MessagePack;

namespace Eos.Objects
{
    using Eos.Objects.Editor;
    using Ore;
    using Service;
    public partial class EosTerrain : EosObjectBase
    {
        private GameObject _terrainobj;
        private OreReference _oldterrainOre;
        public override void PropertyChanged(EosObjectBase parent)
        {
            if (_oldterrainOre.Equals(TerrainOre))
                return;
            _oldterrainOre = TerrainOre;
            if (_terrainobj != null)
                GameObject.Destroy(_terrainobj);
            if (!(parent is TerrainService ts))
                return;
            _terrainobj = TerrainOre.Instantiate();
            _terrainobj.transform.SetParent(ts.Transform.Transform, true);
            var pvm = _terrainobj.GetComponent<PVMOre>();
            var terrain = pvm.Instantiate(ts.Transform.Transform);
        }
        public override void CreatedOnEditor()
        {
            base.CreatedOnEditor();
            PropertyChanged(Parent);
            //            Camera.allCameras[0].transform.localPosition = terrain.transform.localPosition;
        }
    }
}
#endif