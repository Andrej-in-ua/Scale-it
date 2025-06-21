using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GridRendererTest : MonoBehaviour
{
    [SerializeField] private Grid _grid;
    [SerializeField] private Camera _camera;
    [SerializeField] private float _minZoom = 16, _maxZoom = 128;

    private Mesh _mesh;

    private void Start()
    {
        _camera = Camera.main;
        _grid = FindObjectOfType<Grid>();
        _mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = _mesh;
    }

    private void Update()
    {
        DrawGrid();
    }

    private void DrawGrid()
    {
        if (!_grid) return;

        _mesh.Clear();
        Camera camera = _camera;
        Grid grid = _grid;

        float zoom = camera.orthographicSize;
        float t = Mathf.InverseLerp(_minZoom, _maxZoom, zoom);

        float cellSize = grid.cellSize.x;
        float visualSize = cellSize * (t < 0.2f ? 1 : t < 0.6f ? 10 : 50);

        float w = zoom * camera.aspect * 2, h = zoom * 2;
        Vector3 pos = camera.transform.position;
        Vector3 origin = grid.transform.position;

        float left = pos.x - w / 2, right = pos.x + w / 2;
        float bottom = pos.y - h / 2, top = pos.y + h / 2;

        float startX = Mathf.Floor((left - origin.x) / visualSize) * visualSize + origin.x;
        float endX = Mathf.Ceil((right - origin.x) / visualSize) * visualSize + origin.x;
        float startY = Mathf.Floor((bottom - origin.y) / visualSize) * visualSize + origin.y;
        float endY = Mathf.Ceil((top - origin.y) / visualSize) * visualSize + origin.y;

        var verts = new List<Vector3>();
        var inds = new List<int>();

        for (float x = startX; x <= endX; x += visualSize)
        {
            verts.Add(new Vector3(x, startY));
            verts.Add(new Vector3(x, endY));
            inds.Add(verts.Count - 2); inds.Add(verts.Count - 1);
        }

        for (float y = startY; y <= endY; y += visualSize)
        {
            verts.Add(new Vector3(startX, y));
            verts.Add(new Vector3(endX, y));
            inds.Add(verts.Count - 2); inds.Add(verts.Count - 1);
        }

        _mesh.vertices = verts.ToArray();
        _mesh.SetIndices(inds.ToArray(), MeshTopology.Lines, 0);
    }
}