using UnityEngine;
using UnityEngine.EventSystems;

namespace GameTable
{
    public class DragCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private GridManager _gridManager;
        private RectTransform _rectTransform;
        private Canvas _canvas;

        private Vector2 _oldPosition = Vector2.zero;
        private Vector2 _originalPosition, _dragOffset;

        private static readonly Vector2Int CardSize = new Vector2Int(5, 7);

        private bool _isDragging = false, _canDrag = false;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _gridManager = FindObjectOfType<GridManager>();
            _canvas = GetComponentInParent<Canvas>();
            _oldPosition = _rectTransform.anchoredPosition;
        }

        private void Start()
        {
            PlaceCard();
        }

        public void CardCanDrag()
        {
            _canDrag = true;
            _oldPosition = _rectTransform.anchoredPosition;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_canDrag)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _rectTransform.parent as RectTransform,
                    eventData.position,
                    _canvas.worldCamera,
                    out _originalPosition
                );
                _dragOffset = _originalPosition - _rectTransform.anchoredPosition;
                _isDragging = true;
                _canDrag = false;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_isDragging)
            {
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _rectTransform.parent as RectTransform,
                    eventData.position,
                    _canvas.worldCamera,
                    out Vector2 currentPosition
                ))
                {
                    _rectTransform.anchoredPosition = currentPosition - _dragOffset;
                }
                _rectTransform.SetAsLastSibling();
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _isDragging = false;
            
            PlaceCard();
        }

        private void PlaceCard()
        {
            Vector2? newPosition = _gridManager.PlaceOnGrid(
                gameObject.GetInstanceID(),
                _rectTransform.anchoredPosition,
                CardSize
            );

            _rectTransform.anchoredPosition = newPosition ?? _oldPosition;
        }
    }
}