using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Eos.Ore
{
    using Objects;
    // ���� ����ϸ� �ȵǰ�,���� �غ�Ǹ� ����������� ��ü�� ��ü�ؾ��Ѵ�.
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