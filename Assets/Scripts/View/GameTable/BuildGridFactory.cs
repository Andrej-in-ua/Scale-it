using UnityEngine;
using System.Collections.Generic;
using Services;

namespace View.GameTable
{
    public class BuildGridFactory
    {
        private readonly IAssetProviderService _assetProviderService;

        public BuildGridFactory(IAssetProviderService assetProviderService)
        {
            _assetProviderService = assetProviderService;
        }
        
        public (Mesh, GameObject) Construct()
        {
            Mesh mesh = new Mesh();
            GameObject gridPrefab = Object.Instantiate(_assetProviderService.LoadAssetFromResources<GameObject>(Constants.BuildGridPath));
            gridPrefab.transform.position = new Vector2(-1, -1);
            gridPrefab.gameObject.GetComponent<MeshFilter>().mesh = mesh;

            return (mesh, gridPrefab);
        }
    }
}
