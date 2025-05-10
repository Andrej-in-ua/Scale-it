using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

namespace UI.Game.Inventory
{
    public class UIInventory : MonoBehaviour
    {
        public RectTransform _bottomPanel;
        
        [SerializeField] private float _cardScaleInInventory = 0.8f;
        [SerializeField] private int _maxSlots = 11;
        [SerializeField] private int _maxCardsPerStack = 5;
        [SerializeField] private Canvas _canvas;
        
        private readonly List<List<GameObject>> _cards = new();

        public void AddCard(RectTransform card)
        {
            GameObject cardGO = card.gameObject;
            bool addedToStack = false;
            bool foundMatchingStack = false;

            // foreach (var stack in _cards)
            // {
            //     if (stack.Count > 0 && AreCardsEqual(stack[0], cardGO) && stack.Count < _maxCardsPerStack)
            //     {
            //         AddToExistingStack(card, stack);
            //         addedToStack = foundMatchingStack = true;
            //         break;
            //     }
            // }

            if (!addedToStack && _cards.Count < _maxSlots)
            {
                _cards.Add(new List<GameObject> { cardGO });
                addedToStack = true;
            }

            if (addedToStack)
            {
                PlaceCard(card, foundMatchingStack);
                LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
            }
        }

        private void AddToExistingStack(RectTransform card, List<GameObject> stack)
        {
            GameObject cardGO = card.gameObject;

            card.SetParent(stack[0].transform);
            card.position = stack[0].transform.position;

            card.DOScale(1f, 0.25f).From(card.localScale).SetEase(Ease.OutSine);

            // GameObject counter = stack[0].GetComponent<DragCard>().GetCardsCountWindow();
            // counter.SetActive(true);
            // counter.transform.SetAsLastSibling();
            //
            // stack.Add(cardGO);
            //
            // TMP_Text countText = counter.transform.GetChild(0).GetComponent<TMP_Text>();
            //countText.text = stack.Count.ToString();
        }

        private void PlaceCard(RectTransform card, bool inStack)
        {
            if (!inStack)
            {
                card.SetParent(transform);
                card.localScale = Vector3.one * _cardScaleInInventory;
            }
            else
            {
                card.localScale = Vector3.one;
            }
        }

        // private bool AreCardsEqual(GameObject a, GameObject b)
        // {
        //     /return a.GetComponent<DragCard>().GetCardIndex() == b.GetComponent<DragCard>().GetCardIndex();
        // }

        public void RemoveCard(RectTransform card, Transform newParent)
        {
            GameObject cardGO = card.gameObject;

            SetNewParent(card, newParent.gameObject);

            for (int i = _cards.Count - 1; i >= 0; i--)
            {
                // var stack = _cards[i];
                // if (!stack.Contains(cardGO)) continue;
                //
                // stack.Remove(cardGO);
                //
                // if (stack.Count > 0)
                // {
                //     GameObject counter = stack[0].GetComponent<DragCard>().GetCardsCountWindow();
                //     counter.SetActive(stack.Count > 1);
                //     counter.transform.SetAsLastSibling();
                //
                //     TMP_Text countText = counter.transform.GetChild(0).GetComponent<TMP_Text>();
                //     countText.text = stack.Count.ToString();
                // }
                // else
                // {
                //     GetComponent<HorizontalLayoutGroup>().enabled = false;
                //     cardGO.GetComponent<DragCard>().GetCardsCountWindow().SetActive(false);
                //     _cards.RemoveAt(i);
                // }

                break;
            }
        }

        private void SetNewParent(RectTransform card, GameObject newParentObj)
        {
            Vector3 oldPos = card.position;

            card.SetParent(newParentObj.transform, true);

            card.anchorMin = Vector2.one * 0.5f;
            card.anchorMax = Vector2.one * 0.5f;
            card.pivot = Vector2.one * 0.5f;

            card.position = oldPos;
        }

        public void MoveCard(RectTransform card, float targetScale, GameObject newParentObj)
        {
            SetNewParent(card, newParentObj);

            card.DOScale(targetScale, 0.25f).From(card.localScale).SetEase(Ease.OutSine);
        }

        public bool IsCursorOnInventory()
        {
            return RectTransformUtility.RectangleContainsScreenPoint(_bottomPanel, Input.mousePosition, Camera.main);
        }

        public bool DoesInventoryHaveFreePlace(GameObject card)
        {
            if (_cards.Count < _maxSlots) return true;

            if (_cards.Count == _maxSlots)
            {
                foreach (var stack in _cards)
                {
                  //  if (AreCardsEqual(stack[0], card) && stack.Count < _maxCardsPerStack)
                        return true;
                }
            }

            return false;
        }

        public void Construct()
        {
            _canvas.worldCamera = Camera.main;
        }

        public HorizontalLayoutGroup GetLayout()
        {
            return GetComponent<HorizontalLayoutGroup>();
        }
    }
}
