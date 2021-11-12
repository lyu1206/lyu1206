using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battlehub.RTEditor
{
    public class EosRTEDepth : RTEDeps
    {
        protected override IRuntimeEditor RTE
        {
            get
            {
                IRuntimeEditor rte = FindObjectOfType<EosRuntimeEditor>();
                if (rte == null)
                {
                    rte = gameObject.AddComponent<EosRuntimeEditor>();
                }
                return rte;
            }
        }
    }
}