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
        private float _pathRequestCooldown = 0.1f;
        private float _lastPathRequestTime;
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

            _dragStartPosition = new Vector2(mouseContext.GetMouseWorldPosition().x, mouseContext.GetMouseWorldPosition().y);
            
            var startCellPosition = _gridManager.WorldToCell(_dragStartPosition);
            var endCellPosition = _gridManager.WorldToCell(new Vector2(mouseContext.GetMouseWorldPosition().x, mouseContext.GetMouseWorldPosition().y));
            
            _activePathRequestEntity  = _entityManager.CreateEntity();
            _entityManager.AddComponentData(_activePathRequestEntity, new PathRequest
            {
                Start = new int2(startCellPosition.x, startCellPosition.y),
                End = new int2(endCellPosition.x, endCellPosition.y)
            });
            _entityManager.AddBuffer<PathResult>(_activePathRequestEntity);

            if (_draggable != null)
                OnStartDrag.Invoke(CreateDragContext(mouseContext));
        }

        private void HandleMouseMove(MouseContext mouseContext)
        {
            if (_draggable == null || OnDrag == null) return;
            
            if (Time.time - _lastPathRequestTime >= _pathRequestCooldown && _entityManager.HasComponent<PathRequest>(_activePathRequestEntity))
            {
                _lastPathRequestTime = Time.time;
                
                var startCellPosition = _gridManager.WorldToCell(_dragStartPosition);
                var endCellPosition = _gridManager.WorldToCell(new Vector2(mouseContext.GetMouseWorldPosition().x, mouseContext.GetMouseWorldPosition().y));
            
                _entityManager.SetComponentData(_activePathRequestEntity, new PathRequest
                {
                    Start = new int2(startCellPosition.x, startCellPosition.y),
                    End = new int2(endCellPosition.x, endCellPosition.y)
                });
                _entityManager.AddBuffer<PathResult>(_activePathRequestEntity);
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