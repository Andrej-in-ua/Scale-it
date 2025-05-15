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
        [SerializeField] private Canvas _canvas;
        //TODO change to Dictionary (CardType, CardAmount)
        private readonly List<(DragCard card, int stack)> _cards = new();

        public void TakeCardFromStack(int cardId)
        {
            for (int i = 0; i < _cards.Count; i++)
            {
                if (_cards[i].card.CardId == cardId)
                {
                    var existing = _cards[i];
                    existing.stack--;
                    _cards[i] = existing;

                    if (existing.stack > 1)
                    {
                        TMP_Text cardsCountText = _cards[i].card.CardsCountWindow.transform.GetChild(0).GetComponent<TMP_Text>();
                        if (cardsCountText != null)
                            cardsCountText.text = existing.stack.ToString();
                    }
                    else
                    {
                        _cards[i].card.CardsCountWindow.SetActive(false);
                    }

                    break;
                }
            }
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
            existing.stack++;
            _cards[cardIndex] = existing;

            sameCardInInventory.CardsCountWindow.SetActive(true);

            TMP_Text cardsCountText =
                sameCardInInventory.CardsCountWindow.transform.GetChild(0).GetComponent<TMP_Text>();
            cardsCountText.text = existing.stack.ToString();

            Destroy(newCard.gameObject);
        }

        private bool AreCardsSame(DragCard firstCard, DragCard secondCard)
        {
            return firstCard.CardId == secondCard.CardId;
        }

        public bool IsCursorOnInventory()
        {
            return RectTransformUtility.RectangleContainsScreenPoint(_bottomPanel, Input.mousePosition, Camera.main);
        }

        public (bool, DragCard, int) DoesInventoryHaveSameCard(DragCard card)
        {
            for (int i = 0; i < _cards.Count; i++)
            {
                if (AreCardsSame(_cards[i].card, card) && _cards[i].card != card)
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

        public void Construct()
        {
            _canvas.worldCamera = Camera.main;
        }
    }
}