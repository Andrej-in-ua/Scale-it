using GameTable;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum PortType
{
    Input,
    Output,
    FromModifier,
    ForModifier
}

public class LineConnector : MonoBehaviour, IPointerUpHandler
{
    [SerializeField] private PortType _portType;
    [SerializeField] private LineRenderer _linePrefab;

    private LineRenderer _lineRenderer;
    private GridManager _gridManager;
    private bool _isDrawing = false;

    private void Start()
    {
        _gridManager = FindAnyObjectByType<GridManager>();
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

        if (_lineRenderer != null)
        {
            Destroy(_lineRenderer);
        }

        _lineRenderer = Instantiate(_linePrefab);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isDrawing = false;

        LineConnector targetButton = GetButtonUnderCursor(eventData);

        if (targetButton != null && targetButton != this)
        {
            Connection(targetButton, _lineRenderer);
        }
        else
        {
            Destroy(_lineRenderer);
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

    public void SetLine(LineRenderer lineRenderer)
    {
        _lineRenderer = lineRenderer;
    }

    private void Connection(LineConnector Connector, LineRenderer lineRenderer)
    {
        var validConnections = new Dictionary<PortType, PortType>
        {
        { PortType.Input, PortType.Output },
        { PortType.Output, PortType.Input },
        { PortType.ForModifier, PortType.FromModifier },
        { PortType.FromModifier, PortType.ForModifier }
        };

        if (validConnections.TryGetValue(_portType, out var expectedType) && Connector._portType == expectedType)
        {
            Connector.SetLine(lineRenderer);
        }
        else
        {
            Destroy(_lineRenderer);
        }
    }
}
