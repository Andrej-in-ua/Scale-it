using System;
using UnityEngine;
using Services.Input;
using View.GameTable;

namespace Controllers
{
    public class PortDrawController : IDisposable
    {
        public event Action<PortDrawContext> OnStartDraw;

        // public event Action<PortDrawContext> OnDraw;
        //
        // public event Action<PortDrawContext> OnStopDraw;

        private IDraggable _draggable;
        private Vector2 _localHitPoint;

        public void HandleStartDraw(DragContext context)
        {
            // if (_draggable != null || OnStartDraw == null) return;

            foreach (var (draggable, localHitPoint) in context.Draggables)
            {
                if (draggable is PortView)
                {
                    Debug.Log("port");
                    _draggable = draggable;
                    _localHitPoint = localHitPoint;

                   // OnStartDraw.Invoke(CreatePortDrawContext(context.MouseWorldPosition));
                    return;
                }
            }
        }

        private PortDrawContext CreatePortDrawContext(Vector3 mouseWorldPosition)
        {
            return new PortDrawContext
            {
                Draggable = _draggable,
                LocalHitPoint = _localHitPoint,
                WorldMousePosition = mouseWorldPosition
            };
        }
        
        public void Dispose()
        {
            _draggable = null;
            _localHitPoint = Vector2.zero;
        }
    }
}