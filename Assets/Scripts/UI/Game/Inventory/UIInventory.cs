using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameTable;
using TMPro;
using UI.Game.CardPreviews;

namespace UI.Game.Inventory
{
    public class UIInventory : MonoBehaviour
    {
        public RectTransform _bottomPanel;

        [SerializeField] private int _maxCardsPerStack = 5;
        [SerializeField] private Canvas _canvas;

        private readonly List<(DragCard card, int stack)> _cards = new();

        private IUICardFactory _uiCardFactory;

        public GameObject TakeCardFromStack(DragCard card)
        {
            UICardPreview spawnedCard = _uiCardFactory.CreateUICard(_bottomPanel, "Card from stack");

            for (int i = 0; i < _cards.Count; i++)
            {
                if (_cards[i].card == card)
                {
                    var existing = _cards[i];
                    existing.stack--;

                    _cards[i] = existing;

                    if (existing.stack > 1)
                    {
                        TMP_Text cardsCountText = card.CardsCountWindow.transform.GetChild(0).GetComponent<TMP_Text>();
                        if (cardsCountText != null)
                            cardsCountText.text = existing.stack.ToString();
                    }
                    else
                    {
                        card.CardsCountWindow.SetActive(false);
                    }

                    break;
                }
            }
            return spawnedCard.gameObject;
        }

        public void AddCardToInventory(DragCard newCard)
        {
            var (inventoryHaveSameCard, sameCardInInventory, cardIndex) = DoesInventoryHaveSameCard(newCard);

            if (inventoryHaveSameCard)
            {
                AddCardToExistingStack(sameCardInInventory, newCard, cardIndex);
            }
            else
            {
                _cards.Add((newCard, 1));
            }

            newCard.transform.SetParent(_bottomPanel.transform);
            LayoutRebuilder.MarkLayoutForRebuild(_bottomPanel);
        }

        private void AddCardToExistingStack(DragCard sameCardInInventory, DragCard newCard, int cardIndex)
        {
            if (cardIndex < 0 || cardIndex >= _cards.Count)
                return;

            var existing = _cards[cardIndex];

            if (existing.stack < _maxCardsPerStack)
            {
                existing.stack++;
                _cards[cardIndex] = existing;

                sameCardInInventory.CardsCountWindow.SetActive(true);

                TMP_Text cardsCountText =
                    sameCardInInventory.CardsCountWindow.transform.GetChild(0).GetComponent<TMP_Text>();
                cardsCountText.text = existing.stack.ToString();

                Destroy(newCard.gameObject);
            }
        }

        private bool AreCardsSame(DragCard firstCard, DragCard secondCard)
        {
            return firstCard.Index == secondCard.Index;
        }

        public bool IsCursorOnInventory()
        {
            return RectTransformUtility.RectangleContainsScreenPoint(_bottomPanel, Input.mousePosition, Camera.main);
        }

        public (bool, DragCard, int) DoesInventoryHaveSameCard(DragCard card)
        {
            for (int i = 0; i < _cards.Count; i++)
            {
                if (AreCardsSame(_cards[i].card, card) && _cards[i].card != card && _cards[i].stack < _maxCardsPerStack)
                {
                    return (true, _cards[i].card, i);
                }
            }
            return (false, null, 0);
        }

        public bool DoesCardHaveStack(DragCard card)
        {
            for (int i = 0; i < _cards.Count; i++)
            {
                if (_cards[i].card == card)
                {
                    if (_cards[i].stack > 1)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void Construct(IUICardFactory uiCardFactory)
        {
            _uiCardFactory = uiCardFactory;
            _canvas.worldCamera = Camera.main;
        }
    }
}