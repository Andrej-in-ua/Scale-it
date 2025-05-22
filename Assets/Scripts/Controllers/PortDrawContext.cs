using UnityEngine;
using Services.Input;

namespace Controllers
{
    public record PortDrawContext
    {
        public IDraggable Draggable;
        public Vector2 LocalHitPoint;
        public Vector3 WorldMousePosition;
    }
}