#if UNITY_EDITOR
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Eos.Assets
{
    public partial class Mold
    {
        public string [] GetMoldNames()
        {
            var names = _sources.Select((mold, index) => mold.name);
            return names.ToArray();
        }
    }
}
#endif