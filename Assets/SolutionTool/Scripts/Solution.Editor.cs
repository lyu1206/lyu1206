#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Eos.Service
{
    using AI;
    using Eos.Objects.Editor;
    public partial class Solution
    {
        private static Solution _solution;
        public static Solution Instance => _solution;
        public static Solution CreateEditorImpl()
        {
            _solution = new Solution();
            _solution.Name = "Solution";
            _solution.InitEditorImpl();
            return _solution;
        }
        private void InitEditorImpl()
        {
            _workspace = ObjectFactory.CreateInstance<Workspace>(ObjectType.Editor); _workspace.Name = "Workspace";
            _terrainservice = ObjectFactory.CreateInstance<TerrainService>(ObjectType.Editor);_terrainservice.Name = "Terrain";
//            _players = ObjectFactory.CreateInstance<Players>(ObjectType.Editor); _players.Name = "Players";
            _guiservice = ObjectFactory.CreateInstance<GUIService>(ObjectType.Editor);_guiservice.Name = "GUIService";
            //            _aiservice = ObjectFactory.CreateInstance<AIService>(ObjectType.Editor);
            CreatedEditorImpl();
        }
        public override EosEditorObject CreatedEditorImpl()
        {
            var solution = base.CreatedEditorImpl();
            solution.AddChild(_workspace.CreatedEditorImpl());
            solution.AddChild(_terrainservice.CreatedEditorImpl());
            solution.AddChild(_guiservice.CreatedEditorImpl());
            return solution;
        }
    }
}
#endif