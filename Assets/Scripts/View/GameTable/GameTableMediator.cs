using UnityEngine;

namespace View.GameTable
{
    public class GameTableMediator
    {
        private readonly GridFactory _gridFactory;
        private readonly CardViewPool _cardViewPool;
        
        private Grid _grid;

        public GameTableMediator(GridFactory gridFactory, CardViewPool cardViewPool)
        {
            _gridFactory = gridFactory;
            _cardViewPool = cardViewPool;
        }

        public void ConstructGameTable()
        {
            _grid = _gridFactory.Create();

            _cardViewPool.Construct();
            

            int[,] map = {
                {32100, 32100, 32100},
                {31210, 31211, 31211},
                {30100, 31100, 31100},
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
            
            _cardViewPool.Destruct();
        }
        
        private void AssertConstructed()
        {
            if (_grid == null)
                throw new System.Exception("GridView is not constructed");
        }
    }
}