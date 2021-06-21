#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MessagePack;

namespace Eos.Objects
{
    using Ore;
    using Service;
    public partial class EosPawnActor 
    {
        private GameObject _bodyobj;
        private OreReference _oldterrainOre;
        public override void PropertyChanged(EosObjectBase parent)
        {
            if (_oldterrainOre.Equals(BodyOre))
                return;
            _oldterrainOre = BodyOre;
            if (_bodyobj != null)
                GameObject.DestroyImmediate(_bodyobj);
            if (!(parent is ITransform ws))
                return;
            var bodyore = BodyOre.GetOre().GetComponent<BodyOre>();
            var body = bodyore.GetBody();
            _skeleton = new EosSkeleton();
            _animator = body.GetComponent<Animator>();
            _controller = new AnimationController(this, _animator);
            body.transform.SetParent(Transform.Transform);
            body.transform.localPosition = Vector3.zero;
            body.transform.localRotation = Quaternion.identity;

            _bodyobj = body;

            _skeleton.SetupSkeleton(Transform.Transform, body.transform);

            var initialparts = bodyore.GetInitialParts();
            //            var gears = FindChilds<EosGear>();
            foreach (var part in initialparts)
            {
                //                if (gears.Count > 0 && gears.Exists(it => it.Part == part))
                //                    continue;
                EosGear.Gear(part,_bodyobj.transform,_skeleton);
            }
        }
    }
}
#endif