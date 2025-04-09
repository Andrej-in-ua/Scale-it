using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum PortType
{
    Input,
    Output
}

public class LineConnector : MonoBehaviour, IPointerUpHandler
{
    [SerializeField] private PortType _portType;
    [SerializeField] private LineRenderer _linePrefab;

    private GameObject _lineParentObject;

    private LineRenderer _lineRenderer;
    private LinePathCreation _linePathCreation;
    private bool _isDrawing = false;

    private Vector3 _lastStartPos;
    private Vector3 _lastEndPos;
    private List<Vector3> _currentPath = new();

    private void Awake()
    {
        _linePathCreation = FindAnyObjectByType<LinePathCreation>();
        _lineParentObject = FindAnyObjectByType<LinePathCreation>().gameObject;
    }

    private void Update()
    {
        if (_isDrawing)
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 0f;
            Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
            worldMousePosition.z = 0f;

            Vector3 currentStart = transform.position;
            Vector3 currentEnd = worldMousePosition;

            if (Vector3.Distance(_lastStartPos, currentStart) > 0.5f ||
                Vector3.Distance(_lastEndPos, currentEnd) > 0.5f)
            {
                _lastStartPos = currentStart;
                _lastEndPos = currentEnd;

                _currentPath = _linePathCreation.FindPath(currentStart, currentEnd);
            }

            if (_currentPath.Count > 0)
            {
                _lineRenderer.positionCount = _currentPath.Count;
                _lineRenderer.SetPositions(_currentPath.ToArray());
            }
        }
    }

    public void CanDrowLine()
    {
        _isDrawing = true;

        if (_lineRenderer != null)
        {
            Destroy(_lineRenderer.gameObject);
        }

        _lineRenderer = Instantiate(_linePrefab);
        _lineRenderer.transform.SetParent(_lineParentObject.transform, true);
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
            Destroy(_lineRenderer.gameObject);
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
        { PortType.Output, PortType.Input }
        };

        if (validConnections.TryGetValue(_portType, out var expectedType) && Connector._portType == expectedType)
        {
            Connector.SetLine(lineRenderer);
        }
        else
        {
            Destroy(_lineRenderer.gameObject);
        }
    }
}
