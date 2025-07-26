using System;
using System.Collections.Generic;
using System.Linq;
using Controllers;
using DeckManager;
using Services.Input;
using TMPro;
using UI.Game.CardPreviews;
using UI.Game.DebugTools;
using UI.Game.Inventory;
using UnityEngine;
using View.GameTable;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace UI.Game
{
    public class UIGameMediator : IDisposable
    {
        public event Action<Vector3> OnMouseEnterInventory;
        public event Action<Vector3> OnMouseLeaveInventory;

        public event Action OnCardDragRollback;

        private readonly UICardFactory _uiCardFactory;
        private readonly UIGameFactory _uiFactory;

        private UIInventory _inventory;
        private Transform _inventoryPanel;
        private Camera _camera;

        private CardSpawner _cardSpawner;
        private TMP_InputField _cardSearchInputField;
        private string _previousCardSearchInput;

        private bool _isHoverInventory;

        private bool _isDragging;
        private bool _isTaken;
        private UICardPreview _draggableCardPreview;

        public UIGameMediator(
            UICardFactory uiCardFactory,
            UIGameFactory uiFactory
        )
        {
            _uiCardFactory = uiCardFactory;
            _uiFactory = uiFactory;
        }

        public void ConstructUI()
        {
            ClearDragState();

            _camera = Camera.main;
            _inventory = _uiFactory.CreateInventory();
            _inventoryPanel = _inventory.transform.GetChild(0).transform;

            _cardSpawner = _uiFactory.CreateCardSpawner(_inventory.gameObject.transform);
            _cardSpawner.OnCardSpawnRequested += SpawnCard;
            _cardSearchInputField = _cardSpawner.InputField;
            _cardSearchInputField.onValueChanged.AddListener(_ => SearchCardsToSpawn());

            var keys = Deck.Instance.cards.Keys.ToList().GetRange(1, 4);
            for (int i = 0; i < 10; i++)
            {
                SpawnCard(keys[Random.Range(0, keys.Count)]);
            }
        }

        private void SearchCardsToSpawn()
        {
            Transform content = _cardSpawner.Content;
            GameObject scrollView = _cardSpawner.CardScrollView;
            string input = _cardSpawner.InputField.text.ToLower();

            if (input == _previousCardSearchInput) return;
            _previousCardSearchInput = input;

            if (string.IsNullOrEmpty(input))
            {
                scrollView.SetActive(false);
                
                foreach (Transform child in content)
                    Object.Destroy(child.gameObject);
                
                return;
            }

            var foundCards = Deck.Instance.cards.Values
                .Where(card =>
                    card.name.ToLower().Contains(input) ||
                    card.cardID.ToString().StartsWith(input))
                .ToList();

            foreach (Transform child in content)
                Object.Destroy(child.gameObject);
            
            scrollView.SetActive(foundCards.Count > 0);

            foreach (var card in foundCards)
                _uiFactory.CreateCardSearchResultPanel(content, _cardSpawner, card.cardID, card.name);
        }

        private void SpawnCard(int cardId)
        {
            var card = _uiCardFactory.CreateUICard(cardId, _inventoryPanel);
            _inventory.Put(card);
        }
        
        public bool IsCardSearchInputFocused()
        {
            return _cardSearchInputField.isFocused;
        }

        public void HandleMouseMove(MouseContext mouseContext)
        {
            var isCurrentHoverInventory = RectTransformUtility.RectangleContainsScreenPoint(
                _inventory._bottomPanel,
                mouseContext.GetMouseScreenPosition(),
                _camera
            );

            if (isCurrentHoverInventory && !_isHoverInventory)
            {
                Debug.Log("Enter inventory");
                _isHoverInventory = true;
                OnMouseEnterInventory?.Invoke(mouseContext.GetMouseWorldPosition());
            }
            else if (!isCurrentHoverInventory && _isHoverInventory)
            {
                Debug.Log("Leave inventory");
                _isHoverInventory = false;
                OnMouseLeaveInventory?.Invoke(mouseContext.GetMouseWorldPosition());
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

        public void Dispose()
        {
            if (_cardSpawner != null)
                _cardSpawner.OnCardSpawnRequested -= SpawnCard;
        }
    }
}