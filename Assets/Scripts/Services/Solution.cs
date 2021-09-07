using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MessagePack;

namespace Eos.Service
{
    using AI;
    using Objects;

    [NoCreated]
    [MessagePackObject]
    public abstract class EosService : EosObjectBase
    {

    }
    [System.Serializable]
    [NoCreated]
    [MessagePackObject]
    public partial class Solution : EosService 
    {
        private Workspace _workspace;
        private TerrainService _terrainservice;
        private Players _players;
        private GUIService _guiservice;
        private AIService _aiservice;
        private StarterPlayer _starterplayer;
        private StarterPack _starterpack;
        [IgnoreMember] public Workspace Workspace { get => _workspace; set => _workspace = value; }
        [IgnoreMember] public TerrainService Terrain { get => _terrainservice; set => _terrainservice = value; }
        [IgnoreMember] public GUIService GUIService { get => _guiservice; set => _guiservice = value; }
        [IgnoreMember]public AIService AIService => _aiservice;
        [IgnoreMember] public StarterPlayer StarterPlayer => _starterplayer;
        [IgnoreMember] public StarterPack StarterPack => _starterpack;
        [IgnoreMember] public Players Players => _players;

        public void StartGame()
        {
            _aiservice = new AIService();
            _workspace = FindChild<Workspace>();
            _terrainservice = FindChild<TerrainService>();

            _players = ObjectFactory.CreateEosObject<Players>();
            AddChild(_players);

            _guiservice = FindChild<GUIService>();
            _starterplayer = FindChild<StarterPlayer>();
            _starterpack = FindChild<StarterPack>();

            _childrens.ForEach(child => child.Activate(true));
            _childrens.ForEach(child => child.StartPlay());
        }
    }
}
