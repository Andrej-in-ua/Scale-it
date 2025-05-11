using UnityEngine;
using View.CardEntity;

namespace View.GameTable
{
    public class GameTableMediator
    {
        private readonly GridFactory _gridFactory;
        private readonly CardViewFactory _cardViewFactory;

        private Grid _grid;

        public GameTableMediator(GridFactory gridFactory, CardViewFactory cardViewFactory)
        {
            _gridFactory = gridFactory;
            _cardViewFactory = cardViewFactory;
        }

        public void ConstructGameTable()
        {
            _grid = _gridFactory.Create();
            _grid.gameObject.SetActive(true);

            int[,] map = {
                {32100, 32100, 32100},
                {31210, 31211, 31211},
                {30100, 31100, 31100},
            };
            
            for (var i = 0; i < 3; i++)
            {
                for (var j = 0; j < 3; j++)
                {
                    var card = _cardViewFactory.Create(map[j, i]);
                    SnapCardToGrid(card, new Vector3Int(i * 18, j * 24, 0));
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
        }
        
        private void AssertConstructed()
        {
            if (_grid == null)
                throw new System.Exception("GridView is not constructed");
        }
    }
}