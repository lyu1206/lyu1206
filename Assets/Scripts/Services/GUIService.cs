using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using MessagePack;

namespace Eos.Service
{
    using Eos.Objects;
    using EosPlayer;
    [Serializable]
    [NoCreated]
    public class GUIService : EosService , ITransform
    {
        private EosTransform  _guiroot;
        private Canvas _canvas;
        [IgnoreMember]public EosTransform Transform => _guiroot;
        public GUIService()
        {
            _guiroot = ObjectFactory.CreateInstance<EosTransform>();
            var root = new GameObject("GUIRoot");//  _guiroot.Create("GUIRoot").gameObject;
            _canvas = root.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            root.AddComponent<CanvasScaler>();
            root.AddComponent<GraphicRaycaster>();
            var eventsystem = ObjectFactory.CreateUnityInstance("EventSystem", typeof( EventSystem), typeof(StandaloneInputModule)).gameObject;
            eventsystem.transform.parent = root.transform;
            _guiroot.Transform = root.transform;
        }
        public static Canvas Canvas;
        public UGUIEvents RegistUIEvent(Transform eventobject,object reciever)
        {
            var uguievent = eventobject.gameObject.AddComponent<UGUIEvents>();
            return uguievent;
        }
        public void UnRegistUIEvent(UGUIEvents uievent)
        {
            if (uievent != null)
                GameObject.Destroy(uievent);
        }
        public override void OnDestroy()
        {
            _guiroot?.Destroy();
        }
    }
}
