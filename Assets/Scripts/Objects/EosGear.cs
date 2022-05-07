using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using MessagePack;
using Battlehub.RTCommon;
using Battlehub.RTSL;

namespace Eos.Objects
{
    [EosObject]
    public class EosGear : EosObjectBase
    {
        [Key(21)]
        public long GearGUID;
        public GameObject Part;
        public override void OnCopyTo(EosObjectBase target)
        {
            if (!(target is EosGear targetgear))
                return;
            targetgear.Part = Part;
            base.OnCopyTo(target);
        }
        public override void OnCreate()
        {
            base.OnCreate();
            _ready = false;
        }
        public static void Gear(GameObject gearpart,Transform parenttrans,EosSkeleton skeleton)
        {
            var orgmesh = gearpart.GetComponentInChildren<SkinnedMeshRenderer>();
            if (orgmesh != null)
            {
                var part = ObjectFactory.CreateUnityInstance(orgmesh.name).gameObject;// GameObject.Instantiate(orgmesh.gameObject);
                var skinmeshrender = part.AddComponent<SkinnedMeshRenderer>();
                //                Debug.Log($"skinnedmesh:{Part.name}");
                part.transform.parent = parenttrans;
                //                part.name = Part.name;
                part.transform.localPosition = orgmesh.transform.localPosition;
                part.transform.localRotation = orgmesh.transform.localRotation;
                part.transform.localScale = orgmesh.transform.localScale;
                skeleton.SkinedMeshSetup(orgmesh, skinmeshrender);
            }
        }
        protected override void OnActivate(bool active)
        {
            if (!(_parent is EosPawnActor pawn))
                return;
            OnReady();
        }
        private async Task SetupGear()
        {
            var pawn = _parent as EosPawnActor;
            await TaskExtension.WaitUntil(pawn, (p) => p.Skeleton != null);

            //await Task.Delay(Random.Range(1, 10)*1000);
            if (GearGUID != 0)
            {
                await GearLoad();
            }
            else
            {
                legacy_GearLoad();
            }
        }
        private async Task GearLoad()
        {
            var pawn = _parent as EosPawnActor;
            var rsrv = Test.FastTest.Instance;
            var deps = new Stack<long>();
            rsrv.GetDependancies(GearGUID, deps);
            await rsrv.AssetLoadTest(deps);
            var assetdb = IOC.Resolve<IAssetDB<long>>();

            var part = assetdb.FromID<UnityEngine.Object>(GearGUID) as GameObject;
            var orgmesh = part.GetComponentInChildren<SkinnedMeshRenderer>();
            var skinmeshrender = part.AddComponent<SkinnedMeshRenderer>();
            //                Debug.Log($"skinnedmesh:{Part.name}");
            part.transform.SetParent(pawn.Transform.Transform);
            //                part.name = Part.name;
            part.transform.localPosition = orgmesh.transform.localPosition;
            part.transform.localRotation = orgmesh.transform.localRotation;
            part.transform.localScale = orgmesh.transform.localScale;
            pawn.Skeleton.SkinedMeshSetup(orgmesh, skinmeshrender);
        }
        private void legacy_GearLoad()
        {
            var pawn = _parent as EosPawnActor;
            var orgmesh = Part.GetComponentInChildren<SkinnedMeshRenderer>();
            if (orgmesh != null)
            {
                var part = ObjectFactory.CreateUnityInstance(orgmesh.name).gameObject;// GameObject.Instantiate(orgmesh.gameObject);
                var skinmeshrender = part.AddComponent<SkinnedMeshRenderer>();
                //                Debug.Log($"skinnedmesh:{Part.name}");
                part.transform.SetParent(pawn.Transform.Transform);
                //                part.name = Part.name;
                part.transform.localPosition = orgmesh.transform.localPosition;
                part.transform.localRotation = orgmesh.transform.localRotation;
                part.transform.localScale = orgmesh.transform.localScale;
                pawn.Skeleton.SkinedMeshSetup(orgmesh, skinmeshrender);
            }
        }
        public override void OnAncestryChanged()
        {
        }
        private async void OnReady()
        {
            await SetupGear();
            OnReadyEvent?.Invoke(this, null);
            OnReadyEvent = null;
            _ready = true;
        }

    }
}