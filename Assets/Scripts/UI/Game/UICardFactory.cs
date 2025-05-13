using GameTable;
using Services;
using UI.Game.CardPreviews;
using UI.Game.Inventory;
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

        public UICardPreview CreateUICard(Transform parent, UIInventory _inventory, int cardId, out DragCard dragCard)
        {
            var cardPrefab = _assetProviderService.LoadAssetFromResources<GameObject>(Constants.CardPath);
            var cardGameobject = Object.Instantiate(cardPrefab, parent);
            UICardPreview card = cardGameobject.GetComponent<UICardPreview>();
            dragCard = cardGameobject.GetComponent<DragCard>();
            
            card.name += cardId;
            card.Name.text = "Card " + cardId;

            dragCard.Construct(_inventory);
            dragCard.CardId = cardId;
           // ConstructLinePortsForCard(card.transform, 4, 4, 2);

            return card;
        }

        // TODO: MB move it to self factory?
      

        public Transform CreateLinePort(Transform parent, string inputPath)
        {
            GameObject linePortPrefab = _assetProviderService.LoadAssetFromResources<GameObject>(inputPath).gameObject;
            GameObject linePort = Object.Instantiate(linePortPrefab, parent);

            return linePort.GetComponent<Transform>();
        }

        private void ConstructLinePortsForCard(Transform card, int inputsCount, int outputsCount, int modifiersCount)
        {
            (int count, string path)[] portConfigs =
            {
                (inputsCount, Constants.InputPath),
                (outputsCount, Constants.OutputPath),
                (modifiersCount, Constants.ModifierPath)
            };

            for (int i = 0; i < portConfigs.Length; i++)
            {
                Transform parent = card.GetChild(i);

                for (int j = 0; j < portConfigs[i].count; j++)
                {
                    CreateLinePort(parent, portConfigs[i].path);
                }
            }
        }
    }
}