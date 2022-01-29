using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Eos.Objects
{
    [EosObject]
    [DescendantOf(typeof(Eos.Service.Workspace))]

    public class EosShape : EosTransformActor
    {
        public PrimitiveType Type = PrimitiveType.Capsule;
        public EosShape()
        {

        }
        public EosShape(PrimitiveType type)
        {
            Type = type;
        }
        public override void OnCreate()
        {
            var primitive = GameObject.CreatePrimitive(Type);
            _transform.Transform = primitive.transform;
        }
    }
}
