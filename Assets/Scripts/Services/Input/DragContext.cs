using UnityEngine;

namespace Services.Input
{
    public record DragContext
    {
        public IDraggable Draggable;
        public Vector2 LocalHitPoint;
        public Vector3 MouseWorldPosition;
    }
}