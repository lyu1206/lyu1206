using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Battlehub.UIControls;

namespace Battlehub.RTEditor
{
    using RTCommon;
    using UIControls.MenuControl;
    public class EosHierachyViewImpl : HierarchyViewImpl
    {
        private EosVirtualizingTreeViewItem _hoveritem;
        private bool _focused;
        protected override void Awake()
        {
            base.Awake();
            VirtualizingItemContainer.PointerEnter += OnPointerEnter;
            VirtualizingItemContainer.PointerExit += OnPointerExit;
            EosVirtualizingTreeViewItem.HierachButton += HierachyItemButton;
            EosWorkSpace.SolutionChanged += SolutionChanged;
        }
        void SolutionChanged(Eos.Service.Solution solution)
        {
            solution.CreatedOnEditor();
            solution.IterChilds((child) =>
            {
                //                    child.CreatedOnEditor();
                child.RTEOnCreated(Editor);
            }, true);
        }
        private void HierachyItemButton(VirtualizingItemContainer sender)
        {
            Debug.Log("Show popup");
            IContextMenu menu = IOC.Resolve<IContextMenu>();
            List<MenuItemInfo> menuItems = new List<MenuItemInfo>();

            OnContextMenu(menuItems);

            menu.Open(menuItems.ToArray());
        }
        protected virtual void OnContextMenu(List<MenuItemInfo> menuItems)
        {
            MenuItemInfo duplicate = new MenuItemInfo { Path = "Duplicate" };
            duplicate.Action = new MenuItemEvent();
            duplicate.Action.AddListener(DuplicateContextMenuCmd);
            duplicate.Validate = new MenuItemValidationEvent();
            duplicate.Validate.AddListener(DuplicateValidateContextMenuCmd);
            menuItems.Add(duplicate);

            MenuItemInfo delete = new MenuItemInfo { Path = "Delete"};
            delete.Action = new MenuItemEvent();
            delete.Action.AddListener(DeleteContextMenuCmd);
            delete.Validate = new MenuItemValidationEvent();
            delete.Validate.AddListener(DeleteValidateContextMenuCmd);
            menuItems.Add(delete);

            MenuItemInfo rename = new MenuItemInfo { Path = "Rename" };
            rename.Action = new MenuItemEvent();
            rename.Action.AddListener(RenameContextMenuCmd);
            rename.Validate = new MenuItemValidationEvent();
            rename.Validate.AddListener(RenameValidateContextMenuCmd);
            menuItems.Add(rename);
        }

        private void OnPointerEnter(VirtualizingItemContainer sender, PointerEventData eventData)
        {
            _hoveritem = sender as EosVirtualizingTreeViewItem;
            if (_hoveritem != null)
                _hoveritem.isHover = true;
        }
        private void OnPointerExit(VirtualizingItemContainer sender, PointerEventData eventData)
        {
            if (_hoveritem != null)
                _hoveritem.isHover = false;
        }
    }
}