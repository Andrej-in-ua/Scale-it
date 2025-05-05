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
               Transform inventoryPanel = _inventory.GetChild(0).transform;
               var card = _factory.CreateUICard(inventoryPanel);
           }
        }
    }

    public interface IGameMediator
    {
        public void ConstructUI();
    }
}