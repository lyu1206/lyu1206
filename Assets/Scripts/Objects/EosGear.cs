using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Eos.Objects
{
    public class EosGear : EosObjectBase
    {
        public GameObject Part;
        public override void OnCopyTo(EosObjectBase target)
        {
            if (!(target is EosGear targetgear))
                return;
            targetgear.Part = Part;
            base.OnCopyTo(target);
        }
        protected override void OnActivate(bool active)
        {
            if (!(_parent is EosPawnActor pawn))
                return;
            var orgmesh = Part.GetComponentInChildren<SkinnedMeshRenderer>();
            if (orgmesh != null)
            {
                var part = ObjectFactory.CreateUnityInstance(orgmesh.name).gameObject;// GameObject.Instantiate(orgmesh.gameObject);
                var skinmeshrender = part.AddComponent<SkinnedMeshRenderer>();
                //                Debug.Log($"skinnedmesh:{Part.name}");
                part.transform.parent = pawn.Transform;
                //                part.name = Part.name;
                part.transform.localPosition = orgmesh.transform.localPosition;
                part.transform.localRotation = orgmesh.transform.localRotation;
                part.transform.localScale = orgmesh.transform.localScale;
                pawn.Skeleton.SkinedMeshSetup(orgmesh,skinmeshrender);
            }
        }
        public override void OnAncestryChanged()
        {
        }
    }
}