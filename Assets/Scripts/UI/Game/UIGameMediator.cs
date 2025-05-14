using System;
using System.Linq;
using Controllers;
using DeckManager;
using UI.Game.CardPreviews;
using UI.Game.Inventory;
using UnityEngine;
using UnityEngine.InputSystem;
using View.GameTable;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace UI.Game
{
    public class UIGameMediator
    {
        public event Action<Vector3> OnMouseEnterInventory;
        public event Action<Vector3> OnMouseLeaveInventory;

        public event Action OnCardDragRollback;

        private readonly UICardFactory _uiCardFactory;
        private readonly UIGameFactory _uiFactory;
        private readonly PlayerInputActions _playerInputActions;

        private UIInventory _inventory;
        private Transform _inventoryPanel;
        private Camera _camera;

        private bool _isHoverInventory;

        private bool _isDragging;
        private bool _isTaken;
        private UICardPreview _draggableCardPreview;

        public UIGameMediator(
            UICardFactory uiCardFactory,
            UIGameFactory uiFactory,
            // TODO: move it to InputService
            PlayerInputActions playerInputActions
        )
        {
            _uiCardFactory = uiCardFactory;
            _uiFactory = uiFactory;
            _playerInputActions = playerInputActions;
        }

        public void ConstructUI()
        {
            ClearDragState();

            _camera = Camera.main;
            _inventory = _uiFactory.CreateInventory();
            _inventoryPanel = _inventory.transform.GetChild(0).transform;

            // TODO: Move it to mediator
            _playerInputActions.Mouse.Move.performed += HandleMouseMove;

            var keys = Deck.Instance.cards.Keys.ToList().GetRange(1, 4);
            for (int i = 0; i < 10; i++)
            {
                var card = _uiCardFactory.CreateUICard(keys[Random.Range(0, keys.Count)], _inventoryPanel);

                _inventory.Put(card);
            }
        }

        public void HandleMouseMove(InputAction.CallbackContext context)
        {
            var mouseWorldPosition = _camera.ScreenToWorldPoint(context.ReadValue<Vector2>());
            mouseWorldPosition.z = 0;

            var isCurrentHoverInventory = RectTransformUtility.RectangleContainsScreenPoint(
                _inventory._bottomPanel,
                context.ReadValue<Vector2>(),
                _camera
            );

            if (isCurrentHoverInventory && !_isHoverInventory)
            {
                Debug.Log("Enter inventory");
                _isHoverInventory = true;
                OnMouseEnterInventory?.Invoke(mouseWorldPosition);
            }
            else if (!isCurrentHoverInventory && _isHoverInventory)
            {
                Debug.Log("Leave inventory");
                _isHoverInventory = false;
                OnMouseLeaveInventory?.Invoke(mouseWorldPosition);
            }
        }

        public void HandleStartDrag(CardDragContext context)
        {
            _isDragging = true;

            if (context.Draggable is UICardPreview uiCardPreview)
            {
                if (!_inventory.Take(uiCardPreview.CardId))
                {
                    OnCardDragRollback?.Invoke();
                    return;
                }

                _isTaken = true;
                _draggableCardPreview = _uiCardFactory.CreateUICard(uiCardPreview.CardId, _inventoryPanel);
            }
            else
            {
                _isTaken = false;
                _draggableCardPreview = null;
            }
        }

        public void HandleDrag(CardDragContext context)
        {
            if (!_isDragging || _draggableCardPreview == null) return;

            var position = context.WorldMousePosition;
            position.z = 90;

            _draggableCardPreview.transform.SetPositionAndRotation(position, Quaternion.identity);
        }

        public void HandleChangeToPreview(CardDragContext context)
        {
            if (!_isDragging) return;

            if (_draggableCardPreview == null)
            {
                if (context.Draggable is not CardView cardView)
                    throw new Exception("Draggable is not CardView or UICardPreview: " +
                                        context.Draggable.GetType().Name);

                _draggableCardPreview = _uiCardFactory.CreateUICard(cardView.CardId, _inventoryPanel);
            }

            var position = context.WorldMousePosition;
            position.z = 90;

            _draggableCardPreview.transform.SetPositionAndRotation(position, Quaternion.identity);
            _draggableCardPreview.gameObject.SetActive(true);
        }

        public void HandleChangeToView(CardDragContext context)
        {
            if (!_isDragging || _draggableCardPreview == null) return;

            _draggableCardPreview.gameObject.SetActive(false);
        }

        public void HandleStopDrag(CardDragContext context)
        {
            if (!_isDragging) return;

            if (_draggableCardPreview != null && _draggableCardPreview.gameObject.activeSelf)
            {
                _inventory.Put(_draggableCardPreview);
                _draggableCardPreview = null;
            }

            ClearDragState();
        }

        public void HandleRollback(CardDragContext context)
        {
            if (!_isDragging) return;

            if (_isTaken)
            {
                _draggableCardPreview.gameObject.SetActive(true);
                _inventory.Put(_draggableCardPreview);
                _draggableCardPreview = null;
            }

            ClearDragState();
        }

        private void ClearDragState()
        {
            _isDragging = false;
            _isTaken = false;

            if (_draggableCardPreview != null)
            {
                Object.Destroy(_draggableCardPreview.gameObject);
                _draggableCardPreview = null;
            }
        }
    }
}