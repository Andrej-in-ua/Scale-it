using UnityEngine;

namespace Services.Input
{
    public record MouseContext
    {
        private readonly PlayerInputActions _inputActions;
        private readonly Camera _camera;

        public MouseContext(PlayerInputActions inputActions, Camera camera)
        {
            _inputActions = inputActions;
            _camera = camera;
        }

        public Vector2 GetMouseScreenPosition()
        {
            return _inputActions.Mouse.Move.ReadValue<Vector2>();
        }

        public Vector3 GetMouseWorldPosition()
        {
            var mouseWorldPosition = _camera.ScreenToWorldPoint(GetMouseScreenPosition());
            mouseWorldPosition.z = 0;

            return mouseWorldPosition;
        }

        public Ray GetMouseRay()
        {
            return _camera.ScreenPointToRay(GetMouseScreenPosition());
        }

        public float GetMouseScroll()
        {
            return Mathf.Sign(_inputActions.Mouse.Scroll.ReadValue<float>());
        }
    }
}