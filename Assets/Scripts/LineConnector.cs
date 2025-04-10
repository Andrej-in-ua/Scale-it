using System.Collections.Generic;
using System.Threading.Tasks;
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
    private LinePathCreation _linePathCreation;

    private Vector3 _lastStartPos;
    private Vector3 _lastEndPos;

    private List<Vector3> _currentPath = new();

    private bool _isDrawing = false, _isCalculatingPath = false, _canCalculatePathAsynchronously = true;

    private void Start()
    {
        _lineForPathCreation = GameObject.FindGameObjectWithTag("LineForPathCreation").GetComponent<LineRenderer>();
        _lineForPathCreation.useWorldSpace = true;

        _linePathCreation = FindAnyObjectByType<LinePathCreation>();
        _lineParentObject = GameObject.FindGameObjectWithTag("ParentForCreatedLines").gameObject;
    }

    private async void Update()
    {
        if (!_isDrawing || _isCalculatingPath) return;

        Vector3 worldMousePos = GetWorldMousePosition();
        Vector3 start = transform.position;
        Vector3 end = worldMousePos;

        if (HasMovedEnough(start, end))
        {
            _lastStartPos = start;
            _lastEndPos = end;
            _isCalculatingPath = true;

            _currentPath = _canCalculatePathAsynchronously
                ? await Task.Run(() => _linePathCreation.FindPath(start, end))
                : _linePathCreation.FindPath(start, end);

            _isCalculatingPath = false;
            _canCalculatePathAsynchronously = true;
        }

        if (_currentPath.Count > 0)
        {
            DrawPreviewLine(_currentPath);
        }
    }

    private Vector3 GetWorldMousePosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 0;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        worldPos.z = 0;
        return worldPos;
    }

    private bool HasMovedEnough(Vector3 start, Vector3 end)
    {
        return Vector3.Distance(_lastStartPos, start) > 0.5f ||
               Vector3.Distance(_lastEndPos, end) > 0.5f;
    }

    private void DrawPreviewLine(List<Vector3> path)
    {
        _lineForPathCreation.positionCount = path.Count;
        _lineForPathCreation.SetPositions(path.ToArray());
    }

    public void CanDrowLine()
    {
        _isDrawing = true;

        if (_lineRenderer != null)
        {
            Destroy(_lineRenderer.gameObject);
        }

        _lineForPathCreation.gameObject.SetActive(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isDrawing = false;
        LineConnector targetButton = GetButtonUnderCursor(eventData);

        if (targetButton != null && targetButton != this)
        {
            CreateFinalLine();
            Connection(targetButton, _lineRenderer);
            _canCalculatePathAsynchronously = false;
        }

        _lineForPathCreation.gameObject.SetActive(false);
    }

    private void CreateFinalLine()
    {
        _lineRenderer = Instantiate(_linePrefab, _lineParentObject.transform, true);
        _lineRenderer.positionCount = _lineForPathCreation.positionCount;

        Vector3[] pathCopy = new Vector3[_lineForPathCreation.positionCount];
        _lineForPathCreation.GetPositions(pathCopy);
        _lineRenderer.SetPositions(pathCopy);
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

    private void Connection(LineConnector connector, LineRenderer lineRenderer)
    {
        var validConnections = new Dictionary<PortType, PortType>
        {
        { PortType.Input, PortType.Output },
        { PortType.Output, PortType.Input }
        };

        if (validConnections.TryGetValue(_portType, out var expectedType) && connector._portType == expectedType)
        {
            connector.SetLine(lineRenderer);
        }
        else
        {
            Destroy(_lineRenderer.gameObject);
        }
    }
}
