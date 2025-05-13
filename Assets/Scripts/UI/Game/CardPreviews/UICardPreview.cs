using System;
using GameTable;
using Services;
using TMPro;
using UI.Game.Inventory;
using UnityEngine;

namespace UI.Game.CardPreviews
{
    public class UICardPreview : MonoBehaviour, IDraggable
    {
        public TMP_Text Name;
        
        public GameObject CardsCountWindow;
        public int CardId;
        
        private UIInventory _inventory;
        private RectTransform _rectTransform;
        private GameObject _objectToDrag;
        
        public event Action<UICardPreview> OnStartDragCardPreview;

        private static readonly Vector2Int CardSize = new Vector2Int(5, 7);
        
        public void Construct(UIInventory inventory)
        {
            _inventory = inventory;
            _rectTransform = GetComponent<RectTransform>();
        }

        public void OnStartDrag()
        {
            if (_inventory.DoesCardHaveStack(this))
            {
                //_objectToDrag = _inventory.TakeCardFromStack(this);
                OnStartDragCardPreview?.Invoke(this);
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
                _inventory.AddCardToInventory(_objectToDrag.GetComponent<UICardPreview>());
            }
        }
    }
}