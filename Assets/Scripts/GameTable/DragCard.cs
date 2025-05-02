using UnityEngine;

namespace GameTable
{
    public class DragCard : MonoBehaviour
    {
        [SerializeField] private GameObject _cardsCountWindow;

        [SerializeField] private float _moveSpeed;
        [SerializeField] private int _index;

        private GameObject _bottomPanel;
        private Inventory _inventory;
        private GridManager _gridManager;
        private RectTransform _rectTransform;
        private Canvas _canvas;

        private Vector2 _originalPosition = Vector2.zero;
        private Vector2 _dragOffset;

        private static readonly Vector2Int CardSize = new Vector2Int(5, 7);

        private bool _isFollowingCursor = false, _isMovingDirectlyToCursor = false,
            _isMovedToInventory = false, _isOffsetInitialized = false;

        private void Start()
        {
            _rectTransform = GetComponent<RectTransform>();
            _gridManager = FindObjectOfType<GridManager>();
            _inventory = FindObjectOfType<Inventory>();
            _canvas = _gridManager.GetComponent<Canvas>();

            _bottomPanel = GameObject.FindGameObjectWithTag("BottomPanel");

            _originalPosition = _rectTransform.anchoredPosition;

            if (!_inventory.IsCursorOnInventory())
            {
                PlaceCard();
            }
        }

        private void Update()
        {
            if (_isFollowingCursor)
            {
                HandleCursorMovement();
            }
        }

        public int GetCardIndex()
        {
            return _index;
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
                MoveToTable();
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

        private void MoveToTable()
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
            if (_canvas != null)
            {
                _inventory.RemoveCard(_rectTransform, _canvas.transform);
                _gridManager.RemoveFromGrid(gameObject.GetInstanceID());
            }

            _dragOffset = Vector2.zero;
            _isFollowingCursor = true;
            _isOffsetInitialized = false;
        }

        public void StopFollowingCursor()
        {
            _isFollowingCursor = false;
            _isMovingDirectlyToCursor = false;

            _inventory.GetLayout().enabled = true;

            if (_inventory.IsCursorOnInventory())
            {
                if (_inventory.DoesInventoryHaveFreePlace(gameObject))
                {
                    _inventory.AddCard(_rectTransform);
                    return;
                }

                MoveToTable();
            }

            PlaceCard();
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

        public GameObject GetCardsCountWindow()
        {
            return _cardsCountWindow;
        }

        public bool IsCardInInventory()
        {
            return _isMovedToInventory;
        }
    }
}
