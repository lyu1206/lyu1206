using System.IO;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using NLua;

namespace EosLuaPlayer
{
    public static class PrimaryLuaFunction
    {
        public static void BindFunction(Lua state)
        {
            var type = typeof(PrimaryLuaFunction);
            state.RegisterFunction("print", type.GetMethod("Print", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public));
        }
        public static void Print(params object[] args)
        {
            var b = new StringBuilder();
            args.ForEach(it => b.Append(it.ToString()));
            UnityEngine.Debug.Log(b.ToString());
        }
    }
    public class EosLuaPlayer : Eos.Objects.ReferPlayer
    {
        private const string str_loadmodule = "loadmodule";
        public Lua LuaMain { get; private set; } = null;
        private LuaFunction _updater = null;
        private LuaFunction _loadmodule = null;
        public void Init()
        {
            LuaMain = new Lua();
            PrimaryLuaFunction.BindFunction(LuaMain);
            LoadBuiltInLuas();

            _updater = LuaMain["player.Update"] as LuaFunction;
            _updater.Call();
        }
        public void Update()
        {
            LuaMain["Time.deltaTime"] = Time.deltaTime;
            _updater?.Call();
        }
        public void LateUpdate()
        {

        }
        private void LoadModule(string code,string name,string chunkkey)
        {
            _loadmodule = _loadmodule ?? LuaMain[str_loadmodule] as LuaFunction;
            _loadmodule.Call(code, name, chunkkey);
        }
        private void LoadBuiltInLuas()
        {
            var luapath = $"{Application.streamingAssetsPath}/BuiltInLua";
            var files = Directory.GetFiles(luapath, "*.lua");
            files = files.Sort((a, b) => 
            {
                var sa = a.Split('_').Length;
                var sb = b.Split('_').Length;
                if (sa > sb) return 1;
                else if (sa < sb) return -1;
                return 0;
            });
            foreach(var file in files)
            {
                var chunkname = Path.GetFileName(file);
                var name = Path.GetFileNameWithoutExtension(file);
                var attribute = name.Split('_');
                var code = File.ReadAllText(file, Encoding.UTF8);
                var ismodule = attribute.Length > 1;
                if (!ismodule)
                    LuaMain.DoString(code);
                else
                    LoadModule(code, attribute[0], chunkname);
            }
        }
    }
}