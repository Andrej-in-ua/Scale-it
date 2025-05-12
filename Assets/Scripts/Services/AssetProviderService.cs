using UnityEngine;

namespace Services
{
    public class AssetProviderService : IAssetProviderService
    {
        public T LoadAssetFromResources<T>(string path) where T : UnityEngine.Object =>
            Resources.Load<T>(path);

        public T LoadAssetFromResourcesForceInactive<T>(string path) where T : UnityEngine.Object
        {
            var asset = LoadAssetFromResources<T>(path);

            if (asset is not GameObject gameObject)
            {
                Debug.LogError($"Error: Asset at path {path} is not a GameObject.");
                return asset;
            }

            var container = new GameObject("PrefabCloneContainer");
            container.SetActive(false);

            var clone = Object.Instantiate(gameObject, container.transform);
            clone.SetActive(false);
            clone.transform.SetParent(null);
            Object.Destroy(container);

            return clone as T;
        }
    }
}