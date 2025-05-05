using System.Collections.Generic;
using GameTable;
using UnityEngine;

namespace UI.Game
{
    public class GameMediator : IGameMediator
    {
        private readonly IUICardFactory _factory;
        private Transform _inventory;
        
        private List<DragCard> _dragCards;

        public GameMediator(IUICardFactory factory)
        {
            _factory = factory;
        }

        public void ConstructUI()
        {
           _inventory = _factory.CreateInventory();

           for (int i = 0; i < 5; i++)
           {
               var dragCard = _factory.CreateUICard(_inventory);
           }
        }
    }

    public interface IGameMediator
    {
        public void ConstructUI();
    }
}