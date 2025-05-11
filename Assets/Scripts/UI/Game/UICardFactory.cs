using Services;
using UI.Game.CardPreviews;
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

        public UICardPreview CreateUICard(Transform parent, string name)
        {
            var cardPrefab = _assetProviderService.LoadAssetFromResources<GameObject>(Constants.UICardPreviewPath);
            UICardPreview card = Object.Instantiate(cardPrefab, parent).GetComponent<UICardPreview>();
            
            card.name = name;
            card.Name.text = name;

            return card;
        }
    }
}