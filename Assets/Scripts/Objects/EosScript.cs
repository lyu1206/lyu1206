using MessagePack;

namespace Eos.Objects
{
    using Ore;
    using Script;
    [System.Serializable]
    public class EosScript : EosObjectBase
    {
        [IgnoreMember]public string scriptname;
        [RequireMold("ScriptMold")]
        [Inspector("Ore","Script")]
        [Key(1000)] public OreReference ScriptOre { get; set; }
        private IScript _script;
        private int _scriptcoroutineid;
        protected override void OnActivate(bool active)
        {
            var ore = ScriptOre.GetOreObject();
            if (ore != null)
            {
                scriptname = ore.name;
            }
            _script = _script??IngameScriptContainer.GetScript(scriptname, this);
            if (active)
            {
                Ref.ScriptPlayer.RegistScript(_script);
            }
            else
            {
                _script.Enable = false;
                Ref.ScriptPlayer.UnRegistScript(_script);
            }

//            _scriptcoroutineid = Ref.Coroutine.OnCoroutineStart(script.Body());
        }
        public override void OnCopyTo(EosObjectBase target)
        {
            if (!(target is EosScript targetscript))
                return;
            targetscript.ScriptOre = ScriptOre;
            targetscript.scriptname = scriptname;
            base.OnCopyTo(target);
        }
        public override void OnDestroy()
        {
            _script?.Stop();
            Ref.ScriptPlayer.UnRegistScript(_script);
        }
    }
}