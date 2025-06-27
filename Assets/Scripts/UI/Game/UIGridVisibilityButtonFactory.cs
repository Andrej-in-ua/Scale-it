using Services;
using UnityEngine;
using UnityEngine.UI;

public class UIGridVisibilityButtonFactory
{
    private readonly IAssetProviderService _assetProviderService;

    private UIGridVisibilityButtonFactory(IAssetProviderService assetProviderService)
    {
        _assetProviderService = assetProviderService;
    }

    public Button Construct(Transform parent)
    {
        GameObject button = Object.Instantiate(_assetProviderService.LoadAssetFromResources<GameObject>(Constants.GridVisibilityButtonPath), parent);
        
        return button.GetComponent<Button>();
    }
}