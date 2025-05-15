namespace Services
{
    public interface IAssetProviderService
    {
        T LoadAssetFromResources<T>(string path) where T : UnityEngine.Object;

        T LoadAssetFromResourcesForceInactive<T>(string path) where T : UnityEngine.Object;
    }
}