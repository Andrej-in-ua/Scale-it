using Services;
using UnityEngine;

namespace View.GameTable
{
    public class CardDragController
    {
        private const string CardViewSortingLayerNameDefault = "Default";
        private const string CardViewSortingLayerNameDraggable = IDraggable.DraggableViewSortingLayerName;

        private readonly GridManager _gridManager;
        private readonly InputService _inputService;

        private Vector3 _originalPosition;

        public CardDragController(GridManager gridManager, InputService inputService)
        {
            _gridManager = gridManager;
            _inputService = inputService;
        }
        
        public void Construct()
        {
            _inputService.OnStartDrag += HandleStartDrag;
            _inputService.OnDrag += HandleDrag;
            _inputService.OnStopDrag += HandleStopDrag;
        }

        private bool HandleStartDrag(IDraggable draggable, Vector3 worldPosition, Vector3 hitPoint)
        {
            if (draggable is not CardView cardView) return false;

            _originalPosition = cardView.transform.position;

            cardView.SortingGroup.sortingLayerName = CardViewSortingLayerNameDraggable;
            cardView.transform.SetPositionAndRotation(worldPosition, Quaternion.identity);

            return true;
        }

        private void HandleDrag(IDraggable draggable, Vector3 worldPosition)
        {
            if (draggable is not CardView cardView) return;

            cardView.transform.SetPositionAndRotation(worldPosition, Quaternion.identity);
        }

        private void HandleStopDrag(IDraggable draggable, Vector3 worldPosition)
        {
            if (draggable is not CardView cardView) return;
            
            cardView.SortingGroup.sortingLayerName = CardViewSortingLayerNameDefault;

            var placedPosition = _gridManager.PlaceOnNearestAvailablePosition(worldPosition, cardView.PlaceableReference,
                out var needRelocate);
            
            cardView.transform.SetPositionAndRotation(placedPosition.GetValueOrDefault(_originalPosition), Quaternion.identity);
            _originalPosition = default;
        }

        public void Destruct()
        {
            if (_inputService != null)
            {
                _inputService.OnStartDrag -= HandleStartDrag;
                _inputService.OnDrag -= HandleDrag;
                _inputService.OnStopDrag -= HandleStopDrag;
            }
        }
    }
}