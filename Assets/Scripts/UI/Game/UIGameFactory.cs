using Services;
using UI.Game.Inventory;
using UnityEngine;

namespace UI.Game
{
    public class UIGameFactory
    {
        private readonly IAssetProviderService _assetProviderService;
        private IUICardFactory _uiCardFactory;

        public UIGameFactory(IAssetProviderService assetProviderService, IUICardFactory uiCardFactory)
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
            uiInventory.Construct(_uiCardFactory);

            return uiInventory;
        }  
    }
}