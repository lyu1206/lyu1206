using MessagePack;

namespace Eos.Objects
{
    using Script;
    public class EosScript : EosObjectBase
    {
        [Key(1000)]
        public string scriptname;
        private IScript _script;
        private int _scriptcoroutineid;
        protected override void OnActivate(bool active)
        {
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