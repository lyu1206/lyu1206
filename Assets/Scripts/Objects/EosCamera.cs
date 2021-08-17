using System;
using UnityEngine;
namespace Eos.Objects
{
    using MessagePack;
    [Serializable]
    [MessagePackObject]
    public class EosCamera : EosTransformActor
    {
        private EosTransformActor _target;
        private Camera _camera;
        public EosCamera()
        {
            if (_camera == null)
            {
                _camera = _transform.Transform.gameObject.AddComponent<Camera>();
                Camera.SetupCurrent(_camera);
            }
        }
        [Key(330)]public override Vector3 LocalPosition 
        { 
            get => base.LocalPosition;
            set
            {
//                _camera.transform.localPosition = value;
                base.LocalPosition = value;
            }
        }
        [Key(331)]public override Vector3 LocalRotation 
        { 
            get => base.LocalRotation;
            set
            {
                base.LocalRotation = value;
            }
        }
        [IgnoreMember]public static EosCamera Main;
        [IgnoreMember]public EosTransformActor Target
        {
            get=>_target;
            set
            {
                _target = value;
                UnRegistComponent(update);
                if (_target != null)
                    RegistComponent(update);
            }
        }
        private void update(object sender,float delta)
        {
            var targetpos = _target.LocalPosition + new Vector3(-100, 100, 0);
            LocalPosition = targetpos;
            _transform.Transform.LookAt(_target.Transform.Transform, Vector3.up);
        }
        protected override void OnActivate(bool active)
        {
            base.OnActivate(active);
            var camera = _camera;// = _transform.Transform.gameObject.AddComponent<Camera>();
            camera.name = Name;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Color.black;
//            Camera.main.enabled = false;
            camera.enabled = true;

            Main = this;
        }
        public Vector3 CameraspaceDirection(Vector3 vec)
        {
            return _camera.transform.TransformDirection(vec);
        }
    }
}