#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Battlehub.RTCommon;

namespace Eos.Service
{
    using AI;
    using Eos.Objects;
    using Eos.Objects.Editor;
    using MessagePack;
    [MessagePackObject]
    public class SolutionMetaData : ReferPlayer , IMessagePackSerializationCallbackReceiver
    {
        public SolutionMetaData()
        {

        }
        public SolutionMetaData(Solution solution)
        {
            metadata = solution.MeataData;
        }
        [Key(1)]
        public List<ObjectMeataData> metadatas = new List<ObjectMeataData>();
        [Key(2)]
        public ObjectMeataData metadata = new ObjectMeataData();
        public void OnAfterDeserialize()
        {
            var metaqueue = new Queue<ObjectMeataData>(metadatas);
            Ref.Solution.IterChilds((child) =>
            {
                child.MeataData = metaqueue.Dequeue();
            }, true);
        }

        public void OnBeforeSerialize()
        {
            metadatas.Clear();
            Ref.Solution.IterChilds((child) =>
            {
                metadatas.Add(child.MeataData);
            },true);
        }
    }
    public partial class Solution
    {
        private static Solution _solution;
        private SolutionMetaData _solutionmetadata;
        public static Solution Instance => _solution;
        [IgnoreMember]public SolutionMetaData SolutionMetaData
        {
            get
            {
                return _solutionmetadata??new SolutionMetaData(this);
            }
            set
            {
                _solutionmetadata = value;
                MeataData = _solutionmetadata.metadata;
            }
        }
        public static EosEditorObject CreateEditorImpl()
        {
            _solution = new Solution();
            _solution.Name = "Solution";
            return _solution.InitEditorImpl();
        }
        private EosEditorObject InitEditorImpl()
        {
            _workspace = ObjectFactory.CreateInstance<Workspace>(ObjectType.Editor); _workspace.Name = "Workspace";
            _terrainservice = ObjectFactory.CreateInstance<TerrainService>(ObjectType.Editor);_terrainservice.Name = "Terrain";
//            _players = ObjectFactory.CreateInstance<Players>(ObjectType.Editor); _players.Name = "Players";
            _guiservice = ObjectFactory.CreateInstance<GUIService>(ObjectType.Editor);_guiservice.Name = "GUIService";
            //            _aiservice = ObjectFactory.CreateInstance<AIService>(ObjectType.Editor);
            return CreatedEditorImpl();
        }
        public override EosEditorObject CreatedEditorImpl()
        {
            var solution = base.CreatedEditorImpl();
            solution.AddChild(_workspace.CreatedEditorImpl());
            solution.AddChild(_terrainservice.CreatedEditorImpl());
            solution.AddChild(_guiservice.CreatedEditorImpl());
            return solution;
        }
        public override EosObjectBase CreateCloneObjectForEditor(ExposeToEosEditor editorobject)
        {
            var solution = new Solution();
            solution.ObjectID = ObjectID;
            return solution;
        }
        public void OpenMetaData()
        {

        }
    }
}
#endif