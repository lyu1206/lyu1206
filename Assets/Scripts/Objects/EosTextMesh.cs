using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MessagePack;

namespace Eos.Objects.UI
{
    using TMPro;
    using Service;
    [MessagePackObject]
    public class EosTextMesh : EosTransformActor
    {
        [Key(221)]
        private string _text;
        public string Text 
        {
            set
            {
                _text = value;
                if (_textmesh!=null)
                    _textmesh.text = value;
            }
            get => _text; }
        private TextMeshPro _textmesh;
        private int _updaterid;
        public EosTextMesh()
        {
        }
        public EosTextMesh(string name)
        {
            Name = name;
        }
        public override void OnCopyTo(EosObjectBase target)
        {
            if (!(target is EosTextMesh textmesh))
                return;
            base.OnCopyTo(target);
            textmesh.Name = Name;
            textmesh.Text = Text;
            textmesh.Transform.localPosition = Vector3.up * 30;
        }
        public override void OnCreate()
        {
            if (!ActiveInHierachy)
                return;
            var textobj = ObjectFactory.CreateUnityInstance("textmesh").gameObject;
            textobj.name = Name;
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