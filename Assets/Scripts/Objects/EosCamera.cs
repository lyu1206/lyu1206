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
            _transform.LookAt(_target.Transform, Vector3.up);
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
        public Vector3 CameraspaceDirection(Vector3 vec)
        {
            return _camera.transform.TransformDirection(vec);
        }
    }
}