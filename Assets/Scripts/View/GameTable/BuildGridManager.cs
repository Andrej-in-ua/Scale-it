using System.Collections.Generic;
using UnityEngine;

namespace View.GameTable
{
    public class BuildGridManager
    {
         public void DrawGrid(Grid grid, Camera camera, Mesh mesh)
        {
            if (!grid) return;

            mesh.Clear();

            float zoomFactor = Mathf.InverseLerp(Constants.CameraSettings.ZoomMin, Constants.CameraSettings.ZoomMax, camera.orthographicSize);

            float visualCellSize = grid.cellSize.x * (zoomFactor < 0.15f ? 1 : zoomFactor < 0.6f ? 10 : 50);

            float cameraWidth = camera.orthographicSize * camera.aspect * 2f;
            float cameraHeight = camera.orthographicSize * 2;

            Vector3 cameraPosition = camera.transform.position;
            Vector3 gridOrigin = grid.transform.position;

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

            mesh.vertices = lineVertices.ToArray();
            mesh.SetIndices(lineIndices.ToArray(), MeshTopology.Lines, 0);
        }
    }
}