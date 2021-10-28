using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NLua;

namespace EosLuaPlayer
{
    public class EosLuaPlayer : Eos.Objects.ReferPlayer
    {
        public Lua LuaMain { get; private set; } = null;
        private LuaFunction _updater;
        public void Init()
        {
            LuaMain = new Lua();
            LuaMain.DoString(
                @"
                    local updater = {}
                    function updater.Update()
                        print('AAAAAAAAAAAA')
                    end
                 ","Main"
                );
            _updater = LuaMain["updater.Update"] as LuaFunction;
        }
        public void Update()
        {
            LuaMain["Time.Delta"] = Time.deltaTime;
            _updater?.Call();
        }
        public void LateUpdate()
        {

        }
    }
}