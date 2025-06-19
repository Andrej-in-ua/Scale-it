using System;
using UnityEngine;

namespace Services.Input
{
    public class DragService : IDisposable
    {
        public event Action<DragContext> OnStartDrag;
        public event Action<DragContext> OnDrag;
        public event Action<DragContext> OnStopDrag;

        private readonly InputService _inputService;

        private IDraggable _draggable;
        private Vector2 _localHitPoint;

        public DragService(InputService inputService)
        {
            _inputService = inputService;
        }

        public void Construct()
        {
            _inputService.OnMouseLeftDown += HandleMouseLeftDown;
            _inputService.OnMouseLeftUp += HandleMouseLeftUp;
            _inputService.OnMouseMove += HandleMouseMove;
        }

        private void HandleMouseLeftDown(MouseContext mouseContext)
        {
            if (_draggable != null || OnStartDrag == null) return;

            // ReSharper disable once Unity.PreferNonAllocApi
            var hits = Physics2D.GetRayIntersectionAll(mouseContext.GetMouseRay(), Mathf.Infinity);

            foreach (var hit in hits)
            {
                IDraggable draggable = hit.collider?.GetComponent<IDraggable>();
                
                if (draggable == null) continue;

                if (_draggable == null || draggable.Priority > _draggable.Priority)
                {
                    _draggable = draggable;
                    _localHitPoint = hit.point - (Vector2)hit.transform.position;
                }
            }

            if (_draggable != null)
                OnStartDrag.Invoke(CreateDragContext(mouseContext));
        }

        private void HandleMouseMove(MouseContext mouseContext)
        {
            if (_draggable == null || OnDrag == null) return;

            OnDrag.Invoke(CreateDragContext(mouseContext));
        }

        private void HandleMouseLeftUp(MouseContext mouseContext)
        {
            if (_draggable == null || OnStopDrag == null) return;

            OnStopDrag.Invoke(CreateDragContext(mouseContext));
            _draggable = null;
            _localHitPoint = Vector2.zero;
        }

        private DragContext CreateDragContext(MouseContext mouseContext)
        {
            return new DragContext
            {
                Draggable = _draggable,
                LocalHitPoint = _localHitPoint,
                MouseWorldPosition = mouseContext.GetMouseWorldPosition()
            };
        }

        public void Dispose()
        {
            if (_inputService is not null)
            {
                _inputService.OnMouseLeftDown -= HandleMouseLeftDown;
                _inputService.OnMouseLeftUp -= HandleMouseLeftUp;
                _inputService.OnMouseMove -= HandleMouseMove;
            }
        }
    }
}