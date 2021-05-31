using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Eos.Objects
{
    using Ore;
    public class EosMeshObject : EosTransformActor
    {
        public MeshOre MeshOre;
        public override void OnCopyTo(EosObjectBase target)
        {
            if (!(target is EosMeshObject targetmesh))
                return;
            targetmesh.MeshOre = MeshOre;
            base.OnCopyTo(target);
        }

        protected override void OnActivate(bool active)
        {
            base.OnActivate(active);
            var mesh = MeshOre?.Instantiate();
            if (_transform!=null)
            {
                mesh.parent = _transform.parent;
                mesh.localPosition = _transform.localPosition;
                mesh.localRotation = _transform.localRotation;
                GameObject.Destroy(_transform.gameObject);
            }
            mesh.name = Name;
            _transform = mesh;
        }
    }
}