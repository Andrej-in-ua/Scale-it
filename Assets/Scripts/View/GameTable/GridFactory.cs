using Services;
using UnityEngine;

namespace View.GameTable
{
    public class GridFactory
    {
        private readonly IAssetProviderService _assetProviderService;

        private GameObject GridPrefab => _gridPrefab ??= _assetProviderService.LoadAssetFromResources<GameObject>(Constants.GridPath);
        private GameObject _gridPrefab;

        public GridFactory(IAssetProviderService assetProviderService)
        {
            _assetProviderService = assetProviderService;
        }

        public Grid Create()
        {
            var view = Object.Instantiate(GridPrefab);
            view.name = "GameTableGrid";

            return view.GetComponent<Grid>();
        }
    }
}