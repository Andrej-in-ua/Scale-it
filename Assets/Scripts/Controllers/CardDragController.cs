using System;
using Services;
using UI.Game.CardPreviews;

using UnityEngine;
using View.GameTable;

namespace Controllers
{
    public record CardDragContext
    {
        public IDraggable Draggable;
        public Vector2 LocalHitPoint;
        public Vector3 WorldMousePosition;
        public bool IsPreview;
    }

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

        private bool _isPreview;

        public void HandleMouseEnterInventory(Vector3 mouseWorldPosition)
        {
            _isPreview = true;

            if (_draggable != null)
                OnChangeToPreview?.Invoke(CreateCardDragContext(mouseWorldPosition));
        }

        public void HandleMouseLeaveInventory(Vector3 mouseWorldPosition)
        {
            _isPreview = false;

            if (_draggable != null)
                OnChangeToView?.Invoke(CreateCardDragContext(mouseWorldPosition));
        }

        public void HandleStartDrag(DragContext context)
        {
            if (_draggable != null || OnStartDrag == null) return;

            foreach (var (draggable, localHitPoint) in context.Draggables)
            {
                if (draggable is CardView or UICardPreview)
                {
                    _draggable = draggable;
                    _localHitPoint = localHitPoint;

                    OnStartDrag.Invoke(CreateCardDragContext(context.MouseWorldPosition));
                    return;
                }
            }
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
                WorldMousePosition = mouseWorldPosition,
                IsPreview = _isPreview
            };
        }
        
        public void Dispose()
        {
            _draggable = null;
            _localHitPoint = Vector2.zero;
        }
    }
}