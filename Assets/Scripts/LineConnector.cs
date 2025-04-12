using System.Collections.Generic;
using System.Threading.Tasks;
using GameTable;
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
    private LineRenderer _lineRenderer, _lineForPathCreation;
    private LinePathFinder _linePathFinder;
    private Camera _camera;

    private LineConnector _connectedWith;

    private Vector3 _lastStartPos;
    private Vector3 _lastEndPos;

    private List<Vector3> _foundPath = new();

    private bool _isDrawing = false;
    private bool _isCalculatingPath = false;

    private void Start()
    {
        _lineForPathCreation = GameObject.FindGameObjectWithTag("LineForPathCreation").GetComponent<LineRenderer>();
        _lineForPathCreation.useWorldSpace = true;

        _linePathFinder = FindAnyObjectByType<LinePathFinder>();
        _lineParentObject = GameObject.FindGameObjectWithTag("ParentForCreatedLines").gameObject;

        _camera = Camera.main;
    }

    private void Update()
    {
        if (!_isDrawing) return;

        FindPathTask();
        DrawPreviewLine();
    }

    private async void FindPathTask()
    {
        if (_isCalculatingPath) return;

        Vector3 start = transform.position;
        Vector3 end = GetWorldMousePosition();

        // Prevent recalculation when pointer not moved 
        if (
            Vector3.Distance(_lastStartPos, start) < 0.5f &&
            Vector3.Distance(_lastEndPos, end) < 0.5f
        ) return;

        _lastStartPos = start;
        _lastEndPos = end;

        _isCalculatingPath = true;

        try
        {
            _foundPath = await Task.Run(() => _linePathFinder.FindPath(start, end));
        }
        finally
        {
            _isCalculatingPath = false;
        }
    }

    private void DrawPreviewLine()
    {
        if (!_isDrawing || _foundPath.Count <= 0) return;

        _lineForPathCreation.positionCount = _foundPath.Count;
        _lineForPathCreation.SetPositions(_foundPath.ToArray());
    }

    private Vector3 GetWorldMousePosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 0;

        Vector3 worldPos = _camera.ScreenToWorldPoint(mousePos);
        worldPos.z = 0;

        return worldPos;
    }

    public void CanDrawLine()
    {
        _isDrawing = true;

        DisconnectBoth();

        _lineForPathCreation.gameObject.SetActive(true);
        _lineForPathCreation.positionCount = 0;
        _foundPath.Clear();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isDrawing = false;

        LineConnector targetButton = GetButtonUnderCursor(eventData);

        if (targetButton != null && targetButton != this && IsValidConnection(targetButton))
        {
            CreateFinalLine(targetButton);
        }

        _lineForPathCreation.positionCount = 0;
        _foundPath.Clear();
        _lineForPathCreation.gameObject.SetActive(false);
    }

    private async void CreateFinalLine(LineConnector targetLineConnector)
    {
        Vector3 start = transform.position;
        Vector3 end = targetLineConnector.transform.position;

        var path = await Task.Run(() => _linePathFinder.FindPath(start, end));

        if (path.Count == 0) return;

        Connect(targetLineConnector);
        targetLineConnector.Connect(this);

        _lineRenderer = Instantiate(_linePrefab, _lineParentObject.transform, true);
        _lineRenderer.positionCount = path.Count;
        _lineRenderer.SetPositions(path.ToArray());
    }

    public void Connect(LineConnector connector)
    {
        DisconnectBoth();
        _connectedWith = connector;
    }

    private void DisconnectBoth()
    {
        if (_connectedWith != null)
        {
            _connectedWith.Disconnect();
        }

        Disconnect();
    }

    public void Disconnect()
    {
        _connectedWith = null;

        if (_lineRenderer != null)
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

    private bool IsValidConnection(LineConnector connector)
    {
        var validConnections = new Dictionary<PortType, PortType>
        {
            { PortType.Input, PortType.Output },
            { PortType.Output, PortType.Input }
        };

        return validConnections.TryGetValue(_portType, out var expectedType)
               && connector._portType == expectedType;
    }
}