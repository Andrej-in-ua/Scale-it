using System;
using System.Collections.Generic;
using UnityEngine;

namespace Services
{
    public class InputService : MonoBehaviour
    {
        public event Func<IDraggable, Vector3, Vector3, bool> OnStartDrag;
        public event Action<IDraggable, Vector3> OnDrag;
        public event Action<IDraggable, Vector3> OnStopDrag;

        private IDraggable _draggable;

        private Camera _camera;

        private void Update()
        {
            if (!_camera)
            {
                _camera = Camera.main;
            }

            MouseDown();
            MouseDrag();
            MouseUp();
        }

        public void MouseDown()
        {
            if (_draggable != null || !Input.GetMouseButtonDown(0)) return;

            Vector3 mousePosition = Input.mousePosition;
            Vector3 worldPosition = ScreenToWorldPoint(mousePosition);

            foreach (var (target, hitPoint) in HitToDraggable(mousePosition))
            {
                foreach (Func<IDraggable, Vector3, Vector3, bool> handler in OnStartDrag.GetInvocationList())
                {
                    bool result = handler(target, worldPosition, hitPoint);
                    if (result)
                    {
                        _draggable = target;
                        return;
                    }
                }
            }
        }

        public void MouseDrag()
        {
            if (_draggable == null) return;

            Vector3 mousePosition = ScreenToWorldPoint(Input.mousePosition);
            OnDrag?.Invoke(_draggable, mousePosition);
        }

        public void MouseUp()
        {
            if (_draggable == null || !Input.GetMouseButtonUp(0)) return;

            OnStopDrag?.Invoke(_draggable, ScreenToWorldPoint(Input.mousePosition));
            _draggable = null;
        }

        private Vector3 ScreenToWorldPoint(Vector3 screenPosition)
        {
            Vector3 worldPosition = _camera.ScreenToWorldPoint(screenPosition);
            worldPosition.z = 0;

            return worldPosition;
        }

        private IEnumerable<(IDraggable, Vector3)> HitToDraggable(Vector3 mousePosition)
        {
            Ray ray = _camera.ScreenPointToRay(mousePosition);
            var hits = Physics2D.GetRayIntersectionAll(ray, Mathf.Infinity);

            foreach (var hit in hits)
            {
                IDraggable draggable = hit.collider?.GetComponent<IDraggable>();
                // Debug.Log("Dragger hit: " + draggable + " / " + hit.point + " / " + hit.distance);
                if (draggable != null)
                {
                    yield return (draggable, hit.point);
                }
            }
        }
    }
}