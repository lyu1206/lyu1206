using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battlehub.UIControls
{
    using RTEditor;
    using RTCommon;
    public class EosVirtualizingTreeViewItem : VirtualizingTreeViewItem
    {
        public delegate void EosHierachItemEventHandler(VirtualizingItemContainer sender);

        private bool _ishover;
        [SerializeField]
        private UnityEngine.UI.Toggle _hovertoggle;
        [SerializeField]
        private UnityEngine.UI.Toggle _plustoggle;
        public static event EosHierachItemEventHandler HierachButton;
        public override bool IsSelected
        {
            get => base.IsSelected;
            set
            {
                base.IsSelected = value;
                if (value)
                {
                    _hovertoggle.isOn = false;
                    _plustoggle.isOn = false;
                }
            }
        }
        public bool isHover
        {
            set
            {
                _ishover = value;
                if (value)
                {
                    if (IsSelected)
                        _plustoggle.isOn = value;
                    else
                    {
                        _hovertoggle.isOn = value;
                        _plustoggle.isOn = value;
                    }
                }
                else
                {
                    _hovertoggle.isOn = value;
                    _plustoggle.isOn = value;
                }
            }
        }
        public void AddObject()
        {
            IsSelected = true;
            isHover = true;
            HierachButton?.Invoke(this);
        }
    }
}
