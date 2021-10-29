using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Eos.Objects
{
    public partial class EosScript
    {
        public override void DoubleClicked()
        {
            base.DoubleClicked();
            var temppath = System.IO.Path.Combine(Application.persistentDataPath, $"{this.Name}.lua");


            File.WriteAllText(temppath, LuaScript);
            void LoadYourPhysicsProgram()
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                //startInfo.FileName = "PHYSICSPROGRAM.EXE";
                //startInfo.FileName = $"C:/Program Files (x86)/Notepad++/notepad++.exe";
                startInfo.FileName = $"C:/Users/kmioc/AppData/Local/Programs/Microsoft VS Code/Code.exe";
                startInfo.Arguments = $"-r {temppath}";
                Process.Start(startInfo);
            }
            LoadYourPhysicsProgram();
}
    }
}