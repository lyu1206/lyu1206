using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Eos.Ore
{
    using Assets;
    using MessagePack;
    using Objects;
    [MessagePack.MessagePackObject]
    public struct OreReference
    {
        [Key(331)]public int OreID;
        [Key(332)]public string Mold;
        public GameObject GetOre()
        {
            var mold = Assets.Mold.GetMold(Mold);
            return mold.GetOre(OreID);
        }
        public GameObject Instantiate()
        {
            var mold = Assets.Mold.GetMold(Mold);
            var src = mold.GetOre(OreID);
            var obj = GameObject.Instantiate(src);
            obj.name = src.name;
            return obj;
        }
        public override bool Equals(object obj)
        {
            if (!(obj is OreReference refer))
                return false;
            return refer.OreID == OreID && refer.Mold == Mold;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
    // 모노로 사용하면 안되고,툴이 준비되면 씨리얼라이즈 객체로 교체해야한다.
    public class OreBase : MonoBehaviour
    {
        public Transform Instantiate()
        {
            return (GameObject.Instantiate(gameObject) as GameObject).transform;
        }
        public Transform Instantiate(Transform parent)
        {
            var oreobj = GameObject.Instantiate(gameObject) as GameObject;
            oreobj.transform.SetParent(parent);
            oreobj.transform.localPosition = gameObject.transform.localPosition;
            oreobj.transform.localRotation = gameObject.transform.localRotation;
            return oreobj.transform;
        }
    }
    public abstract class BoltLinkOre : OreBase
    {
        public abstract void SetObject(EosObjectBase eosobj);
    }

}