using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Eos.Service
{
    using EosPlayer;
    public class GUIService : EosService , ITransform
    {
        private Transform _guiroot;
        private Canvas _canvas;
        public Transform Transform => _guiroot;
        public GUIService()
        {
            var root = new GameObject("GUIRoot");
            _canvas = root.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            root.AddComponent<CanvasScaler>();
            root.AddComponent<GraphicRaycaster>();
            var eventsystem = new GameObject("EventSystem", typeof( EventSystem), typeof(StandaloneInputModule));
            eventsystem.transform.parent = root.transform;
            _guiroot = root.transform;
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
            GameObject.Destroy(_guiroot.gameObject);
        }
    }
}
