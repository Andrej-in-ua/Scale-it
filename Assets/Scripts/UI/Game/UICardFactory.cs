using Services;
using UI.Game.CardPreviews;
using UnityEngine;

namespace UI.Game
{
    public class UICardFactory
    {
        private readonly IAssetProviderService _assetProviderService;

        public UICardFactory(IAssetProviderService assetProviderService)
        {
            _assetProviderService = assetProviderService;
        }

        public UICardPreview CreateUICard(int cardId, Transform parent)
        {
            var cardPrefab = _assetProviderService.LoadAssetFromResources<GameObject>(Constants.UICardPreviewPath);
            var cardGameObject = Object.Instantiate(cardPrefab, parent);
            UICardPreview card = cardGameObject.GetComponent<UICardPreview>();
            
            card.name = "UiCardPreview_" + cardId;
            card.Name.text = "Card " + cardId;
            card.CardId = cardId;

            return card;
        }
    }
}