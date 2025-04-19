using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace GameTable
{
    public class Inventory : MonoBehaviour
    {
        [SerializeField] private float _cardScaleInInventory;
        [SerializeField] private RectTransform _bottomPanle;
         
        public List<GameObject> _cards = new List<GameObject>();

        public void AddCard(RectTransform card)
        {
            if (_cards.Count < 11)
            {
                card.SetParent(transform);

                card.localScale = new Vector3(_cardScaleInInventory, _cardScaleInInventory, _cardScaleInInventory);

                if (!_cards.Contains(card.gameObject))
                {
                    _cards.Add(card.gameObject);
                }
                else
                {
                    LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
                }
            }
        }

        public void RemoveCard(RectTransform card, Transform parentObject)
        {
            card.SetParent(parentObject);

            card.anchorMin = new Vector2(0.5f, 0.5f);
            card.anchorMax = new Vector2(0.5f, 0.5f);
            card.pivot = new Vector2(0.5f, 0.5f);

            card.localScale = new Vector3(1, 1, 1);

            for (int i = _cards.Count - 1; i >= 0; i--)
            {
                if (card.gameObject == _cards[i])
                {
                    _cards.RemoveAt(i);
                }
            }
        }

        public void MoveCard(RectTransform card, float targetScale, GameObject newParentObj)
        {
            Vector3 oldPos = card.transform.position;

            card.anchorMin = new Vector2(0.5f, 0.5f);
            card.anchorMax = new Vector2(0.5f, 0.5f);
            card.pivot = new Vector2(0.5f, 0.5f);

            card.SetParent(newParentObj.transform, true);

            card.position = oldPos;

            Vector3 currentLocalScale = card.localScale;
            card.DOScale(targetScale, 0.25f).From(currentLocalScale).SetEase(Ease.OutSine);
        }

        public bool IsCursorOnInventory()
        {
            return RectTransformUtility.RectangleContainsScreenPoint(
                _bottomPanle,
                Input.mousePosition,
                Camera.main
            );
        }
    }
}