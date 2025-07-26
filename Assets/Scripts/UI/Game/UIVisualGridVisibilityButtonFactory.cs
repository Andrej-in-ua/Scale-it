using Services;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Game
{
    public class UIVisualGridVisibilityButtonFactory
    {
        private readonly IAssetProviderService _assetProviderService;

        public UIVisualGridVisibilityButtonFactory(IAssetProviderService assetProviderService)
        {
            _assetProviderService = assetProviderService;
        }

        public Button Construct(Transform parent)
        {
            GameObject button =
                Object.Instantiate(
                    _assetProviderService.LoadAssetFromResources<GameObject>(Constants.VisualGridVisibilityButtonPath),
                    parent);

            return button.GetComponent<Button>();
        }
    }
}