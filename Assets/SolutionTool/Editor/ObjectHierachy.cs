using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Eos.Objects.Editor;

public class ObjectHierachy : EditorWindow
{
    [MenuItem("Tools/Hierachy")]
    public static void ShowHierachy()
    {
        var wnd = GetWindow<ObjectHierachy>();
        wnd.titleContent = new GUIContent("Eos Hierachy");
    }
    //[FormerlySerializedAs("_EosObjectTreeViewState")] [FormerlySerializedAs("EosObjectTreeViewState")] 
    [SerializeField] TreeViewState _eosObjectTreeViewState;
    EosObjectTreeView _eosObjectTreeView;

    void OnEnable ()
    {
        // Check whether there is already a serialized view state (state 
        // that survived assembly reloading)
        if (_eosObjectTreeViewState == null)
            _eosObjectTreeViewState = new TreeViewState ();

        //var columns = new MultiColumnHeaderState.Column[2];
        //columns[0] = new MultiColumnHeaderState.Column { width = 100 };
        //columns[1] = new MultiColumnHeaderState.Column { width = 10 };
        //var multiculumnheaderstate = new MultiColumnHeaderState(columns);

        //var multicolumheader = new MultiColumnHeader(multiculumnheaderstate);

        //_eosObjectTreeView = new EosObjectTreeView(_eosObjectTreeViewState, multicolumheader);

        _eosObjectTreeView = new EosObjectTreeView(_eosObjectTreeViewState);
    }

    void OnGUI ()
    {
        GUI.changed = false;
        _eosObjectTreeView?.OnGUI(new Rect(0, 0, position.width, position.height));
        //var rows = _eosObjectTreeView.GetRows().ToArray();
        //if (GUI.changed)
        //    _eosObjectTreeView.Repaint();
    }
}
public class TOBJ
{
    public int ID;
    public string name;
    public List<TOBJ> _child;
}
public class EosObjectTreeView : TreeView
{
    private GUISkin _skin;
    public EosObjectTreeView(TreeViewState state) : base(state)
    {
        Reload();
        _skin = Resources.Load("TreeviewStyle") as GUISkin;
        showAlternatingRowBackgrounds = true;
    }
    public EosObjectTreeView(TreeViewState state, MultiColumnHeader multiColumnHeader) : base(state, multiColumnHeader)
    {
        Reload();
    }
    protected override void SelectionChanged(IList<int> selectedIds)
    {
        base.SelectionChanged(selectedIds);
        if (selectedIds.Count==1)
        {
            var wp = EditorWindow.GetWindow<ObjectInspector>();
            wp.SetTarget(selectedIds[0]);
            var wnd = EditorWindow.GetWindow<ObjectHierachy>();
            GUI.FocusWindow(wnd.GetInstanceID());
        }
    }
    protected override TreeViewItem BuildRoot()
    {
        var solution = Eos.Editor.SolutionEditor.EosSolution;
        var root = new  ObjectTreeviewItem(solution);
        var allItems = new List<TreeViewItem>();
        solution.IterChilds((child) =>
        {
            if (child != null)
            {
                allItems.Add(new ObjectTreeviewItem(child));
                child.CreatedOnEditor();
            }
        },true);

        // BuildRoot is called every time Reload is called to ensure that TreeViewItems 
        // are created from data. Here we create a fixed set of items. In a real world example,
        // a data model should be passed into the TreeView and the items created from the model.

        // This section illustrates that IDs should be unique. The root item is required to 
        // have a depth of -1, and the rest of the items increment from that.

        /*
        var root = new TreeViewItem {id = 0, depth = -1, displayName = "Root"};
        var allItems = new List<TreeViewItem>
        {
            new TreeViewItem {id = 1, depth = 0, displayName = "Animals"},
            new TreeViewItem {id = 2, depth = 1, displayName = "Mammals"},
            new TreeViewItem {id = 3, depth = 2, displayName = "Tiger"},
            new TreeViewItem {id = 4, depth = 2, displayName = "Elephant"},
            new TreeViewItem {id = 5, depth = 2, displayName = "Okapi"},
            new TreeViewItem {id = 6, depth = 2, displayName = "Armadillo"},
            new TreeViewItem {id = 7, depth = 1, displayName = "Reptiles"},
            new TreeViewItem {id = 8, depth = 2, displayName = "Crocodile"},
            new TreeViewItem {id = 9, depth = 2, displayName = "Lizard"},
        };
        */
        // Utility method that initializes the TreeViewItem.children and .parent for all items.
        SetupParentsAndChildrenFromDepths(root, allItems);

        // Return root of the tree
        return root;

    }

