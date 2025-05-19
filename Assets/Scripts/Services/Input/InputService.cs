using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Services.Input
{
    public class InputService : IDisposable
    {
        public event Action<MouseContext> OnMouseMove;
        public event Action<MouseContext> OnMouseLeftDown;
        public event Action<MouseContext> OnMouseLeftUp;

        private readonly PlayerInputActions _inputActions;

        private MouseContext _mouseContext;

        public InputService(PlayerInputActions inputActions)
        {
            _inputActions = inputActions;
        }

        public void Construct(Camera camera)
        {
            _mouseContext = new MouseContext(_inputActions, camera);

            _inputActions.Mouse.Move.performed += HandleMouseMove;
            _inputActions.Mouse.LeftButton.performed += HandleMouseDown;
            _inputActions.Mouse.LeftButton.canceled += HandleMouseUp;

            _inputActions.Mouse.Enable();
        }

        private void HandleMouseMove(InputAction.CallbackContext context) => OnMouseMove?.Invoke(_mouseContext);
        private void HandleMouseDown(InputAction.CallbackContext context) => OnMouseLeftDown?.Invoke(_mouseContext);
        private void HandleMouseUp(InputAction.CallbackContext context) => OnMouseLeftUp?.Invoke(_mouseContext);

        public void Dispose()
        {
            _mouseContext = null;

            if (_inputActions != null)
            {
                _inputActions.Mouse.Move.performed -= HandleMouseMove;
                _inputActions.Mouse.LeftButton.performed -= HandleMouseDown;
                _inputActions.Mouse.LeftButton.canceled -= HandleMouseUp;

                _inputActions.Mouse.Disable();
            }
        }
    }
}