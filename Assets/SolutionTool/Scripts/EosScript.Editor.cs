using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Battlehub.RTCommon;

namespace Eos.Objects
{
    using Battlehub.RTEditor;
    public partial class EosScript
    {
        public override void RTEOnCreated(IRTE editor)
        {
            base.RTEOnCreated(editor);
            var ws = IOC.Resolve<EosWorkspace>();
            var path = Path.Combine(ws.GetScriptPath(this),$"{Name}.lua");
        }
        public void ReadyForEdit()
        {
            Debug.Log($"script : {this.Name}");
            var ws = IOC.Resolve<EosWorkspace>();
            var path = Path.Combine(ws.GetScriptPath(this), $"{Name}.lua");
            if (!File.Exists(path))
                File.WriteAllText(path, LuaScript);
            ScriptEditor.OpenScript(path);
        }
    }
}