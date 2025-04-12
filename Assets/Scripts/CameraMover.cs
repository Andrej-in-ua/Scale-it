using UnityEngine;

public class CameraMover : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _inertia;

    private Vector2 _currentVelocity;
    private Vector2 _moveInput;

    [Header("Zoom")]
    [SerializeField] private float _zoomSpeed;
    [SerializeField] private float _minZoom;
    [SerializeField] private float _maxZoom;

    private float _targetZoom;

    private Camera _mainCam;
    private CameraControls _controls;

    private void Awake()
    {
        _controls = new CameraControls();
        _mainCam = Camera.main;
        _targetZoom = _mainCam.orthographic ? _mainCam.orthographicSize : _mainCam.transform.position.y;
    }

    private void OnEnable()
    {
        _controls.Camera.Move.performed += ctx => _moveInput = ctx.ReadValue<Vector2>();
        _controls.Camera.Move.canceled += ctx => _moveInput = Vector2.zero;

        _controls.Camera.Zoom.performed += ctx => OnZoom(ctx.ReadValue<float>());

        _controls.Enable();
    }

    private void OnDisable()
    {
        _controls.Disable();
    }

    private void Update()
    {
        _currentVelocity = Vector2.Lerp(_currentVelocity, _moveInput, Time.deltaTime * _inertia);

        float delta = _moveSpeed * Time.deltaTime;
        Vector3 move = new Vector3(_currentVelocity.x, _currentVelocity.y, 0) * delta;
        transform.Translate(move, Space.World);

        if (_mainCam.orthographic)
        {
            _mainCam.orthographicSize = Mathf.Lerp(_mainCam.orthographicSize, _targetZoom, Time.deltaTime * 5f);
        }
        else
        {
            Vector3 camPos = _mainCam.transform.position;
            camPos.y = Mathf.Lerp(camPos.y, _targetZoom, Time.deltaTime * 5f);
            _mainCam.transform.position = camPos;
        }
    }

    private void OnZoom(float scrollDelta)
    {
        _targetZoom -= scrollDelta * _zoomSpeed * Time.deltaTime;
        _targetZoom = Mathf.Clamp(_targetZoom, _minZoom, _maxZoom);
    }
}
