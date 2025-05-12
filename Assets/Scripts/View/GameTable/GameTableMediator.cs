using UnityEngine;

namespace View.GameTable
{
    public class GameTableMediator
    {
        private readonly GridManager _gridManager;
        private readonly CardViewPool _cardViewPool;
        private readonly CardDragController _cardDragController;
        
        private bool _isConstructed;

        public GameTableMediator(
            GridManager gridManager,
            CardViewPool cardViewPool,
            CardDragController cardDragController
        )
        {
            _gridManager = gridManager;
            _cardViewPool = cardViewPool;
            _cardDragController = cardDragController;
        }

        public void ConstructGameTable()
        {
            _gridManager.Construct();
            _cardViewPool.Construct();
            _cardDragController.Construct();
            
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

            var placedPosition = _gridManager.PlaceOnNearestAvailablePosition(position, cardView.PlaceableReference, out var needRelocate);

            if (placedPosition.HasValue)
            {
                cardView.transform.SetPositionAndRotation(placedPosition.Value, Quaternion.identity);
            }
        }

        public void DestructGameTable()
        {
            _isConstructed = false;

            _cardDragController.Destruct();
            _cardViewPool.Destruct();
            _gridManager.Destruct();
        }

        private void AssertConstructed()
        {
            if (!_isConstructed)
                throw new System.Exception("GridView is not constructed");
        }
    }
}