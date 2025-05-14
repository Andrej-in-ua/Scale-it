using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Services
{
    public record DragContext
    {
        public List<(IDraggable, Vector2)> Draggables;
        public Vector3 MouseWorldPosition;
    }

    public class DragService : IDisposable
    {
        public event Action<DragContext> OnStartDrag;
        public event Action<DragContext> OnDrag;
        public event Action<DragContext> OnStopDrag;

        private readonly PlayerInputActions _inputActions;
        private Camera _camera;

        private List<(IDraggable, Vector2)> _draggables;

        public DragService(PlayerInputActions inputActions)
        {
            _inputActions = inputActions;
        }

        public void Construct(Camera camera)
        {
            _camera = camera;

            _inputActions.Mouse.LeftButton.performed += OnLeftMouseDown;
            _inputActions.Mouse.LeftButton.canceled += OnLeftMouseUp;
            _inputActions.Mouse.Move.performed += OnMouseMove;

            _inputActions.Mouse.Enable();
        }

        private void OnLeftMouseDown(InputAction.CallbackContext context)
        {
            if (_draggables != null || OnStartDrag == null) return;

            _draggables = new List<(IDraggable, Vector2)>();

            var ray = _camera.ScreenPointToRay(_inputActions.Mouse.Move.ReadValue<Vector2>());
            // ReSharper disable once Unity.PreferNonAllocApi
            var hits = Physics2D.GetRayIntersectionAll(ray, Mathf.Infinity);

            foreach (var hit in hits)
            {
                IDraggable draggable = hit.collider?.GetComponent<IDraggable>();
                // Debug.Log("Dragger hit: " + draggable + " / " + hit.point + " / " + hit.distance);
                if (draggable != null)
                {
                    _draggables.Add((draggable, hit.point - (Vector2)hit.transform.position));
                }
            }

            OnStartDrag.Invoke(CreateContext());
        }

        private void OnMouseMove(InputAction.CallbackContext context)
        {
            if (_draggables == null || OnDrag == null) return;

            OnDrag.Invoke(CreateContext());
        }

        private void OnLeftMouseUp(InputAction.CallbackContext context)
        {
            if (_draggables == null || OnStopDrag == null) return;

            OnStopDrag.Invoke(CreateContext());
            _draggables = null;
        }

        private DragContext CreateContext()
        {
            return new DragContext
            {
                Draggables = _draggables,
                MouseWorldPosition = MouseWorldPosition()
            };
        }

        private Vector3 MouseWorldPosition()
        {
            Vector3 worldPosition = _camera.ScreenToWorldPoint(_inputActions.Mouse.Move.ReadValue<Vector2>());
            worldPosition.z = 0;

            return worldPosition;
        }

        public void Dispose()
        {
            if (_camera is not null)
            {
                _camera = null;
            }

            if (_inputActions != null)
            {
                _inputActions.Mouse.LeftButton.performed -= OnLeftMouseDown;
                _inputActions.Mouse.LeftButton.canceled -= OnLeftMouseUp;
                _inputActions.Mouse.Move.performed -= OnMouseMove;
            }
        }
    }
}
