using Services;
using UnityEngine;

namespace View.GameTable
{
    public class ConnectionFactory
    {
        private readonly IAssetProviderService _assetProviderService;

        public ConnectionFactory(IAssetProviderService assetProviderService)
        {
            _assetProviderService = assetProviderService;
        }

        public Transform CreateConnectionView()
        {
            var connectionPrefab = _assetProviderService.LoadAssetFromResources<GameObject>(Constants.ConnectionViewPath);
            var connectionGameObject = Object.Instantiate(connectionPrefab);
            
            return connectionGameObject.transform;
        }
    }
}