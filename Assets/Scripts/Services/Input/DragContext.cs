using UnityEngine;

namespace Services.Input
{
    public record DragContext
    {
        public (IDraggable, Vector2)? Draggable;
        public Vector3 MouseWorldPosition;
    }
}