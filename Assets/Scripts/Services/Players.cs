using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Eos.Service
{
    using Objects;
    public class Player : EosObjectBase 
    #if UNITY_EDITOR
    , ITransform
    #endif
    {
        private EosHumanoid _humanoid;
        public EosHumanoid Humanoid => _humanoid;
        public ulong SID;

#if UNITY_EDITOR
        public override string Name 
        { 
            get => base.Name; 
            set
            {
                base.Name = value;
                _transform.name = value;
            }
        }
        private Transform _transform;
        public Transform Transform => _transform;
        public Player()
        {
            var trans = new GameObject(Name);
            _transform = trans.transform;
        }
        public override void OnAncestryChanged()
        {
            base.OnAncestryChanged();
            if (!(_parent is ITransform transactor))
                return;
            _transform.parent = transactor.Transform;
            _transform.localPosition = Vector3.zero;
            _transform.localRotation = Quaternion.identity;
        }
#endif
        protected override void OnActivate(bool active)
        {
            base.OnActivate(active);
            var maincamera = EosCamera.Main;
            var model = FindChild<EosModel>();
            var humanoidroot = model.FindChild<EosTransformActor>(EosHumanoid.humanoidroot);
            maincamera.Target = humanoidroot;
            Ref.Solution.AIService.SetAccessPlate(model.FindChild<EosHumanoid>());
        }
        protected override void OnStartPlay()
        {
            var model = FindChild<EosModel>();
            var spainplayer = Ref.Solution.Terrain.FindNode("obj_SpawnIn");
            var humanoid = _humanoid = model.FindChild<EosHumanoid>();
            humanoid.SetPosition(spainplayer.position);


            // humanoidroot.Transform.position = spainplayer.position;
            // humanoidroot.Transform.rotation = spainplayer.rotation;
        }
    }
    public class Players : EosService , ITransform
    {
        private Transform _transform;
        public Transform Transform => _transform;
        private Player _localplayer;
        public Player LocalPlayer => _localplayer;
        public Players()
        {
            Name = "Players";
#if UNITY_EDITOR
            var trans = new GameObject(Name);
            _transform = trans.transform;
#endif
        }
        private Player CreatePlayer(ulong sid)
        {
            var player = new Player();
            player.Name = $"player{sid}";
            return player;
        }
        public void OnConnectPlayer(ulong sid)
        {
            var player = CreatePlayer(sid);
            _localplayer = player;
            AddChild(player);
        }
    }
}