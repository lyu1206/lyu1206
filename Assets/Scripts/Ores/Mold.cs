using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Eos.Assets
{
    using Ore;
    using GuAsset;
    public partial class Mold : ScriptableObject
    {
        public const string MoldPath = "Molds";
        [SerializeField]
        private Object[] _sources;
        public static Mold GetMold(string name)
        {
            var path = Path.Combine(MoldPath, name);
            var mold = Resources.Load<Mold>(path);
            return mold;
        }
        public int GetIndexInMold(OreReference ore)
        {
            return _sources.FindIndex(it => it.GetInstanceID() == ore.OreID);
        }
        public int GetOreID(int index)
        {
            return _sources[index].GetInstanceID();
        }
        public GameObject GetOre(int instanceID)
        {
            var ret = _sources.Find<Object>(it => it.GetInstanceID() == instanceID);
            return ret as GameObject;
        }
        public Object GetScriptOre(int instanceID)
        {
            return _sources.Find<Object>(it => it.GetInstanceID() == instanceID);
        }
    }
}