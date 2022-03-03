using UnityEngine;
using Battlehub.RTSL;
using Battlehub.RTCommon;
using Battlehub.RTSL.Battlehub.SL2;
using Battlehub.RTSL.Interface;
using Battlehub.Utils;
using ProtoBuf.Meta;

namespace Eos.Test
{
    using EosPlayer;
    using Objects;
    using Service;
    public class FastTest : MonoBehaviour
    {
        [SerializeField]
        private GameObject _bone;
        [SerializeField]
        private GameObject[] _gears;
        private void Awake()
        {
            TypeMap<long> typeMap = new TypeMap<long>();
            IOC.Register<ITypeMap>(typeMap);
        }
        // Start is called before the first frame update
        void Start()
        {
            var solution = ObjectFactory.CreateEosObject<Solution>(); solution.Name = "Solution";
            var workspace = ObjectFactory.CreateEosObject<Workspace>(); workspace.Name = "Workspace";
            solution.AddChild(workspace);

            var camera = ObjectFactory.CreateEosObject<EosCamera>();camera.Name = "Cam";
            camera.LocalPosition = new Vector3(0, 6, -8);
            camera.LocalRotation = new Vector3(25, 0, 0);
            workspace.AddChild(camera);

            var floor = ObjectFactory.CreateEosObject<EosShape>(PrimitiveType.Cube);floor.Name = "Floor";
            floor.LocalScale = new Vector3(50, 0.5f, 50);
            workspace.AddChild(floor);
            //var avatar = ObjectFactory.CreateEosObject<EosShape>(PrimitiveType.Capsule); floor.Name = "Avatar";
            //avatar.LocalPosition = new Vector3(0, 1, 0);
            //floor.AddChild(avatar);
            var objroot = ObjectFactory.CreateEosObject<EosTransformActor>();objroot.Name = "Root";
            objroot.LocalScale = Vector3.one * 0.1f;
            workspace.AddChild(objroot);

            var avatar = ObjectFactory.CreateEosObject<EosPawnActor>();avatar.Name = "Avatar";
            var bone = ObjectFactory.CreateEosObject<EosBone>();bone.Bone = _bone;bone.Name = "bone";
            avatar.AddChild(bone);
               
            objroot.AddChild(avatar);
            avatar.LocalPosition = new Vector3(0, 3, 0);
            avatar.LocalScale = Vector3.one;
            foreach (var gearsrc in _gears)
            {
                var gear = ObjectFactory.CreateEosObject<EosGear>();
                gear.Part = gearsrc;
                avatar.AddChild(gear);
            }
            camera.LocalPosition = new Vector3(0, 10, 10);
            camera.LookAt(avatar);


            EosPlayer.Instance.SetSolution(solution as Solution);
            EosPlayer.Instance.Play();


            var typemap = IOC.Resolve<ITypeMap>();
            var ptype = typemap.ToPersistentType(solution.GetType());
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
