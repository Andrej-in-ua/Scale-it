using UnityEngine;

namespace GameTable
{
    public class DragCard : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed;

        private GameObject _bottomPanel;
        private Inventory _inventory;
        private GridManager _gridManager;
        private RectTransform _rectTransform;
        private Canvas _canvas;

        private Vector2 _originalPosition = Vector2.zero;
        private Vector2 _dragOffset;

        private static readonly Vector2Int CardSize = new Vector2Int(5, 7);

        private bool _isFollowingCursor = false;
        private bool _isOffsetInitialized = false;
        private bool _isMovedToInventory = false;
        private bool _isMovingDirectlyToCursor = false;

        private void Start()
        {
            _rectTransform = GetComponent<RectTransform>();
            _gridManager = FindObjectOfType<GridManager>();
            _inventory = FindObjectOfType<Inventory>();
            _canvas = _gridManager.GetComponent<Canvas>();

            _bottomPanel = GameObject.FindGameObjectWithTag("BottomPanel");

            PlaceCard();
            _originalPosition = _rectTransform.anchoredPosition;
        }

        private void Update()
        {
            if (_isFollowingCursor)
            {
                HandleCursorMovement();
            }
        }

        private void HandleCursorMovement()
        {
            bool cursorOnInventory = _inventory.IsCursorOnInventory();

            if (cursorOnInventory)
            {
                MoveToInventory();
            }
            else if (!cursorOnInventory && _isMovedToInventory)
            {
                MoveToCanvas();
            }

            UpdateCardPosition();
        }

        private void MoveToInventory()
        {
            _inventory.MoveCard(_rectTransform, 0.5f, _bottomPanel);
            _isMovedToInventory = true;
            _isOffsetInitialized = false;
            _isMovingDirectlyToCursor = true;
        }

        private void MoveToCanvas()
        {
            _inventory.MoveCard(_rectTransform, 1f, _canvas.gameObject);
            _isMovedToInventory = false;
            _isOffsetInitialized = false;
            _isMovingDirectlyToCursor = true;
        }

        private void UpdateCardPosition()
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _rectTransform.parent as RectTransform,
                Input.mousePosition,
                _canvas.worldCamera,
                out Vector2 localMousePos))
            {
                if (!_isOffsetInitialized)
                {
                    _dragOffset = localMousePos - _rectTransform.anchoredPosition;
                    _isOffsetInitialized = true;
                }

                Vector2 targetPos = _isMovingDirectlyToCursor
                    ? localMousePos
                    : localMousePos - _dragOffset;

                _rectTransform.anchoredPosition = Vector2.Lerp(
                    _rectTransform.anchoredPosition,
                    targetPos,
                    Time.deltaTime * _moveSpeed
                );

                _rectTransform.SetAsLastSibling();
            }
        }

        public void FollowCursorWithoutClick()
        {
            _dragOffset = Vector2.zero;
            _isFollowingCursor = true;
            _isOffsetInitialized = false;
        }

        public void StopFollowingCursor()
        {
            _isFollowingCursor = false;
            _isMovingDirectlyToCursor = false;

            if (_inventory.IsCursorOnInventory())
            {
                _inventory.AddCard(_rectTransform);
            }
            else
            {
                PlaceCard();
                _inventory.RemoveCard(_rectTransform, _canvas.transform);
            }
        }

        private void PlaceCard()
        {
            Vector2? newPosition = _gridManager.PlaceOnGrid(
                gameObject.GetInstanceID(),
                _rectTransform.anchoredPosition,
                CardSize
            );

            _rectTransform.anchoredPosition = newPosition ?? _originalPosition;
        }
    }
}
