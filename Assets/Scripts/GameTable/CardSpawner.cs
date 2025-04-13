using TMPro;
using UnityEngine;

namespace GameTable
{
    public class CardSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _parentCanvasForCard;
        [SerializeField] private TMP_Dropdown _IDCardDropdown;
        [SerializeField] private GameObject[] _cards;

        private DragCard _lastSpawnedCard;

        public void Spawn(Transform positionForSpawnCard)
        {
            Vector3 worldPos = positionForSpawnCard.position;
            Vector2 localPos;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _parentCanvasForCard.GetComponent<RectTransform>(),
                RectTransformUtility.WorldToScreenPoint(null, worldPos),
                null,
                out localPos
            );

            GameObject card = Instantiate(_cards[_IDCardDropdown.value].gameObject);

            Debug.Log(_IDCardDropdown.value);

            card.transform.SetParent(_parentCanvasForCard.transform, false);
            card.transform.SetAsLastSibling();

            card.GetComponent<RectTransform>().anchoredPosition = localPos;

            card.GetComponent<DragCard>().FollowCursorWithoutClick();

            _lastSpawnedCard = card.GetComponent<DragCard>();
        }

        public void DropCard()
        {
            if (_lastSpawnedCard != null)
            {
                _lastSpawnedCard.StopFollowingCursor();
                _lastSpawnedCard = null;
            }
        }
    }
}

