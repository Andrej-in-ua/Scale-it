using GameTable;
using Services;
using UI.Game.Inventory;
using UnityEngine;

namespace UI.Game
{
    public class UIGameFactory : IUIGameFactory
    {
        private readonly IAssetProviderService _assetProviderService;
        private UICardFactory _uiCardFactory;

        public UIGameFactory(IAssetProviderService assetProviderService, UICardFactory uiCardFactory)
        {
            _uiCardFactory = uiCardFactory;
            _assetProviderService = assetProviderService;
        }
        
        public UIInventory CreateInventory()
        {
            var inventoryPrefab = _assetProviderService.LoadAssetFromResources<GameObject>(Constants.InventoryPath)
                .gameObject;
            GameObject inventory = Object.Instantiate(inventoryPrefab);
            
            var uiInventory = inventory.GetComponent<UIInventory>();
            uiInventory.Construct();

            return uiInventory;
        }

        public CardSpawner CreateCardSpawner(Transform parent)
        {
            var cardSpawnerPrefab = _assetProviderService.LoadAssetFromResources<GameObject>(Constants.CardSpawnerPath)
                .gameObject;
            
            GameObject cardSpawnerButton = Object.Instantiate(cardSpawnerPrefab, parent);

            return cardSpawnerButton.GetComponent<CardSpawner>();
        }
    }
}