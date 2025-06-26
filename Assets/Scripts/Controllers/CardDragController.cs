using System;
using Services.Input;
using UI.Game.CardPreviews;
using UnityEngine;
using View.GameTable;

namespace Controllers
{
    public class CardDragController : IDisposable
    {
        public event Action<CardDragContext> OnStartDrag;
        public event Action<CardDragContext> OnDrag;
        public event Action<CardDragContext> OnChangeToPreview;
        public event Action<CardDragContext> OnChangeToView;
        public event Action<CardDragContext> OnStopDrag;
        public event Action<CardDragContext> OnRollback;

        private IDraggable _draggable;
        private Vector2 _localHitPoint;

        public void HandleMouseEnterInventory(Vector3 mouseWorldPosition)
        {
            if (_draggable != null)
                OnChangeToPreview?.Invoke(CreateCardDragContext(mouseWorldPosition));
        }

        public void HandleMouseLeaveInventory(Vector3 mouseWorldPosition)
        {
            if (_draggable != null)
                OnChangeToView?.Invoke(CreateCardDragContext(mouseWorldPosition));
        }

        public void HandleStartDrag(DragContext context)
        {
            if (_draggable != null || OnStartDrag == null)
                return;

            _draggable = context.Draggable;
            _localHitPoint = context.LocalHitPoint;

            OnStartDrag.Invoke(CreateCardDragContext(context.MouseWorldPosition));
        }

        public void HandleDrag(DragContext context)
        {
            if (_draggable == null) return;
            OnDrag?.Invoke(CreateCardDragContext(context.MouseWorldPosition));
        }

        public void HandleStopDrag(DragContext context)
        {
            if (_draggable == null) return;

            OnStopDrag?.Invoke(CreateCardDragContext(context.MouseWorldPosition));

            _draggable = null;
            _localHitPoint = Vector2.zero;
        }

        public void HandleCardDragRollback()
        {
            if (_draggable == null) return;

            OnRollback?.Invoke(CreateCardDragContext(Vector3.zero));

            _draggable = null;
            _localHitPoint = Vector2.zero;
        }

        private CardDragContext CreateCardDragContext(Vector3 mouseWorldPosition)
        {
            return new CardDragContext
            {
                Draggable = _draggable,
                LocalHitPoint = _localHitPoint,
                WorldMousePosition = mouseWorldPosition
            };
        }

        public void Dispose()
        {
            _draggable = null;
            _localHitPoint = Vector2.zero;
        }
    }
}