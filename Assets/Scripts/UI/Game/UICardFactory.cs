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

        public Transform CreateUICard(Transform parent)
        {
            var cardPrefab = _assetProviderService.LoadAssetFromResources<GameObject>(Constants.CardPath);
            GameObject card = Object.Instantiate(cardPrefab, parent);

            return card.transform;
        }

        // TODO: MB move it to salf factory?
        public Transform CreateInventory()
        {
            var inventoryPrefab = _assetProviderService.LoadAssetFromResources<GameObject>(Constants.InventoryPath).gameObject;
            GameObject inventory = Object.Instantiate(inventoryPrefab);

            return inventory.GetComponent<Transform>();
        }

        public Transform CreateLinePort(Transform parent, string inputPath)
        {
            GameObject linePortPrefab = _assetProviderService.LoadAssetFromResources<GameObject>(inputPath).gameObject;
            GameObject linePort = Object.Instantiate(linePortPrefab, parent);
            
            return linePort.GetComponent<Transform>();
        }

        public Transform CreateCardName(Transform parent)
        {
            GameObject cardNamePrefab = _assetProviderService.LoadAssetFromResources<GameObject>(Constants.CardNamePath).gameObject;
            
            GameObject cardName = Object.Instantiate(cardNamePrefab, parent);
            cardName.transform.position = new Vector2(parent.position.x, parent.position.y + 80);
            
            return cardName.transform;
        }
    }
}