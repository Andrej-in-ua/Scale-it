using UnityEngine;
using Services;

namespace View.GameTable
{
    public class EnvironmentFactory : IEnvironmentFactory
    {
        private readonly IAssetProviderService _assetProviderService;

        private GameObject _treeOne, _treeTwo, _treeThree;

        public EnvironmentFactory(IAssetProviderService assetProviderService)
        {
            _assetProviderService = assetProviderService;
        }
        
        public void LoadAssets()
        {
            _treeOne   = _assetProviderService.LoadAssetFromResources<GameObject>(Constants.TreeOneViewPath);
            _treeTwo   = _assetProviderService.LoadAssetFromResources<GameObject>(Constants.TreeTwoViewPath);
            _treeThree = _assetProviderService.LoadAssetFromResources<GameObject>(Constants.TreeThreeViewPath);
        }

        public GameObject CreateEnvironmentObject(float noise, Vector3 position, Transform parent)
        {
            GameObject prefab = GetPrefabByNoise(noise);
            
            return prefab == null ? null : Object.Instantiate(prefab, position, Quaternion.identity, parent);
        }

        private GameObject GetPrefabByNoise(float noise)
        {
            if (noise > 0.75f) return _treeThree;
            if (noise > 0.55f) return _treeTwo;
            if (noise > 0.40f) return _treeOne;
            return null;
        }
    }
}