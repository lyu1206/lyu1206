using System;
using UnityEngine;
namespace Eos.Objects
{
    using MessagePack;
    [Serializable]
    public class EosCamera : EosTransformActor
    {
        private EosTransformActor _target;
        private Camera _camera;
        public static EosCamera Main;
        public EosTransformActor Target
        {
            get=>_target;
            set
            {
                _target = value;
            }
        }
        protected override void OnActivate(bool active)
        {
            base.OnActivate(active);
            var camera = _camera = _transform.gameObject.AddComponent<Camera>();
            camera.name = Name;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Color.black;
            Camera.main.enabled = false;
            camera.enabled = true;

            Main = this;
        }
        protected override void OnStartPlay()
        {
            base.OnStartPlay();
            if (_target!=null)
                Ref.ObjectManager.RegistUpdateObject(this);
        }
        public override void Update(float delta)
        {
            var targetpos = _target.LocalPosition + new Vector3(-100,100,0);
            LocalPosition = targetpos;
            _transform.LookAt(_target.Transform,Vector3.up);
        }
        public Vector3 CameraspaceDirection(Vector3 vec)
        {
            return _camera.transform.TransformDirection(vec);
        }
    }
}