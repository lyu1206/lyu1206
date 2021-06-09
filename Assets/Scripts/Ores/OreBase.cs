using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Eos.Ore
{
    using Objects;
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