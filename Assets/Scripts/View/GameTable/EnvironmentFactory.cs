using System.Collections.Generic;
using UnityEngine;
using Services;

namespace View.GameTable
{
    public class EnvironmentFactory
    {
        private readonly IAssetProviderService _assetProviderService;

        private GameObject _treeOne, _treeTwo, _treeThree;
        private GameObject _container;
        
        private readonly Dictionary<Vector2Int, GameObject> _generatedChunks = new();

        private float _seed;
        
        private const float Zoom = 90f;
        private const int ChunkSize = 120;
        private const int CellStep = 10;
        private const int ActiveChunkRange = 3;

        public EnvironmentFactory(IAssetProviderService assetProviderService)
        {
            _assetProviderService = assetProviderService;
        }
        
        public void Initialize(Vector3 cameraPosition, Grid grid)
        {
            _container = Object.Instantiate(new GameObject("Environment Container"));
            
            _seed = Random.Range(0, 9999999);
            LoadEnvironmentViews();
            UpdateEnvironmentAround(cameraPosition, grid);
        }

        private void LoadEnvironmentViews()
        {
            _treeOne   = _assetProviderService.LoadAssetFromResources<GameObject>(Constants.TreeOneViewPath);
            _treeTwo   = _assetProviderService.LoadAssetFromResources<GameObject>(Constants.TreeTwoViewPath);
            _treeThree = _assetProviderService.LoadAssetFromResources<GameObject>(Constants.TreeThreeViewPath);
        }

        public void UpdateEnvironmentAround(Vector3 cameraPosition, Grid grid)
        {
            if (grid == null) return;

            Vector3Int cellPos = grid.WorldToCell(cameraPosition);
            Vector2Int centerChunk = new(cellPos.x / ChunkSize, cellPos.y / ChunkSize);

            HashSet<Vector2Int> requiredChunks = new();

            for (int dx = -ActiveChunkRange; dx <= ActiveChunkRange; dx++)
            {
                for (int dy = -ActiveChunkRange; dy <= ActiveChunkRange; dy++)
                {
                    Vector2Int chunkCoord = centerChunk + new Vector2Int(dx, dy);
                    requiredChunks.Add(chunkCoord);

                    if (!_generatedChunks.TryGetValue(chunkCoord, out GameObject chunk))
                    {
                        chunk = GenerateChunk(chunkCoord, _container.transform, grid);
                        _generatedChunks.Add(chunkCoord, chunk);
                    }
                    chunk.SetActive(true);
                }
            }

            foreach (var (coord, chunk) in _generatedChunks)
            {
                if (!requiredChunks.Contains(coord))
                    chunk.SetActive(false);
            }
        }

        private GameObject GenerateChunk(Vector2Int chunkCoord, Transform parent, Grid grid)
        {
            GameObject chunkRoot = new($"Chunk_{chunkCoord.x}_{chunkCoord.y}");
            chunkRoot.transform.SetParent(parent);

            int baseX = chunkCoord.x * ChunkSize;
            int baseY = chunkCoord.y * ChunkSize;

            for (int x = 0; x < ChunkSize; x += CellStep)
            {
                for (int y = 0; y < ChunkSize; y += CellStep)
                {
                    int worldX = baseX + x;
                    int worldY = baseY + y;
                    Vector3 worldPosition = grid.CellToWorld(new Vector3Int(worldX, worldY, 0)) + grid.cellSize / 2f;

                    float noise = Mathf.PerlinNoise((worldX + _seed) / Zoom, (worldY + _seed) / Zoom);
                    GameObject prefab = GetPrefabByNoise(noise);

                    if (prefab != null)
                        Object.Instantiate(prefab, worldPosition, Quaternion.identity, chunkRoot.transform).SetActive(true);
                }
            }

            return chunkRoot;
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