using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EosPlayer
{
    using Eos.Service;
    using Eos.Objects;

    public partial class EosPlayer : MonoBehaviour
    {
        private ObjectManager _editobjectmanager;
        public void PreviewSetup()
        {
            _editobjectmanager = _objectmanager;
            _objectmanager = new ObjectManager();
        }
        public void StopPreview()
        {
            Stop();
            _objectmanager.Reset();
            _objectmanager = _editobjectmanager;
        }
        public EosObjectBase GetObjectFromEditObject(uint objid)
        {
            return _editobjectmanager[objid];
        }
    }
}