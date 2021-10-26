using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CTest : Eos.Service.Workspace
{
    public int A;
}
namespace Battlehub.RTCommon
{
    using Eos.Objects;
    [System.Serializable]
    public class ExposeToEosEditor : ExposeToEditor
    {
        [SerializeField]
        private uint _ownerID;
        public uint OwnerID => _ownerID;
        private EosObjectBase _owner;
        public EosObjectBase Owner 
        {
            get
            {
                if (_owner != null)
                    return _owner;
                return EosPlayer.EosPlayer.Instance.ObjectManager[_ownerID];
            }
            set
            {
                _owner = value;
                _ownerID = _owner.ObjectID;
            }
        }
    }
}