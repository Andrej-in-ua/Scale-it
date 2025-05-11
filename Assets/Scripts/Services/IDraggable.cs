using UnityEngine;

namespace Services
{
    public interface IDraggable
    {
        const string DraggableViewSortingLayerName= "DraggableView";
        const string DraggableUISortingLayerName= "DraggableUI";

        public void OnStartDrag();
        public void OnDrag(Vector3 mousePosition);

        public void OnStopDrag(Vector3 mousePosition);
    }
}