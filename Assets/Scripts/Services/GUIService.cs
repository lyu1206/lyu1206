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
    public partial class GUIService : EosService , ITransform
    {
        private EosTransform  _guiroot;
        private Canvas _canvas;
        private Transform _root;
        [IgnoreMember] public Canvas Canvas => _canvas;
        [IgnoreMember]public EosTransform Transform => _guiroot;
        public GUIService()
        {
            _guiroot = ObjectFactory.CreateInstance<EosTransform>();
        }
        public override void OnCreate()
        {
            base.OnCreate();
            _guiroot.Create("GUIRoot");
        }
        private void CreateCanvas()
        {
            _canvas = _guiroot.GetComponent<Canvas>();
            if (_canvas != null)
                return;
            _canvas = _guiroot.AddComponent<Canvas>();
            _guiroot.AddComponent<CanvasScaler>();
            _guiroot.AddComponent<GraphicRaycaster>();

            _root = _guiroot.Transform;

            if (EventSystem.current == null)
            {
                var eventsystem = ObjectFactory.CreateUnityInstance("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule)).gameObject;
                eventsystem.transform.parent = _root.transform;
            }
            //            _guiroot.Transform = root.transform;
        }
        protected override void OnActivate(bool active)
        {
            base.OnActivate(active);
            if (_canvas != null)
                return;
            CreateCanvas();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }
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
