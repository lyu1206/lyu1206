using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Eos.Service
{
    using Objects;
    [NoCreated]
    public class Player : EosObjectBase 
    #if UNITY_EDITOR
    , ITransform
    #endif
    {
        private EosHumanoid _humanoid;
        public EosHumanoid Humanoid => _humanoid;
        public ulong SID;

#if UNITY_EDITOR
        [MessagePack.IgnoreMember]
        public override string Name 
        { 
            get => base.Name; 
            set
            {
                base.Name = value;
                _transform.Name = value;
            }
        }
        private EosTransform _transform;
        public EosTransform Transform => _transform;
        public Player()
        {
            _transform = ObjectFactory.CreateInstance<EosTransform>();
            _transform.Create(Name);
        }
        public override void OnAncestryChanged()
        {
            base.OnAncestryChanged();
            if (!(_parent is ITransform transactor))
                return;
            _transform.SetParent(transactor.Transform);
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
    [NoCreated]
    public class Players : EosService , ITransform
    {
        private EosTransform _transform;
        public EosTransform Transform => _transform;
        private Player _localplayer;
        public Player LocalPlayer => _localplayer;
        public Players()
        {
            Name = "Players";
            _transform = ObjectFactory.CreateInstance<EosTransform>();
            _transform.Create(Name);
        }
        private Player CreatePlayer(ulong sid)
        {
            var player = ObjectFactory.CreateEosObject<Player>();
            player.Name = $"player{sid}";
            return player;
        }
        public void OnConnectPlayer(ulong sid)
        {
            var player = CreatePlayer(sid);
            _localplayer = player;
            AddChild(player);
            Ref.Solution.StarterPlayer.IterChilds((child) =>
            {
                child.Clone(player);
            });
            player.Activate(true);
        }
        protected override void OnStartPlay()
        {
            OnConnectPlayer(1);
        }
        public override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}