using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using MessagePack;
using Battlehub.RTCommon;
using Battlehub.RTSL;
namespace Eos.Objects
{
    [EosObject]
    [DescendantOf(typeof(Eos.Service.Workspace))]
    [MessagePackObject]

    public class EosBone : EosObjectBase
    {
        [Key(21)]
        public long BoneGUID;
        private EosSkeleton _skeleton;
        private GameObject _bone;
        public GameObject Bone;

        public EosSkeleton Skeleton => _skeleton;
        public Transform BoneRoot => _bone.transform;

        public override void OnCreate()
        {
            _skeleton = new EosSkeleton();
        }
        public override void OnAncestryChanged()
        {
            base.OnAncestryChanged();
            SetupBone();
        }
        protected override void OnActivate(bool active)
        {
            base.OnActivate(active);
            if (!(_parent is EosTransformActor))
                return;
            OnReady();
        }
        private async Task SetupBone()
        {
            if (!(_parent is EosTransformActor parentrans))
                return;
            if (_bone != null)
                return;
            if (BoneGUID != 0)
            {
                var rsrv = Test.FastTest.Instance;
                var deps = new Stack<long>();
                rsrv.GetDependancies(BoneGUID, deps);
                await rsrv.AssetLoadTest(deps);
                var assetdb = IOC.Resolve <IAssetDB<long>>();
                _bone =  assetdb.FromID<UnityEngine.Object>(BoneGUID) as GameObject;

                //var uo = Test.FastTest.Instance.GetResource(BoneGUID);
                //_bone = (uo != null) ? Object.Instantiate(uo, parentrans.Transform.Transform, false) as GameObject : new GameObject(Name);
            }
            else
            {
                _bone = GameObject.Instantiate(Bone.gameObject, parentrans.Transform.Transform, false);
            }
            _skeleton.SetupSkeleton(parentrans.Transform.Transform, _bone.transform);           
        }
        private async void OnReady()
        {
            await SetupBone();
        }
    }
}
