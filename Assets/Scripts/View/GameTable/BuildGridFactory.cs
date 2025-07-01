using UnityEngine;
using System.Collections.Generic;
using Services;

namespace View.GameTable
{
    public class BuildGridFactory
    {
        private readonly IAssetProviderService _assetProviderService;
        
        private Mesh _mesh;
        private GameObject _gridPrefab;

        public BuildGridFactory(IAssetProviderService assetProviderService)
        {
            _assetProviderService = assetProviderService;
        }
        
        public (Mesh, GameObject) Construct()
        {
            _mesh = new Mesh();
            _gridPrefab = Object.Instantiate(_assetProviderService.LoadAssetFromResources<GameObject>(Constants.BuildGridPath));
            _gridPrefab.gameObject.GetComponent<MeshFilter>().mesh = _mesh;
            
            return (_mesh, _gridPrefab);
        }
    }
}
