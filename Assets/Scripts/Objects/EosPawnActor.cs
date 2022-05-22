using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Eos.Objects
{
    using Ore;
    using MessagePack;
    public  class  EosSkeleton 
    {
        private Transform[] _bones;
        private readonly Hashtable _bonesByHash =new Hashtable();
        private readonly List<Matrix4x4> _bindposes = new List<Matrix4x4>();
        private Transform _root;
        public void SetupSkeleton(Transform body,Transform root)
        {
            var boneindex = 0;
            _bones = root.GetComponentsInChildren<Transform>(true);
            _root = root.transform;
            foreach (var bone in _bones)
            {
                _bonesByHash.Add(bone.name,boneindex);
                _bindposes.Add(bone.worldToLocalMatrix * body.localToWorldMatrix);
                boneindex++;
            }
        }
        public void SkinedMeshSetup(SkinnedMeshRenderer skinmeshrender,SkinnedMeshRenderer target)
        {
            var newbones = new Transform[skinmeshrender.bones.Length];
            var newbindpos = new Matrix4x4[skinmeshrender.bones.Length];
            for (int i = 0; i < newbones.Length; i++)
            {
                var boneindex = (int) _bonesByHash[skinmeshrender.bones[i].name];
                newbones[i] = _bones[boneindex];
                newbindpos[i] = _bindposes[boneindex];
            }

            var mesh = new Mesh();

            var omesh = skinmeshrender.sharedMesh;
            var bones = skinmeshrender.bones;
                
            var ci = new CombineInstance
            {
                mesh =  omesh,
                transform = skinmeshrender.transform.localToWorldMatrix
            };
            mesh.CombineMeshes(new CombineInstance[]{ci});
            mesh.uv = omesh.uv;
            mesh.bindposes = _bindposes.ToArray();

            var boneWeights = new BoneWeight[omesh.boneWeights.Length];
            for (int i=0;i<omesh.boneWeights.Length;i++)
            {
                var bWeight = omesh.boneWeights[i];
                var obones = skinmeshrender.bones;
                bWeight.boneIndex0 = (int)_bonesByHash[bones[bWeight.boneIndex0].name];
                bWeight.boneIndex1 = (int)_bonesByHash[bones[bWeight.boneIndex1].name];
                bWeight.boneIndex2 = (int)_bonesByHash[bones[bWeight.boneIndex2].name];
                bWeight.boneIndex3 = (int)_bonesByHash[bones[bWeight.boneIndex3].name];
                boneWeights[i] = bWeight;
            }
            mesh.boneWeights = boneWeights;
            mesh.RecalculateBounds();

            target.sharedMaterial = skinmeshrender.sharedMaterial;
            target.sharedMesh = mesh;
            target.bones = _bones;
//                skinmeshrender.rootBone = newbones[0];// _bones[0];
            target.rootBone = _bones[0].GetChild(0);
        }

    }
    public class AnimationAdapter : MonoBehaviour
    {
        private EosPawnActor _actor;
        public void SetActor(EosPawnActor actor)
        {
            _actor = actor;
        }
        public void AniEvent(string name)
        {
            _actor.OnAnimationEvent?.Invoke(_actor, name);
        }
    }
    public class AnimationController
    {
        private Animation _animation;
        private EosPawnActor _owner;
        private string _curanimation;
        private bool _isloop;
        private int _checkerID = -1;
        public AnimationController(EosPawnActor owner, Animation animator)
        {
            _owner = owner;
            _animation = animator;
        }
        public void PlayNode(string name,bool rewind = false)
        {
            _curanimation = name;
            _animation.CrossFade(name, 0.1f, 0);
//            var state = _animatipm.GetNextAnimatorStateInfo(0);
//            if (_checkerID != -1)
//                _owner.Ref.Coroutine.OnStopCoroutine(_checkerID);
//            var clips = _animatipm.GetCurrentAnimatorClipInfo(0);
//            if (clips.Length==0)
//            {
////                Debug.Break();
//                return;
//            }
//            _owner.Ref.Coroutine.OnCoroutineStart(CheckAniEnd(_animatipm.GetCurrentAnimatorClipInfo(0)[0].clip.length));
//            _checkerID = _owner.Ref.Coroutine.NowID;
        }
        private IEnumerator CheckAniEnd(float length)
        {
            yield return new WaitForSeconds(length);
            _owner.OnAnimationStopped?.Invoke(_owner, _curanimation);
        }
    }
    [EosObject]
    [DescendantOf(typeof(Eos.Service.Workspace))]
    public partial class EosPawnActor : EosTransformActor
    {
        [IgnoreMember]public BodyOre Body { get; set; }// will be deleted..
        [RequireMold("BodyMolds")]
        [Inspector("Ore", "Body")]
        [Key(331)] public OreReference BodyOre { get; set; } = new OreReference();
        [IgnoreMember] public EosSkeleton Skeleton => _skeleton;
        private EosSkeleton _skeleton;
        private Animation _animator;
        private Rigidbody _rigidbody;
        private AnimationController _controller;
        [IgnoreMember] public Rigidbody Rigidbody => _rigidbody;
        [IgnoreMember] public List<EosCollider> Collders = new List<EosCollider>();
        [IgnoreMember] public EventHandler<string> OnAnimationEvent;
        [IgnoreMember] public EventHandler<string> OnAnimationStopped;
        public override void OnCopyTo(EosObjectBase target)
        {
            if (!(target is EosPawnActor targetpawn))
                return;
            targetpawn.Body = Body;
            targetpawn.BodyOre = BodyOre;
            base.OnCopyTo(target);
        }

        public bool RayCast()
        {
            return false;
        }

        protected override void OnActivate(bool active)
        {
            base.OnActivate(active);
//            if (BodyOre == null)
//                return;
//            var bodyore = BodyOre.GetOre();
//            if (bodyore != null)
//                Body = bodyore.GetComponent<BodyOre>();
//            var body = Body.GetBody();
//            _skeleton = new EosSkeleton();
//            _animator = body.GetComponent<Animator>();
//            _controller = new AnimationController(this, _animator);
//            var anievent = body.AddComponent<AnimationAdapter>();
//            anievent.SetActor(this);
//            body.transform.SetParent(Transform.Transform);
//            body.transform.localPosition = Vector3.zero;
//            body.transform.localRotation = Quaternion.identity;

//            _skeleton.SetupSkeleton(Transform.Transform, body.transform);

//            var initialparts = Body.GetInitialParts();
////            var gears = FindChilds<EosGear>();
//            foreach(var part in initialparts)
//            {
//                //                if (gears.Count > 0 && gears.Exists(it => it.Part == part))
//                //                    continue;
//                var gear = ObjectFactory.CreateInstance<EosGear>();// new EosGear();
//                gear.Part = part;
//                AddChild(gear);
//            }
        }
        public void PlayNode(string name,bool rewind = false)
        {
            _controller?.PlayNode(name,rewind);
        }
        public override void OnChildAdded(EosObjectBase child)
        {
            if (child is EosCollider collider)
            {
                Collders.Add(collider);
                _rigidbody = Transform.GetComponent<Rigidbody>();
                if (_rigidbody == null)
                {
                    _rigidbody = Transform.AddComponent<Rigidbody>();
//                    _rigidbody.isKinematic = true;
                    _rigidbody.useGravity = true;
                    _rigidbody.drag = 0;
                    _rigidbody.angularDrag = 0;
                    _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
                }
            }
            if (child is EosBone bone)
            {
                //bone.OnReadyEvent += (sender, arga) =>
                  {
                      _skeleton = bone.Skeleton;
                      var skel = bone.BoneRoot;
                      _animator = skel.GetComponent<Animation>();
                      _controller = new AnimationController(this, _animator);
                      var anievent = skel.gameObject.AddComponent<AnimationAdapter>();
                      anievent.SetActor(this);
                  };
            }
        }
        public override void OnChildRemoved(EosObjectBase child)
        {
            if (child is EosCollider collider)
            {
                Collders.Remove(collider);
                if (Collders.Count==0)
                {
                    GameObject.Destroy(_rigidbody);
                    _rigidbody = null;
                }
            }
        }
    }
}