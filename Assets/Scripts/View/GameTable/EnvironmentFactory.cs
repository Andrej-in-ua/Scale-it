using System.Collections.Generic;
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

        private float _seed;
        private float _zoom = 90f;

        private readonly int _cellBlockSize = 200;
        private readonly int _activeChunkRange = 3;

        private readonly Dictionary<Vector2Int, GameObject> _generatedChunks = new();

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
        
        public void Initialize()
        {
            _seed = Random.Range(0, 9999999);
            LoadEnvironmentViews();
        }
        
        public void UpdateEnvironmentAround(Vector3 worldPosition, Transform parent, Grid grid)
        {
            if (grid == null)
                return;

            Vector3Int cellPos = grid.WorldToCell(worldPosition);
            Vector2Int centerChunk = new(cellPos.x / _cellBlockSize, cellPos.y / _cellBlockSize);

            HashSet<Vector2Int> neededChunks = new();

            for (int dx = -_activeChunkRange; dx <= _activeChunkRange; dx++)
            {
                for (int dy = -_activeChunkRange; dy <= _activeChunkRange; dy++)
                {
                    Vector2Int chunkCoord = centerChunk + new Vector2Int(dx, dy);
                    neededChunks.Add(chunkCoord);

                    if (!_generatedChunks.ContainsKey(chunkCoord))
                    {
                        GameObject chunk = GenerateChunkAt(chunkCoord, parent, grid);
                        _generatedChunks.Add(chunkCoord, chunk);
                    }
                    else
                    {
                        _generatedChunks[chunkCoord].SetActive(true);
                    }
                }
            }

            foreach (var kvp in _generatedChunks)
            {
                if (!neededChunks.Contains(kvp.Key))
                {
                    kvp.Value.SetActive(false);
                }
            }
        }
        
        private GameObject GenerateChunkAt(Vector2Int chunkCoord, Transform parent, Grid grid)
        {
            GameObject chunkRoot = new($"Chunk_{chunkCoord.x}_{chunkCoord.y}");
            chunkRoot.transform.parent = parent;

            int baseX = chunkCoord.x * _cellBlockSize;
            int baseY = chunkCoord.y * _cellBlockSize;

            int cellBlockSize = 10;
            
            for (int x = 0; x < _cellBlockSize; x += cellBlockSize)
            {
                for (int y = 0; y < _cellBlockSize; y += cellBlockSize)
                {
                    int worldX = baseX + x;
                    int worldY = baseY + y;

                    Vector3Int cellPosition = new(worldX, worldY, 0);
                    Vector3 worldPosition = grid.CellToWorld(cellPosition) + grid.cellSize / 2f;

                    float noiseValue = Mathf.PerlinNoise((worldX + _seed) / _zoom, (worldY + _seed) / _zoom);

                    GameObject prefabToSpawn = null;

                    if (noiseValue > 0.75f)
                        prefabToSpawn = _treeThree;
                    else if (noiseValue > 0.55f)
                        prefabToSpawn = _treeTwo;
                    else if (noiseValue > 0.4f)
                        prefabToSpawn = _treeOne;

                    if (prefabToSpawn != null)
                    {
                        var instance = Object.Instantiate(prefabToSpawn, worldPosition, Quaternion.identity, chunkRoot.transform);
                        instance.SetActive(true);
                    }
                }
            }

            return chunkRoot;
        }
    }
}