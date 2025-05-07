using System.Collections.Generic;
using GameTable;
using UI.Game.Inventory;
using UnityEngine;

namespace UI.Game
{
    public class UIGameMediator : IUIGameMediator
    {
        private readonly IUICardFactory _cardFactory;
        private readonly UIGameFactory _uiFactory;
        
        private UIInventory _inventory;
        
        private List<DragCard> _dragCards;

        public UIGameMediator(IUICardFactory cardFactory, UIGameFactory uiFactory)
        {
            _cardFactory = cardFactory;
            _uiFactory = uiFactory;
        }
        
        public void ConstructUI()
        {
           _inventory = _uiFactory.CreateInventory();

           for (int i = 0; i < 5; i++)
           {
               Transform inventoryPanel = _inventory.transform.GetChild(0).transform;
               var card = _cardFactory.CreateUICard(inventoryPanel, "Card " + i);
           }
        }
    }
}