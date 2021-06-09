using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Eos.Objects.UI
{
    using TMPro;
    using Service;
    public class EosTextMesh : EosTransformActor
    {
        public string Text { set => _textmesh.text = value; get => _textmesh.text; }
        private TextMeshPro _textmesh;
        private int _updaterid;
        public EosTextMesh(string name)
        {
            
            var textobj = new GameObject("textmesh");
            textobj.name = name;
            var textpro = _textmesh = textobj.AddComponent<TextMeshPro>();
            textpro.text = "Hello";
            textpro.alignment = TextAlignmentOptions.Center;
            textpro.fontSize = 50;
            GameObject.Destroy(_transform.gameObject);
            _transform = textobj.transform;
            _transform.localPosition = Vector3.up * 30;
        }
        protected override void OnActivate(bool active)
        {
            _updaterid = Ref.Scheduler.Schedule(() =>
            {
                _transform.rotation = EosCamera.Main.Transform.rotation;
            });
        }
        public override void OnDestroy()
        {
            base.OnDestroy();
            Ref.Scheduler.UnSchedule(_updaterid);
        }
    }
}