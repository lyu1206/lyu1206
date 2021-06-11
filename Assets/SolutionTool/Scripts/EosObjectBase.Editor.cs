#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Eos.Objects
{
    using Editor;
    public partial class EosObjectBase
    {
        public virtual EosEditorObject CreatedEditorImpl()
        {
            return EosEditorObject.Create(this);
        }
    }
}
#endif