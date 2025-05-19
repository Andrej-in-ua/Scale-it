using InputActions;
using UnityEngine;

namespace Services.Input
{
    public record KeyboardContext
    {
        private readonly PlayerInputActions _inputActions;

        public KeyboardContext(PlayerInputActions inputActions)
        {
            _inputActions = inputActions;
        }

        public Vector2 GetMoveDirection()
        {
            return _inputActions.Keyboard.Move.ReadValue<Vector2>();
        }
    }
}