using System;
using DG.Tweening;
using Services.Input;
using UnityEngine;
using Zenject;

namespace Services
{
    public class CameraMover : ITickable, IDisposable
    {
        public event Action<Transform> OnCameraMove;

        private readonly InputService _inputService;
        private Camera _camera;

        private bool _isMoving;
        private Vector2 _moveVelocity;
        private Vector2 _moveDirection;

        private Tween _zoomTween;
        private float _zoomTarget;

        public CameraMover(InputService inputService)
        {
            _inputService = inputService;
        }

        public void Construct(Camera camera)
        {
            _camera = camera;

            _isMoving = false;
            _moveDirection = Vector2.zero;

            _zoomTarget = _camera.orthographicSize;
            _zoomTarget -= _zoomTarget % Constants.CameraSettings.ZoomStep;

            _inputService.OnKeyboardMoveStart += HandleKeyboardMoveStart;
            _inputService.OnKeyboardMoveStop += HandleKeyboardMoveStop;
            _inputService.OnMouseScroll += HandleMouseScroll;
        }

        #region Camera Scroll

        public void Tick()
        {
            if (!_isMoving) return;

            var zoomFactor = Mathf.InverseLerp(
                Constants.CameraSettings.ZoomMin,
                Constants.CameraSettings.ZoomMax,
                _zoomTarget
            );

            var dynamicMaxSpeed = Mathf.Lerp(
                Constants.CameraSettings.MoveMinSpeed,
                Constants.CameraSettings.MoveMaxSpeed,
                zoomFactor
            );

            var accelMagnitude = (_moveDirection == Vector2.zero)
                ? dynamicMaxSpeed / Constants.CameraSettings.MoveDecelerationDuration
                : dynamicMaxSpeed / Constants.CameraSettings.MoveAccelerationDuration;

            var targetSpeed = _moveDirection * dynamicMaxSpeed;

            _moveVelocity = Vector2.MoveTowards(
                _moveVelocity,
                targetSpeed,
                accelMagnitude * Time.deltaTime
            );

            if (_moveVelocity.sqrMagnitude > 0.0001f)
            {
                _camera.transform.position += new Vector3(
                    _moveVelocity.x * Time.deltaTime,
                    _moveVelocity.y * Time.deltaTime,
                    0f
                );

                OnCameraMove?.Invoke(_camera.transform);
            }
            else
            {
                _isMoving = false;
            }
        }

        private void HandleKeyboardMoveStart(KeyboardContext context)
        {
            _moveDirection = context.GetMoveDirection();
            _isMoving = true;
        }

        private void HandleKeyboardMoveStop(KeyboardContext obj)
        {
            _moveDirection = Vector2.zero;
        }

        #endregion

        #region Camera Zoom

        private void HandleMouseScroll(MouseContext context)
        {
            var scrollValue = context.GetMouseScroll();
            if (Mathf.Abs(scrollValue) > 0.01f)
            {
                _zoomTarget = Mathf.Clamp(
                    _zoomTarget - (scrollValue * Constants.CameraSettings.ZoomStep),
                    Constants.CameraSettings.ZoomMin,
                    Constants.CameraSettings.ZoomMax
                );

                _zoomTween?.Kill();

                _zoomTween = _camera
                    .DOOrthoSize(_zoomTarget, Constants.CameraSettings.ZoomDuration)
                    .OnUpdate(() => OnCameraMove?.Invoke(_camera.transform));
            }
        }

        #endregion

        public void Dispose()
        {
            _zoomTween?.Kill();
            _zoomTween = null;

            if (_inputService != null)
            {
                _inputService.OnKeyboardMoveStart -= HandleKeyboardMoveStart;
                _inputService.OnKeyboardMoveStop -= HandleKeyboardMoveStop;
                _inputService.OnMouseScroll -= HandleMouseScroll;
            }
        }
    }
}