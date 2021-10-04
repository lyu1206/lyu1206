using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Battlehub.UIControls;

namespace Battlehub.RTEditor
{
    using Eos.Objects;
    using RTCommon;
    using UIControls.MenuControl;
    public class EosHierachyViewImpl : HierarchyViewImpl
    {
        private EosVirtualizingTreeViewItem _hoveritem;
        private EosObjectBase _solution;
        private EosObjectBase _currentSelect;
        private bool _focused;
        private Dictionary<int, EosObjectBase> _objecttoExpose = new Dictionary<int, EosObjectBase>();
        protected override void Awake()
        {
            base.Awake();
            VirtualizingItemContainer.PointerEnter += OnPointerEnter;
            VirtualizingItemContainer.PointerExit += OnPointerExit;
            EosVirtualizingTreeViewItem.HierachyButton += HierachyItemButton;
            EosWorkspace.SolutionChanged += SolutionChanged;
        }
        void SolutionChanged(Eos.Objects.EosObjectBase solution)
        {
            _solution = solution;
            //var treeview = TreeView;
            //treeview.Items = solution.Children;

            solution.CreatedOnEditor();

            solution.IterChilds((child) =>
            {
                child.RTEOnCreated(Editor);
                _objecttoExpose[child.EditorTrasnform.GetHashCode()] = child;
            }, true);
        }
        private EosObjectBase GetObjectWithTreeviewItem(object item)
        {
            if (item == null)
                return null;
            var viewitem = item as ExposeToEditor;
            var objid = viewitem.transform.GetHashCode();
            if (!_objecttoExpose.ContainsKey(objid))
                return null;
            return _objecttoExpose[objid];
        }
        protected override void OnItemDataBinding(object sender, VirtualizingTreeViewItemDataBindingArgs e)
        {
            base.OnItemDataBinding(sender, e);
            var eosobj = GetObjectWithTreeviewItem(e.Item);
            if (eosobj == null)
                return;
            if (eosobj.Parent == _solution)
            {
                e.CanDrag = false;
                e.CanEdit = false;
            }
        }
        private void HierachyItemButton(VirtualizingItemContainer sender)
        {
            Debug.Log("Show popup");
            IContextMenu menu = IOC.Resolve<IContextMenu>();
            List<MenuItemInfo> menuItems = new List<MenuItemInfo>();

            OnContextMenu(menuItems);

            menu.Open(menuItems.ToArray());
        }
        private void AddObjectContextMenuCmd(string arg)
        {
            //            base.RenameContextMenuCmd(arg);
            if (_currentSelect == null)
                return;
            var newobject = ObjectFactory.CreateInstance($"Eos.Objects.{arg}");
            newobject.Name = arg;
            _currentSelect.AddChildEditorImpl(newobject);
            TreeView.AddChild(_currentSelect, newobject);
            TreeView.Expand(_currentSelect);
            TreeView.GetTreeViewItem(newobject).IsSelected = true;
        }
        private void AddObjectValidateContextMenuCmd(MenuItemValidationArgs args)
        {
            args.IsValid = true;
            //            base.RenameValidateContextMenuCmd(args);
        }

        protected override void OnContextMenu(List<MenuItemInfo> menuItems)
        {
            MenuItemInfo duplicate = new MenuItemInfo { Path = "Add Object" };
            duplicate.Action = new MenuItemEvent();
            duplicate.Command = typeof(EosTransformActor).Name;
            duplicate.Action.AddListener(AddObjectContextMenuCmd);
            duplicate.Validate = new MenuItemValidationEvent();
            duplicate.Validate.AddListener(AddObjectValidateContextMenuCmd);
            menuItems.Add(duplicate);

            //MenuItemInfo delete = new MenuItemInfo { Path = "Delete"};
            //delete.Action = new MenuItemEvent();
            //delete.Action.AddListener(DeleteContextMenuCmd);
            //delete.Validate = new MenuItemValidationEvent();
            //delete.Validate.AddListener(DeleteValidateContextMenuCmd);
            //menuItems.Add(delete);

            //MenuItemInfo rename = new MenuItemInfo { Path = "Rename" };
            //rename.Action = new MenuItemEvent();
            //rename.Action.AddListener(RenameContextMenuCmd);
            //rename.Validate = new MenuItemValidationEvent();
            //rename.Validate.AddListener(RenameValidateContextMenuCmd);
            //menuItems.Add(rename);
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
        protected override void OnDestroy()
        {
            base.OnDestroy();
            VirtualizingItemContainer.PointerEnter -= OnPointerEnter;
            VirtualizingItemContainer.PointerExit -= OnPointerExit;
            EosVirtualizingTreeViewItem.HierachyButton -= HierachyItemButton;
        }
        protected override void RenameContextMenuCmd(string arg)
        {
//            base.RenameContextMenuCmd(arg);
        }
        protected override void RenameValidateContextMenuCmd(MenuItemValidationArgs args)
        {
//            base.RenameValidateContextMenuCmd(args);
        }
        protected override void OnSelectionChanged(object sender, SelectionChangedArgs e)
        {
            base.OnSelectionChanged(sender, e);
            _currentSelect = GetObjectWithTreeviewItem(e.NewItem);
        }
        protected override void OnItemBeginEdit(object sender, VirtualizingTreeViewItemDataBindingArgs e)
        {
            base.OnItemBeginEdit(sender, e);
            TMP_InputField inputField = e.EditorPresenter.GetComponentInChildren<TMP_InputField>(true);
            inputField.onEndEdit.AddListener(OnEndEditName);
        }
        private void OnEndEditName(string name)
        {
            _currentSelect.Name = name;
        }
        protected override void OnNameChanged(ExposeToEditor obj)
        {
            base.OnNameChanged(obj);
            _currentSelect.Name = obj.name;
        }
        protected override void OnParentChanged(ExposeToEditor obj, ExposeToEditor oldParent, ExposeToEditor newParent)
        {
            base.OnParentChanged(obj, oldParent, newParent);
            var child = GetObjectWithTreeviewItem(obj);
            if (child == null)
                return;
            var parent = GetObjectWithTreeviewItem(newParent);
            child.Parent = parent;
        }
    }
}