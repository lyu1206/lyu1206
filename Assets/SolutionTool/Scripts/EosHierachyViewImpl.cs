using System;
using System.Collections;
using System.Linq;
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
    public class AttributeCaches
    {
        //이건 당장 쓸것이 아니다.다만 미래에 쓸수도 있을지 몰라 예제삼아 넣어 본다.
        private static Type[] _eosobjecttypes;
        public static void Initialize()
        {
            var type = typeof(EosObjectBase);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsClass && type.IsAssignableFrom(x))
                .Where(x =>
                {
                    var gg = x;
                    return x.GetCustomAttributes(false).Where(t => t is EosObjectAttribute).ToArray().Length > 0;
                });
            _eosobjecttypes = types.ToArray();
        }
        public static string[] GetAvailableTypeNames(EosObjectBase obj)
        {
            Type targettype = obj.GetType();
            var parent = obj.Parent;
            var parentadaptchild = parent.GetType().GetCustomAttributes(false).Where(x => x.GetType() == typeof(NoChild)).Count();
            if (parentadaptchild != 0)
                return null;
            var availabletype = new List<Type>();
            var ssssss = _eosobjecttypes.Select(t => t).ToArray();
            var objtype = obj.GetType();
            Console.WriteLine(ssssss.Length);
            var list = _eosobjecttypes.Select(t => t).Where(it =>
            {
                var creationattributes = it.GetCustomAttributes(false).Where(d => d is CreationAttribute);
                var creationattributescount = creationattributes.Count();
                if (creationattributescount == 0)
                    return true;

                var tt = creationattributes
                .Where(at => 
                {
                    var attribute = at as CreationAttribute;
                    return attribute.CanCreate(obj);
                }).ToArray();
                if (tt.Length > 0)
                    return true;
                return false;
            }).Select(x => x.Name);
            var result = list.ToArray();
            return result;
        }
    }
    public class EosHierachyViewImpl : HierarchyViewImpl
    {
        private EosVirtualizingTreeViewItem _hoveritem;
        private EosObjectBase _solution;
        private EosObjectBase _currentSelect;
        private EosVirtualizingTreeViewItem _currentSelectItem;
        private bool _focused;
        private Dictionary<int, EosObjectBase> _objecttoExpose = new Dictionary<int, EosObjectBase>();
        protected override void Awake()
        {
            base.Awake();
            VirtualizingItemContainer.PointerEnter += OnPointerEnter;
            VirtualizingItemContainer.PointerExit += OnPointerExit;
            EosVirtualizingTreeViewItem.HierachyButton += HierachyItemButton;
            EosWorkspace.SolutionChanged += SolutionChanged;
            AttributeCaches.Initialize();
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
        protected override void OnItemDoubleClicked(object sender, ItemArgs e)
        {
            base.OnItemDoubleClicked(sender, e);
            ExposeToEosEditor exposeToEditor = (ExposeToEosEditor)e.Items[0];
            exposeToEditor.Owner.DoubleClicked();
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
            if (_currentSelect == null)
                return;
            var newobject = ObjectFactory.CreateInstance($"Eos.Objects.{arg}");
            newobject.Name = arg;
            newobject.OnCreate();
            _currentSelect.AddChildEditorImpl(newobject);
            _objecttoExpose[newobject.EditorTrasnform.GetHashCode()] = newobject;
            newobject.RTEOnCreated(Editor);
            _hoveritem.isHover = false;
            TreeView.Expand(_currentSelect.RTEditObject);
            TreeView.GetTreeViewItem(newobject.RTEditObject).IsSelected = true;

        }
        private void AddObjectValidateContextMenuCmd(MenuItemValidationArgs args)
        {
            args.IsValid = true;
        }

        protected override void OnContextMenu(List<MenuItemInfo> menuItems)
        {
            var types = AttributeCaches.GetAvailableTypeNames(_currentSelect);
            foreach (var type in types)
            {
                MenuItemInfo duplicate = new MenuItemInfo { Path = type };
                duplicate.Action = new MenuItemEvent();
                duplicate.Command = type;
                duplicate.Action.AddListener(AddObjectContextMenuCmd);
                duplicate.Validate = new MenuItemValidationEvent();
                duplicate.Validate.AddListener(AddObjectValidateContextMenuCmd);
                menuItems.Add(duplicate);
            }

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
        protected override void OnTreeViewPointerEnter(object sender, PointerEventArgs e)
        {
            base.OnTreeViewPointerEnter(sender, e);
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
        protected override void OnSelectionChanged(object sender, SelectionChangedArgs e)
        {
            base.OnSelectionChanged(sender, e);

            var selectitem = e.NewItem as ExposeToEosEditor;
            if (selectitem == null)
                return;
            _currentSelect = selectitem.Owner;
            _currentSelectItem = TreeView.GetItemContainer(e.NewItem) as EosVirtualizingTreeViewItem;
        }
        protected override void OnItemBeginEdit(object sender, VirtualizingTreeViewItemDataBindingArgs e)
        {
            base.OnItemBeginEdit(sender, e);
            TMP_InputField inputField = e.EditorPresenter.GetComponentInChildren<TMP_InputField>(true);
            inputField.onEndEdit.AddListener(OnEndEditName);
        }
        protected override void OnItemsRemoved(object sender, ItemsRemovedArgs e)
        {
            base.OnItemsRemoved(sender, e);
            foreach (var it in e.Items)
            {
                var item = it as ExposeToEditor;
                var obj = GetObjectWithTreeviewItem(it);
                _objecttoExpose.Remove(item.transform.GetHashCode());
                obj?.Destroy();
            }
        }
        //protected override void OnItemRemoving(object sender, ItemsCancelArgs e)
        //{
        //    base.OnItemRemoving(sender, e);
        //    foreach (var it in e.Items)
        //    {
        //        var item = it as ExposeToEditor;
        //        var obj = GetObjectWithTreeviewItem(it);
        //        _objecttoExpose.Remove(item.transform.GetHashCode());
        //        obj?.Destroy();
        //    }
        //}
        private void OnEndEditName(string name)
        {
            _currentSelect.Name = name;
        }
        protected override void OnNameChanged(ExposeToEditor obj)
        {
            base.OnNameChanged(obj);
            OnEndEditName(obj.name);
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