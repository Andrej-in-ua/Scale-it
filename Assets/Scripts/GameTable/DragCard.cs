using UnityEngine;

namespace GameTable
{
    public class DragCard : MonoBehaviour
    {
        private GridManager _gridManager;
        private RectTransform _rectTransform;
        private Canvas _canvas;

        private Vector2 _oldPosition = Vector2.zero;
        private Vector2 _dragOffset;

        private static readonly Vector2Int CardSize = new Vector2Int(5, 7);

        private bool _followCursor = false, _offsetInitialized = false;

        private void Start()
        {
            _rectTransform = GetComponent<RectTransform>();
            _gridManager = FindObjectOfType<GridManager>();
            _canvas = GetComponentInParent<Canvas>();
            _oldPosition = _rectTransform.anchoredPosition;

            PlaceCard();
        }

        private void Update()
        {
            if (_followCursor)
            {
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _rectTransform.parent as RectTransform,
                    Input.mousePosition,
                    _canvas.worldCamera,
                    out Vector2 localMousePos))
                {
                    if (!_offsetInitialized)
                    {
                        _dragOffset = localMousePos - _rectTransform.anchoredPosition;
                        _offsetInitialized = true;
                    }

                    _rectTransform.anchoredPosition = localMousePos - _dragOffset;
                    _rectTransform.SetAsLastSibling();
                }
            }
        }


        public void FollowCursorWithoutClick()
        {
            _dragOffset = Vector2.zero;
            _followCursor = true;
            _offsetInitialized = false;
        }

        public void StopFollowingCursor()
        {
            _followCursor = false;
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