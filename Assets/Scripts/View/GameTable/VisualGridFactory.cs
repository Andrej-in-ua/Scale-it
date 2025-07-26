using UnityEngine;
using Services;

namespace View.GameTable
{
    public class VisualGridFactory
    {
        private readonly IAssetProviderService _assetProviderService;

        public VisualGridFactory(IAssetProviderService assetProviderService)
        {
            _assetProviderService = assetProviderService;
        }
        
        public (Mesh, GameObject) Construct()
        {
            Mesh mesh = new Mesh();
            GameObject gridPrefab = Object.Instantiate(_assetProviderService.LoadAssetFromResources<GameObject>(Constants.VisualGridPath));
            gridPrefab.transform.position = new Vector2(-1, -1);
            gridPrefab.gameObject.GetComponent<MeshFilter>().mesh = mesh;

            return (mesh, gridPrefab);
        }
    }
}
