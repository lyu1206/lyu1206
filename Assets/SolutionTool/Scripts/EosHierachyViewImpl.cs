using UnityEngine.EventSystems;
using Battlehub.UIControls;

namespace Battlehub.RTEditor
{

    public class EosHierachyViewImpl : HierarchyViewImpl
    {
        protected override void Awake()
        {
            base.Awake();
            VirtualizingItemContainer.PointerEnter += OnPointerEnter;
        }
        private void OnPointerEnter(VirtualizingItemContainer sender, PointerEventData eventData)
        {

        }
        protected override void OnTreeViewPointerEnter(object sender, PointerEventArgs e)
        {
            base.OnTreeViewPointerEnter(sender, e);
        }
        protected override void OnTreeViewPointerExit(object sender, PointerEventArgs e)
        {
            base.OnTreeViewPointerExit(sender, e);
        }
    }
}