    private string k_GenericDragID="dd";
    private List<TreeViewItem> _draggingrows;
    public override void OnGUI(Rect rect)
    {
        GUI.skin = _skin;
        base.OnGUI(rect);
    }
    protected override void SetupDragAndDrop(SetupDragAndDropArgs args)
    {
        if (hasSearch)
            return;
 Debug.Log("Start #1");
        DragAndDrop.PrepareStartDrag();
        var draggedRows = _draggingrows = GetRows().Where(item => args.draggedItemIDs.Contains(item.id)).ToList();
        DragAndDrop.SetGenericData(k_GenericDragID, draggedRows);
        DragAndDrop.objectReferences = new UnityEngine.Object[] { }; // this IS required for dragging to work
        string title = draggedRows.Count == 1 ? draggedRows[0].displayName : "< Multiple >";
        DragAndDrop.StartDrag (title);    
    }
    protected override bool CanBeParent(TreeViewItem item)
    {
        return true;
    }
    protected override bool CanMultiSelect(TreeViewItem item)
    {
        return true;
    }
    protected override bool CanStartDrag(CanStartDragArgs args)
    {
        Debug.Log("Start #0");
        return true;//return base.CanStartDrag(args);
    }
    protected override DragAndDropVisualMode HandleDragAndDrop(DragAndDropArgs args)
    {
        if (args.performDrop)
        {
            var selections = new List<int>();
            foreach (var row in _draggingrows)
            {
                if (args.parentItem.hasChildren && args.parentItem.children.Contains(row))
                    continue;
                selections.Add(row.id);
                if (args.dragAndDropPosition == DragAndDropPosition.UponItem)
                {
                    row.parent.children.Remove(row);
                    if (!args.parentItem.hasChildren)
                        args.parentItem.AddChild(row);
                    else
                        args.parentItem.children.Add(row);
                    FrameItem(row.id);
                    SetExpanded(args.parentItem.id, true);
                    SetupDepthsFromParentsAndChildren(args.parentItem);
                }
                else 
                {
                    if (row.parent != null)
                    {
                        var oldparent = row.parent;
                        row.parent.children.Remove(row);
                        args.parentItem.children.Insert(args.insertAtIndex, row);
                        row.parent = args.parentItem;
                        SetupDepthsFromParentsAndChildren(args.parentItem);
                    }
                }
            }
            BuildRows(rootItem);
            SetSelection(selections);
            GUI.changed = true;
            return DragAndDropVisualMode.None;
        }
        return DragAndDropVisualMode.Move;
    }
    protected override void ContextClicked()
    {
        base.ContextClicked();
    }
    protected override void ContextClickedItem(int id)
    {
        base.ContextClickedItem(id);
        var gm = new GenericMenu();
        gm.AddItem(new GUIContent("Test menu"), false, () =>
          { 
          });
        gm.ShowAsContext();
        Repaint();
    }
    protected override void RowGUI(RowGUIArgs args)
    {
        var rect = args.rowRect;
       
        rect.x += this.depthIndentWidth * (args.item.depth+1);
        GUI.Label(rect, args.item.displayName,_skin.GetStyle("thumb"));
        if (args.focused)
        {
            rect.x += 200;
            rect.width = 16;
            if (GUI.Button(rect, "+"))
            {
                var gm = new GenericMenu();
                var typenames = SolutionEditorEditor.EosObjectNames;
                foreach (var tname in typenames)
                {
                    gm.AddItem(new GUIContent(tname), false, () =>
                    {
                    });
                }
                gm.ShowAsContext();
            }
        }
        //        base.RowGUI(args);
        //var rect = args.rowRect;
        //rect.x = args.rowRect.x + args.rowRect.width;
        //rect.width = 20;
        //GUI.Button(rect,"+");
    }
    //protected override DragAndDropVisualMode HandleDragAndDrop(DragAndDropArgs args)
    //{
    //    if (args.performDrop)
    //    {
    //        foreach(var row in _draggingrows)
    //        {
    //            if (row.parent != null)
    //            {
    //                row.parent.children.Remove(row);
    //                args.parentItem.children.Insert(args.insertAtIndex, row);
    //            }
    //        }
    //        GUI.changed = true;
    //        SetupDepthsFromParentsAndChildren(args.parentItem);
    //        this.FrameItem(args.parentItem.id);
    //    }
    //    return DragAndDropVisualMode.Move;
    //}
}
