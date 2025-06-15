using Services;
using UnityEngine;

namespace View.GameTable
{
    public class EnvironmentFactory
    {
        private readonly IAssetProviderService _assetProviderService;
        
        private GameObject _treeOne;
        private GameObject _treeTwo;
        private GameObject _treeThree;

        private int _width = 500;
        private int _height = 500;

        private float _sid, _zoom = 90; 

        EnvironmentFactory(IAssetProviderService assetProviderService)
        {
            _assetProviderService = assetProviderService;
        }
        
        public void LoadEnvironmentViews()
        {
            _treeOne = _assetProviderService.LoadAssetFromResources<GameObject>(Constants.TreeOneViewPath);
            _treeTwo = _assetProviderService.LoadAssetFromResources<GameObject>(Constants.TreeTwoViewPath);
            _treeThree = _assetProviderService.LoadAssetFromResources<GameObject>(Constants.TreeThreeViewPath);
        }
        
        public void ConstructEnvironment(Transform parent, Grid grid)
        {
            if (grid == null)
            {
                return;
            }
            
            LoadEnvironmentViews();

            _sid = Random.Range(0, 9999999);
            
            int cellBlockSize = 10;

            for (int x = 0; x < _width; x += cellBlockSize)
            {
                for (int y = 0; y < _height; y += cellBlockSize)
                {
                    Vector3Int cellPosition = new Vector3Int(x, y, 0);
                    Vector3 worldPosition = grid.CellToWorld(cellPosition) + grid.cellSize * cellBlockSize / 2f;

                    float noiseValue = Mathf.PerlinNoise((x + _sid) / _zoom, (y + _sid) / _zoom);

                    GameObject prefabToSpawn = null;

                    if (noiseValue > 0.75f)
                        prefabToSpawn = _treeThree;
                    else if (noiseValue > 0.55f)
                        prefabToSpawn = _treeTwo;
                    else if (noiseValue > 0.4f)
                        prefabToSpawn = _treeOne;

                    if (prefabToSpawn != null)
                    {
                        var instance = Object.Instantiate(prefabToSpawn, worldPosition, Quaternion.identity, parent);
                        instance.SetActive(true);
                    }
                }
            }
        }
    }
}