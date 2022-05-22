using System.Collections;
using UnityEngine;
using MessagePack;

namespace Eos.Objects
{
    [EosObject]
    [DescendantOf(typeof(Eos.Service.Workspace))]
    public class EosShape : EosTransformActor
    {
        private PrimitiveType _type = PrimitiveType.Capsule;
        [Key(201)]
        public PrimitiveType Type
        {
            set
            {
                _transform.Destroy();
                _type = value;         
                var primitive =GameObject.CreatePrimitive(Type);
                primitive.name = Name;
                _transform.Transform = primitive.transform;
            }
            get => _type;
        }

        [IgnoreMember]
        public int PType
        {
            set
            {
                Type = (PrimitiveType) value;
            }
        }
        public EosShape()
        {

        }
        public EosShape(PrimitiveType type)
        {
            Type = type;
        }
        public override void OnCreate()
        {
            Type = _type;
        }
    }
}
