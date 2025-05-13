using System;
using Services;
using UnityEngine;

namespace View.GameTable
{
    public class CardDragController
    {
        private const string CardViewSortingLayerNameDefault = "Default";
        private const string CardViewSortingLayerNameDraggable = IDraggable.DraggableViewSortingLayerName;

        private readonly GridManager _gridManager;
        private Vector3 _originalPosition;

        public CardDragController(GridManager gridManager)
        {
            _gridManager = gridManager;
        }

        public bool HandleStartDrag(IDraggable draggable, Vector3 worldPosition, Vector3 hitPoint)
        {
            if (draggable is not CardView cardView) return false;

            _originalPosition = cardView.transform.position;

            cardView.SortingGroup.sortingLayerName = CardViewSortingLayerNameDraggable;
            cardView.transform.SetPositionAndRotation(worldPosition, Quaternion.identity);

            return true;
        }

        public void HandleDrag(IDraggable draggable, Vector3 worldPosition)
        {
            if (draggable is not CardView cardView) return;

            cardView.transform.SetPositionAndRotation(worldPosition, Quaternion.identity);
        }

        public void HandleStopDrag(IDraggable draggable, Vector3 worldPosition)
        {
            if (draggable is not CardView cardView) return;
            
            cardView.SortingGroup.sortingLayerName = CardViewSortingLayerNameDefault;

            var placedPosition = _gridManager.PlaceOnNearestAvailablePosition(worldPosition, cardView.PlaceableReference,
                out var needRelocate);
            
            cardView.transform.SetPositionAndRotation(placedPosition.GetValueOrDefault(_originalPosition), Quaternion.identity);
            _originalPosition = default;
        }
    }
}