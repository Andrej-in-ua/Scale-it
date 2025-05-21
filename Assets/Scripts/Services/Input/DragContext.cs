using System.Collections.Generic;
using UnityEngine;

namespace Services.Input
{
    public record DragContext
    {
        public List<(IDraggable, Vector2)> Draggables;
        public Vector3 MouseWorldPosition;
    }
}