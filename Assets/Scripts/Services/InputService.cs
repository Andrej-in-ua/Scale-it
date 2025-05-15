using GameTable;
using UnityEngine;

namespace Services
{
    public class InputService : MonoBehaviour
    {
        private IDraggable _draggable;

        private (IDraggable, Vector3) HitToDragable()
        {
            Vector3 mousePosition = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit2D[] hits = Physics2D.GetRayIntersectionAll(ray, Mathf.Infinity);

            foreach (var hit in hits)
            {
                if (hit.collider == null)
                    continue;

                IDraggable draggable = hit.collider.GetComponent<IDraggable>();
                if (draggable == null)
                    continue;

                return (draggable, mousePosition);
            }

            return default;
        }

        public void MouseDown()
        {
            if (_draggable == null)
            {
                var (hitTarget, mousePosition) = HitToDragable();

                if (hitTarget != null)
                {
                    hitTarget.OnStartDrag();
                    _draggable = hitTarget;
                }
            }
        }

        public void MouseDrag()
        {
            if (_draggable != null)
            {
                _draggable.OnDrag(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            }
        }

        public void MouseUp()
        {
            if (_draggable != null)
            {
                _draggable.OnStopDrag(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                _draggable = null;
            }
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0)) MouseDown();
            MouseDrag();
            if (Input.GetMouseButtonUp(0)) MouseUp();
        }
    }
}