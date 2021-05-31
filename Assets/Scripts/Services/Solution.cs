using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Eos.Service
{
    using AI;
    using Objects;

    public class EosService : EosObjectBase
    {

    }
    public class Solution : EosService
    {
        private Workspace _workspace;
        private TerrainService _terrainservice;
        private Players _players;
        private GUIService _guiservice;
        private AIService _aiservice;
        public Workspace Workspace =>_workspace;
        public TerrainService Terrain => _terrainservice;
        public GUIService GUIService => _guiservice;
        public AIService AIService => _aiservice;


        public Players Players => _players;

        public void StartGame()
        {
            _workspace = FindChild<Workspace>();
            _terrainservice = FindChild<TerrainService>();
            _players = FindChild<Players>();
            _guiservice = FindChild<GUIService>();
            _aiservice = FindChild<AIService>();
            
            _childrens.ForEach(child => child.Activate(true));
            _childrens.ForEach(child => child.StartPlay());
        }
    }
}
