using Services;
using UI.Game.Inventory;
using UnityEngine;

namespace UI.Game
{
    public class UIGameFactory
    {
        private readonly IAssetProviderService _assetProviderService;

        public UIGameFactory(IAssetProviderService assetProviderService)
        {
            _assetProviderService = assetProviderService;
        }
        
        public UIInventory CreateInventory()
        {
            var inventoryPrefab = _assetProviderService.LoadAssetFromResources<GameObject>(Constants.InventoryPath)
                .gameObject;
            GameObject inventory = Object.Instantiate(inventoryPrefab);

            return inventory.GetComponent<UIInventory>();
        }  
    }
}