using Services;
using UnityEngine;

namespace View.GameTable
{
    public class GameTableMediator
    {
        // TODO: Move in to DragController
        private const string CardViewSortingLayerNameDefault = "Default";
        private const string CardViewSortingLayerNameDraggable = IDraggable.DraggableViewSortingLayerName;

        private readonly GridFactory _gridFactory;
        private readonly CardViewPool _cardViewPool;
        private readonly InputService _inputService;

        private Grid _grid;

        public GameTableMediator(
            GridFactory gridFactory,
            CardViewPool cardViewPool,
            InputService inputService
        )
        {
            _gridFactory = gridFactory;
            _cardViewPool = cardViewPool;
            _inputService = inputService;
        }

        public void ConstructGameTable()
        {
            _inputService.OnStartDrag += HandleCardViewStartDrag;
            _inputService.OnDrag += HandleCardViewDrag;
            _inputService.OnStopDrag += HandleCardViewStopDrag;

            _grid = _gridFactory.Create();

            _cardViewPool.Construct();

            int[,] map =
            {
                { 32100, 32100, 32100 },
                { 31210, 31211, 31211 },
                { 30100, 31100, 31100 },
            };

            for (var i = 0; i < 3; i++)
            {
                for (var j = 0; j < 3; j++)
                {
                    var card = _cardViewPool.GetCardView(map[j, i]);
                    SnapCardToGrid(card, new Vector3Int(i * 18, j * 24, 0));
                    card.gameObject.SetActive(true);
                }
            }
        }

        // TODO: Move in to DragController
        private bool HandleCardViewStartDrag(IDraggable draggable, Vector3 worldPosition, Vector3 hitPoint)
        {
            if (draggable is not CardView cardView) return false;

            cardView.SortingGroup.sortingLayerName = CardViewSortingLayerNameDraggable;
            cardView.transform.SetPositionAndRotation(worldPosition, Quaternion.identity);

            return true;

        }

        // TODO: Move in to DragController
        private void HandleCardViewDrag(IDraggable draggable, Vector3 worldPosition)
        {
            if (draggable is not CardView cardView) return;

            cardView.transform.SetPositionAndRotation(worldPosition, Quaternion.identity);
        }

        // TODO: Move in to DragController
        private void HandleCardViewStopDrag(IDraggable draggable, Vector3 worldPosition)
        {
            if (draggable is not CardView cardView) return;
            
            cardView.SortingGroup.sortingLayerName = CardViewSortingLayerNameDefault;
            // TODO: Use GridManager to find empty cell
            SnapCardToGridByWorldPosition(cardView, cardView.transform.position);
        }

        public void SnapCardToGridByWorldPosition(CardView cardView, Vector3 position)
        {
            var cel = _grid.WorldToCell(position);
            SnapCardToGrid(cardView, new Vector3Int(cel.x - (cel.x % 3), cel.y - (cel.y % 3), cel.z));
        }

        public void SnapCardToGrid(CardView cardView, Vector3Int position)
        {
            AssertConstructed();

            cardView.transform.SetPositionAndRotation(_grid.CellToWorld(position), Quaternion.identity);
        }

        public void DestructGameTable()
        {
            if (_grid != null)
            {
                _grid.gameObject.SetActive(false);
                Object.Destroy(_grid.gameObject);
                _grid = null;
            }

            _inputService.OnStartDrag -= HandleCardViewStartDrag;
            _inputService.OnDrag -= HandleCardViewDrag;
            _inputService.OnStopDrag -= HandleCardViewStopDrag;

            _cardViewPool.Destruct();
        }

        private void AssertConstructed()
        {
            if (_grid == null)
                throw new System.Exception("GridView is not constructed");
        }
    }
}