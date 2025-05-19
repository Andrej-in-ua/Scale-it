using Services.Input;
using UnityEngine;

namespace Controllers
{
    public record CardDragContext
    {
        public IDraggable Draggable;
        public Vector2 LocalHitPoint;
        public Vector3 WorldMousePosition;
    }
}