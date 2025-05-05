using Services;
using UnityEngine;

namespace UI.Game
{
    public class UICardFactory : IUICardFactory
    {
        private readonly IAssetProviderService _assetProviderService;

        public UICardFactory(IAssetProviderService assetProviderService)
        {
            _assetProviderService = assetProviderService;
        }

        public CardView CreateUICard(Transform parent)
        {
            var cardPrefab = _assetProviderService.LoadAssetFromResources<GameObject>(Constants.CardPath);
            GameObject card = Object.Instantiate(cardPrefab, parent);

            return card.GetComponent<CardView>();
        }

        // TODO: MB move it to salf factory?
        public Transform CreateInventory()
        {
            // TODO: Add inventory prefab
            var inventoryPrefab = _assetProviderService.LoadAssetFromResources<GameObject>(Constants.InventoryPath).gameObject;
            GameObject inventory = Object.Instantiate(inventoryPrefab);

            return inventory.GetComponent<Transform>();
        }
    }
}