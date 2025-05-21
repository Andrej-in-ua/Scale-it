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
        public event Action<MouseContext> OnMouseScroll;

        public event Action<KeyboardContext> OnKeyboardMoveStart;
        public event Action<KeyboardContext> OnKeyboardMoveStop;

        private readonly PlayerInputActions _inputActions;

        private MouseContext _mouseContext;
        private KeyboardContext _keyboardContext;

        public InputService(PlayerInputActions inputActions)
        {
            _inputActions = inputActions;
        }

        public void Construct(Camera camera)
        {
            _mouseContext = new MouseContext(_inputActions, camera);
            _keyboardContext = new KeyboardContext(_inputActions);

            _inputActions.Mouse.Move.performed += HandleMouseMove;
            _inputActions.Mouse.LeftButton.performed += HandleMouseDown;
            _inputActions.Mouse.LeftButton.canceled += HandleMouseUp;
            _inputActions.Mouse.Scroll.performed += HandleMouseScroll;
            _inputActions.Mouse.Enable();

            _inputActions.Keyboard.Move.performed += HandleKeyboardMoveStarted;
            _inputActions.Keyboard.Move.canceled += HandleKeyboardMoveCanceled;
            _inputActions.Keyboard.Enable();
        }


        private void HandleMouseMove(InputAction.CallbackContext context) => OnMouseMove?.Invoke(_mouseContext);
        private void HandleMouseDown(InputAction.CallbackContext context) => OnMouseLeftDown?.Invoke(_mouseContext);
        private void HandleMouseUp(InputAction.CallbackContext context) => OnMouseLeftUp?.Invoke(_mouseContext);
        private void HandleMouseScroll(InputAction.CallbackContext context) => OnMouseScroll?.Invoke(_mouseContext);

        private void HandleKeyboardMoveStarted(InputAction.CallbackContext context) =>
            OnKeyboardMoveStart?.Invoke(_keyboardContext);

        private void HandleKeyboardMoveCanceled(InputAction.CallbackContext context) =>
            OnKeyboardMoveStop?.Invoke(_keyboardContext);

        public void Dispose()
        {
            _mouseContext = null;

            if (_inputActions != null)
            {
                _inputActions.Mouse.Move.performed -= HandleMouseMove;
                _inputActions.Mouse.LeftButton.performed -= HandleMouseDown;
                _inputActions.Mouse.LeftButton.canceled -= HandleMouseUp;
                _inputActions.Mouse.Scroll.performed -= HandleMouseScroll;
                _inputActions.Mouse.Disable();

                _inputActions.Keyboard.Move.performed -= HandleKeyboardMoveStarted;
                _inputActions.Keyboard.Move.canceled -= HandleKeyboardMoveCanceled;
                _inputActions.Keyboard.Disable();
            }
        }
    }
}