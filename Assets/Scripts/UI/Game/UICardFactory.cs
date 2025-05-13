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
            var cardPrefab = _assetProviderService.LoadAssetFromResources<GameObject>(Constants.UICardPreviewPath);
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
    }
}