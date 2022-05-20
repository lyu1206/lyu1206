using System.Collections;
using UnityEngine;
using MessagePack;

namespace Eos.Objects
{
    [EosObject]
    [DescendantOf(typeof(Eos.Service.Workspace))]

    public class EosShape : EosTransformActor
    {
        [Key(201)]public PrimitiveType Type = PrimitiveType.Capsule;
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
