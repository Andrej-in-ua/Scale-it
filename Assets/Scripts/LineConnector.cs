using GameTable;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LineConnector : MonoBehaviour, IPointerUpHandler
{
    private LineRenderer _lineRenderer;
    private GridManager _gridManager;
    private bool _isDrawing = false;

    private void Start()
    {
        _gridManager = GameObject.FindAnyObjectByType<GridManager>();

        GameObject lineObj = new GameObject("DynamicLine");
        _lineRenderer = lineObj.AddComponent<LineRenderer>();

        _lineRenderer.sortingLayerName = "Card";
        _lineRenderer.sortingOrder = 2;

        _lineRenderer.startWidth = 0.1f;
        _lineRenderer.endWidth = 0.1f;
        _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        _lineRenderer.startColor = Color.white;
        _lineRenderer.endColor = Color.white;
    }

    private void Update()
    {
        if (_isDrawing)
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 0f;
            Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
            worldMousePosition.z = 0f;

            List<Vector3> path = _gridManager.FindPath(transform.position, worldMousePosition);

            if (path.Count > 0)
            {
                _lineRenderer.positionCount = path.Count;
                _lineRenderer.SetPositions(path.ToArray());
            }
        }
    }

    public void CanDrowLine()
    {
        _isDrawing = true;
        _lineRenderer.positionCount = 2;
        _lineRenderer.SetPosition(0, transform.position);
        _lineRenderer.SetPosition(1, transform.position);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isDrawing = false;

        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 0f;
        Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        worldMousePosition.z = 0f;

        List<Vector3> path = _gridManager.FindPath(transform.position, worldMousePosition);

        Debug.Log(path.Count);

        if (path.Count > 0)
        {
            _lineRenderer.positionCount = path.Count;
            _lineRenderer.SetPositions(path.ToArray());
        }
    }

    private LineConnector GetButtonUnderCursor(PointerEventData eventData)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (RaycastResult result in results)
        {
            LineConnector button = result.gameObject.GetComponent<LineConnector>();
            if (button != null)
            {
                return button;
            }
        }
        return null;
    }
}
