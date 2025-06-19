using System;
using Controllers;
using Services.Input;
using UI.Game.CardPreviews;
using UnityEngine;

namespace View.GameTable
{
    public class GameTableMediator
    {
        private const string CardViewSortingLayerNameDefault = "Default";
        private const string CardViewSortingLayerNameDraggable = IDraggable.DraggableViewSortingLayerName;

        public event Action OnCardDragRollback;

        private readonly GridManager _gridManager;
        private readonly CardViewPool _cardViewPool;
        private readonly ConnectionFactory _connectionFactory;

        private bool _isConstructed;

        private bool _isDragging;
        private Vector3 _originalPosition;
        private CardView _originalCardView;
        private CardView _createdCardView;
        private CardView _draggableCardView;
        
        private IDraggable _draggablePort;
        private int _portPriority = 2;

        private Transform _connectionsContainer;

        public GameTableMediator(
            GridManager gridManager,
            CardViewPool cardViewPool,
            ConnectionFactory connectionFactory
        )
        {
            _gridManager = gridManager;
            _cardViewPool = cardViewPool;
            _connectionFactory = connectionFactory;
        }

        public void ConstructGameTable()
        {
            _gridManager.Construct();
            _cardViewPool.Construct();
            _connectionsContainer = _connectionFactory.CreateConnectionsContainer();

            _isConstructed = true;

            int[,] map =
            {
                { 32100, 32100, 32100 },
                { 31210, 31211, 31211 },
                { 30100, 31100, 31100 },
                { 32100, 32100, 32100 }
            };

            for (var i = 0; i < 3; i++)
            {
                for (var j = 0; j < 3; j++)
                {
                    var card = _cardViewPool.GetCardView(map[i, j]);
                    // SnapCardToGridByWorldPosition(card, new Vector2Int(5, 7));
                    SnapCardToGridByWorldPosition(card, new Vector3(0, 0, 0));
                    card.gameObject.SetActive(true);
                }
            }
        }

        public void SnapCardToGridByWorldPosition(CardView cardView, Vector3 position)
        {
            AssertConstructed();

            var placedPosition = _gridManager.PlaceOnNearestAvailablePosition(
                position,
                cardView.PlaceableReference,
                out var needRelocate
            );

            if (placedPosition.HasValue)
            {
                cardView.transform.SetPositionAndRotation(placedPosition.Value, Quaternion.identity);
                // TODO: Relocate
            }
        }

        public void HandleStartDraw(DragContext context)
        {
            if (_draggablePort != null)
                return;

            IDraggable draggable = context.Draggable;

            if (draggable.Priority != _portPriority)
                return;
            
            _connectionFactory.CreateConnectionView(_connectionsContainer);

            _draggablePort = draggable;
        }

        public void HandleDraw(DragContext context)
        {
            if (_draggablePort == null) return;
            // pathfinding
        }
        
        public void HandleStopDraw(DragContext context)
        {
            _draggablePort = null;
            // pathfinding
        }
        
        public void HandleStartDrag(CardDragContext context)
        {
            _isDragging = true;

            if (context.Draggable is CardView cardView)
            {
                _originalPosition = cardView.transform.position;
                _originalCardView = cardView;
                _draggableCardView = _originalCardView;

                cardView.SortingGroup.sortingLayerName = CardViewSortingLayerNameDraggable;
                cardView.transform.SetPositionAndRotation(NormalizeWorldPosition(context), Quaternion.identity);
            }
            else
            {
                _originalPosition = Vector3.zero;
                _originalCardView = null;
                _draggableCardView = null;
            }
        }

        public void HandleDrag(CardDragContext context)
        {
            if (!_isDragging || !_draggableCardView) return;

            _draggableCardView.transform.SetPositionAndRotation(NormalizeWorldPosition(context), Quaternion.identity);
        }

        public void HandleChangeToPreview(CardDragContext context)
        {
            if (!_isDragging) return;

            if (_draggableCardView)
                HideCardView(_draggableCardView);

            _draggableCardView = null;
        }


        public void HandleChangeToView(CardDragContext context)
        {
            if (!_isDragging) return;

            if (_originalCardView != null)
            {
                _draggableCardView = _originalCardView;
            }
            else
            {
                if (_createdCardView == null)
                {
                    if (context.Draggable is not UICardPreview preview)
                        throw new Exception("Draggable is not UICardPreview or CardView");

                    _createdCardView = _cardViewPool.GetCardView(preview.CardId, true);
                    _createdCardView.SortingGroup.sortingLayerName = CardViewSortingLayerNameDraggable;
                }

                _draggableCardView = _createdCardView;
            }

            ShowCardView(_draggableCardView);
            _draggableCardView.transform.SetPositionAndRotation(context.WorldMousePosition, Quaternion.identity);
        }

        public void HandleStopDrag(CardDragContext context)
        {
            if (!_isDragging) return;

            if (_draggableCardView)
            {
                _draggableCardView.SortingGroup.sortingLayerName = CardViewSortingLayerNameDefault;
                var placedPosition = _gridManager.PlaceOnNearestAvailablePosition(
                    NormalizeWorldPosition(context),
                    _draggableCardView.PlaceableReference,
                    out var needRelocate
                );

                if (!placedPosition.HasValue)
                {
                    OnCardDragRollback?.Invoke();
                    return;
                }

                _draggableCardView.transform.SetPositionAndRotation(placedPosition.Value, Quaternion.identity);
                _draggableCardView.SortingGroup.sortingLayerName = CardViewSortingLayerNameDefault;
                ShowCardView(_draggableCardView);

                _originalCardView = null;
                _createdCardView = null;
            }
            else if (_originalCardView)
            {
                _gridManager.Release(_originalCardView.PlaceableReference);
            }

            ClearDragState();
        }

        public void HandleRollback(CardDragContext context)
        {
            if (!_isDragging) return;

            if (_originalCardView)
            {
                _originalCardView.transform.SetPositionAndRotation(_originalPosition, Quaternion.identity);
                _originalCardView.SortingGroup.sortingLayerName = CardViewSortingLayerNameDefault;
                ShowCardView(_originalCardView);
                _originalCardView = null;
            }

            ClearDragState();
        }

        private void ClearDragState()
        {
            if (_originalCardView)
            {
                ShowCardView(_originalCardView);
                _cardViewPool.ReturnCardView(_originalCardView);
            }

            if (_createdCardView)
            {
                ShowCardView(_createdCardView);
                _cardViewPool.ReturnCardView(_createdCardView);
            }

            _isDragging = false;
            _originalPosition = Vector3.zero;
            _originalCardView = null;
            _createdCardView = null;
            _draggableCardView = null;
        }

        private Vector3 NormalizeWorldPosition(CardDragContext context)
        {
            return _originalCardView != null
                ? context.WorldMousePosition - (Vector3)context.LocalHitPoint
                : context.WorldMousePosition;
        }

        private static void HideCardView(CardView cardView)
        {
            foreach (var renderer in cardView.GetComponentsInChildren<Renderer>())
            {
                renderer.enabled = false;
            }
        }

        private static void ShowCardView(CardView cardView)
        {
            foreach (var renderer in cardView.GetComponentsInChildren<Renderer>())
            {
                renderer.enabled = true;
            }
        }

        public void DestructGameTable()
        {
            _isConstructed = false;

            _cardViewPool.Destruct();
            _gridManager.Destruct();
        }

        private void AssertConstructed()
        {
            if (!_isConstructed)
                throw new Exception("GridView is not constructed");
        }
    }
}