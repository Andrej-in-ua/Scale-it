using UnityEngine;
using System.Collections.Generic;

namespace View.GameTable
{
    public class EnvironmentManager
    {
        private readonly Dictionary<Vector2Int, GameObject> _generatedChunks = new();
        
        private GameObject _container;
        
        private float _seed;

        private const float Zoom = 90f;
        private const int ChunkSize = 120;
        private const int CellStep = 10;
        private const int ActiveChunkRange = 3;

        public void Construct(Vector3 cameraPosition, Grid grid, IEnvironmentFactory factory)
        {
            _container = new GameObject("Environment Container");
            _seed = Random.Range(0, 9999999);

            UpdateEnvironmentAround(cameraPosition, grid, factory);
        }
        
        public void UpdateEnvironmentAround(Vector3 cameraPosition, Grid grid, IEnvironmentFactory factory)
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
                        chunk = GenerateChunk(chunkCoord, grid, factory);
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

        private GameObject GenerateChunk(Vector2Int chunkCoord, Grid grid, IEnvironmentFactory factory)
        {
            GameObject chunkRoot = new($"Chunk_{chunkCoord.x}_{chunkCoord.y}");
            chunkRoot.transform.SetParent(_container.transform);

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

                    factory.CreateEnvironmentObject(noise, worldPosition, chunkRoot.transform);
                }
            }

            return chunkRoot;
        }
    }
}