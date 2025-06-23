using UnityEngine;
using System.Collections.Generic;
using Services;

namespace View.GameTable
{
    public class BuildGridFactory
    {
        private readonly IAssetProviderService _assetProviderService;
        
        private Grid _grid;
        private Camera _camera;
        private Mesh _mesh;
        private GameObject _gridPrefab;

        public BuildGridFactory(IAssetProviderService assetProviderService)
        {
            _assetProviderService = assetProviderService;
        }
        
        public void Construct(Grid grid, Camera camera)
        {
            _camera = camera;
            _grid = grid;
            _mesh = new Mesh();
            
            _gridPrefab = Object.Instantiate(_assetProviderService.LoadAssetFromResources<GameObject>(Constants.BuildGridPath));
            _gridPrefab.gameObject.GetComponent<MeshFilter>().mesh = _mesh;
            
            DrawGrid();
        }

        public void DrawGrid()
        {
            if (!_grid) return;

            _mesh.Clear();

            float zoomFactor = Mathf.InverseLerp(Constants.CameraSettings.ZoomMin, Constants.CameraSettings.ZoomMax, _camera.orthographicSize);

            float visualCellSize = _grid.cellSize.x * (zoomFactor < 0.15f ? 1 : zoomFactor < 0.6f ? 10 : 50);

            float cameraWidth = _camera.orthographicSize * _camera.aspect * 2f;
            float cameraHeight = _camera.orthographicSize * 2;

            Vector3 cameraPosition = _camera.transform.position;
            Vector3 gridOrigin = _grid.transform.position;

            float left = cameraPosition.x - cameraWidth / 2;
            float right = cameraPosition.x + cameraWidth / 2;
            float bottom = cameraPosition.y - cameraHeight / 2;
            float top = cameraPosition.y + cameraHeight / 2;

            float startX = Mathf.Floor((left - gridOrigin.x) / visualCellSize) * visualCellSize + gridOrigin.x;
            float endX = Mathf.Ceil((right - gridOrigin.x) / visualCellSize) * visualCellSize + gridOrigin.x;
            float startY = Mathf.Floor((bottom - gridOrigin.y) / visualCellSize) * visualCellSize + gridOrigin.y;
            float endY = Mathf.Ceil((top - gridOrigin.y) / visualCellSize) * visualCellSize + gridOrigin.y;

            List<Vector3> lineVertices = new();
            List<int> lineIndices = new();

            for (float x = startX; x <= endX; x += visualCellSize)
            {
                lineVertices.Add(new Vector3(x, startY));
                lineVertices.Add(new Vector3(x, endY));
                lineIndices.Add(lineVertices.Count - 2);
                lineIndices.Add(lineVertices.Count - 1);
            }

            for (float y = startY; y <= endY; y += visualCellSize)
            {
                lineVertices.Add(new Vector3(startX, y));
                lineVertices.Add(new Vector3(endX, y));
                lineIndices.Add(lineVertices.Count - 2);
                lineIndices.Add(lineVertices.Count - 1);
            }

            _mesh.vertices = lineVertices.ToArray();
            _mesh.SetIndices(lineIndices.ToArray(), MeshTopology.Lines, 0);
        }
    }
}
