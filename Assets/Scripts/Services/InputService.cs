using GameTable;
using UnityEngine;

namespace Services
{
    public class InputService : MonoBehaviour
    {
        private Camera _camera;
        private DragCard _draggable;
        private bool _dragging = false;

        private void Update()
        {
            if (!_dragging && Input.GetMouseButtonDown(0)) TryStartDrag();
            if (_dragging) UpdateDrag();
        }

        void UpdateDrag()
        {
            Vector3 sp = Input.mousePosition;

            if (_draggable != null)
            {
                _draggable.Drag(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            }

           // Ray ray = Camera.main.ScreenPointToRay(sp);
            
            // bool overInventory = Physics2D.GetRayIntersection(ray, Mathf.Infinity);
            // if (!overInventory && ghost == null)
            // {
            //     var hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity, worldMask);
            //     Vector3 dropPos = hit ? hit.point : Camera.main.ScreenToWorldPoint(sp);
            //     dropPos.z = 0;
            //     ghost = Instantiate(cardPrefab, dropPos, Quaternion.identity);
            // }
        }
        
        void TryStartDrag()
        {
            Vector3 sp = Input.mousePosition;
            Ray ray    = Camera.main.ScreenPointToRay(sp);

            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);
            if (hit.collider == null)
                return;                         

            _draggable = hit.collider.GetComponent<DragCard>();
            if (_draggable == null)
                return;                        

            _dragging   = true;
            
            // currentIcon = draggable.gameObject;
            // startPos    = currentIcon.transform.position;
            //
            // currentIcon.transform.SetAsLastSibling();   
            // currentIcon.GetComponent<CanvasGroup>().blocksRaycasts = false;

            _draggable.OnBeginDrag();
        }
        
        private void Construct()
        {
            _camera = Camera.main;
        }
    }
}