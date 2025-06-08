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

        public Transform CreateConnectionView(Transform parent)
        {
            var connectionPrefab = _assetProviderService.LoadAssetFromResources<GameObject>(Constants.ConnectionViewPath);
            var connectionGameObject = Object.Instantiate(connectionPrefab, parent);
            
            return connectionGameObject.transform;
        }

        public Transform CreateConnectionsContainer()
        {
            var objectPrefab = _assetProviderService.LoadAssetFromResources<GameObject>(Constants.ConnectionsContainerPath);
            var connectionsContainer = Object.Instantiate(objectPrefab);
            
            return connectionsContainer.transform;
        }
    }
}