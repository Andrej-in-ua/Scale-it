using System.Collections.Generic;
using GameTable;
using UI.Game.CardPreviews;
using UI.Game.Inventory;
using UnityEngine;

namespace UI.Game
{
    public class UIGameMediator : IUIGameMediator
    {
        private readonly IUICardFactory _uiCardFactory;
        private readonly UIGameFactory _uiFactory;

        private UIInventory _inventory;
        private List<UICardPreview> _dragCards;
        private Transform _inventoryPanel;

        public UIGameMediator(
            IUICardFactory uiCardFactory,
            UIGameFactory uiFactory
        )
        {
            _uiCardFactory = uiCardFactory;
            _uiFactory = uiFactory;
        }

        public void ConstructUI()
        {
            _inventory = _uiFactory.CreateInventory();
            _inventoryPanel = _inventory.transform.GetChild(0).transform;

            for (int i = 0; i < 10; i++)
            {
                var card = _uiCardFactory.CreateUICard(_inventoryPanel, _inventory, Random.Range(0, 10));
                card.OnStartDragCardPreview += MoveCardToTable;
                
                _inventory.AddCardToInventory(card);
            }
        }

        private void MoveCardToTable(UICardPreview container)
        {
            var card = _uiCardFactory.CreateUICard(_inventoryPanel, _inventory, container.CardId);
            _inventory.TakeCardFromStack(card.CardId);
        }
    }
}