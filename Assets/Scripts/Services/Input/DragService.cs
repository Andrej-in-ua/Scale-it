using System;
using System.Collections.Generic;
using ECS.Components;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using View.GameTable;

namespace Services.Input
{
    public class DragService : IDisposable
    {
        public event Action<DragContext> OnStartDrag;
        public event Action<DragContext> OnDrag;
        public event Action<DragContext> OnStopDrag;

        private readonly InputService _inputService;
        private readonly GridManager _gridManager;

        private List<(IDraggable, Vector2)> _draggables;
        
        private Vector3 _dragStartPosition;

        public DragService(InputService inputService, GridManager gridManager)
        {
            _inputService = inputService;
            _gridManager = gridManager;
        }

        public void Construct()
        {
            _inputService.OnMouseLeftDown += HandleMouseLeftDown;
            _inputService.OnMouseLeftUp += HandleMouseLeftUp;
            _inputService.OnMouseMove += HandleMouseMove;
        }

        private void HandleMouseLeftDown(MouseContext mouseContext)
        {
            if (_draggables != null || OnStartDrag == null) return;

            _draggables = new List<(IDraggable, Vector2)>();
            // ReSharper disable once Unity.PreferNonAllocApi
            var hits = Physics2D.GetRayIntersectionAll(mouseContext.GetMouseRay(), Mathf.Infinity);

            foreach (var hit in hits)
            {
                IDraggable draggable = hit.collider?.GetComponent<IDraggable>();
                // Debug.Log("Dragger hit: " + draggable + " / " + hit.point + " / " + hit.distance);
                if (draggable != null)
                    _draggables.Add((draggable, hit.point - (Vector2)hit.transform.position));
            }
            
            _dragStartPosition = new Vector2(mouseContext.GetMouseWorldPosition().x, mouseContext.GetMouseWorldPosition().y);

            OnStartDrag.Invoke(CreateDragContext(mouseContext));
        }

        private void HandleMouseMove(MouseContext mouseContext)
        {
            if (_draggables == null || OnDrag == null) return;
            
            //Example of pathfinder request
            var startCellPosition = _gridManager.WorldToCell(_dragStartPosition);
            var endCellPosition = _gridManager.WorldToCell(new Vector2(mouseContext.GetMouseWorldPosition().x, mouseContext.GetMouseWorldPosition().y));
            
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var entity = entityManager.CreateEntity();
            entityManager.AddComponentData(entity, new PathRequest
            {
                Start = new int2(startCellPosition.x, startCellPosition.y),
                End = new int2(endCellPosition.x, endCellPosition.y)
            });
            entityManager.AddBuffer<PathResult>(entity);
            //

            OnDrag.Invoke(CreateDragContext(mouseContext));
        }

        private void HandleMouseLeftUp(MouseContext mouseContext)
        {
            if (_draggables == null || OnStopDrag == null) return;

            OnStopDrag.Invoke(CreateDragContext(mouseContext));
            _draggables = null;
        }

        private DragContext CreateDragContext(MouseContext mouseContext)
        {
            return new DragContext
            {
                Draggables = _draggables,
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
