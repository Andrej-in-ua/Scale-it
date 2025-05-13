using Services;
using UI.Game.Inventory;
using UnityEngine;

namespace GameTable
{
    public class DragCard : MonoBehaviour, IDraggable
    {
        public GameObject CardsCountWindow;
        public int Index;
        
        private UIInventory _inventory;
        private RectTransform _rectTransform;
        private GameObject _objectToDrag;

        private static readonly Vector2Int CardSize = new Vector2Int(5, 7);

        private void Start()
        {
            _rectTransform = GetComponent<RectTransform>();
            _inventory = FindObjectOfType<UIInventory>();
        }

        public void OnStartDrag()
        {
            if (_inventory.DoesCardHaveStack(this))
            {
                _objectToDrag = _inventory.TakeCardFromStack(this);
            }
            else
            {
                _objectToDrag = gameObject;
            }
        }

        public void OnDrag(Vector3 mousePosition)
        {
            _objectToDrag.transform.position = new Vector3(mousePosition.x, mousePosition.y, _rectTransform.position.z);
        }

        public void OnStopDrag(Vector3 mousePosition)
        {
            if (_inventory.IsCursorOnInventory())
            {
                _inventory.AddCardToInventory(_objectToDrag.GetComponent<DragCard>());
            }
        }
    }
}