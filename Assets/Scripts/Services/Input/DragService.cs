using System;
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

        private IDraggable _draggable;
        private Vector2 _localHitPoint;

        private Vector3 _dragStartPosition;
        private Entity _activePathRequestEntity;
        private EntityManager _entityManager;

        public DragService(InputService inputService, GridManager gridManager)
        {
            _inputService = inputService;
            _gridManager = gridManager;
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
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

            _dragStartPosition = mouseContext.GetMouseWorldPosition();

            _activePathRequestEntity = _entityManager.CreateEntity();
            _entityManager.AddBuffer<PathResult>(_activePathRequestEntity); 

            if (_draggable != null)
                OnStartDrag.Invoke(CreateDragContext(mouseContext));
        }

        private void HandleMouseMove(MouseContext mouseContext)
        {
            if (_draggable == null || OnDrag == null) return;

             var startCell = _gridManager.WorldToCell(_dragStartPosition);
             var endCell = _gridManager.WorldToCell(mouseContext.GetMouseWorldPosition());
            
             if (!startCell.Equals(endCell))
             {
                 var request = new PathRequest
                 {
                     Start = new int2(startCell.x, startCell.y),
                     End = new int2(endCell.x, endCell.y)
                 };
                 
                 if (!_entityManager.HasComponent<PathRequest>(_activePathRequestEntity))
                     _entityManager.AddComponentData(_activePathRequestEntity, request);
                 else
                     _entityManager.SetComponentData(_activePathRequestEntity, request);
            
                 var buffer = _entityManager.GetBuffer<PathResult>(_activePathRequestEntity);
                 buffer.Clear();
             }

            OnDrag.Invoke(CreateDragContext(mouseContext));
        }

        private void HandleMouseLeftUp(MouseContext mouseContext)
        {
            if (_draggable == null || OnStopDrag == null) return;

            if (_entityManager.Exists(_activePathRequestEntity))
                _entityManager.DestroyEntity(_activePathRequestEntity);

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
