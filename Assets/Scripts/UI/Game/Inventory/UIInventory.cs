using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UI.Game.CardPreviews;

namespace UI.Game.Inventory
{
    public class UIInventory : MonoBehaviour
    {
        public RectTransform _bottomPanel;

        [SerializeField] private Canvas _canvas;

        private Dictionary<int, int> _cardsQuantity;
        private Dictionary<int, UICardPreview> _cardPreviews;

        public void Construct()
        {
            _cardsQuantity = new Dictionary<int, int>();
            _cardPreviews = new Dictionary<int, UICardPreview>();

            _canvas.worldCamera = Camera.main;
            _canvas.sortingLayerName = "Inventory";
        }

        public bool Take(UICardPreview cardPreview) => Take(cardPreview.CardId);

        public bool Take(int cardId)
        {
            if (!_cardsQuantity.ContainsKey(cardId) || _cardsQuantity[cardId] <= 0) return false;

            _cardsQuantity[cardId]--;
            Debug.Log("Take card " + cardId + ", left: " + _cardsQuantity[cardId]);
            UpdateCardCounter(cardId);
            return true;
        }

        public void Put(UICardPreview card)
        {
            if (!_cardsQuantity.ContainsKey(card.CardId))
            {
                _cardsQuantity[card.CardId] = 1;
                _cardPreviews.Add(card.CardId, card);
                card.transform.SetParent(_bottomPanel.transform);
            }
            else
            {
                _cardsQuantity[card.CardId]++;
                Destroy(card.gameObject);
            }
            
            UpdateCardCounter(card.CardId);
        }

        private void UpdateCardCounter(int cardId)
        {
            switch (_cardsQuantity[cardId])
            {
                case <= 0:
                    _cardsQuantity.Remove(cardId);
                    Destroy(_cardPreviews[cardId].gameObject);
                    _cardPreviews.Remove(cardId);
                    break;
                case 1:
                    _cardPreviews[cardId].CardsCountWindow.SetActive(false);
                    break;
                case > 1:
                {
                    TMP_Text cardsCountText = _cardPreviews[cardId].CardsCountWindow.transform.GetChild(0)
                        .GetComponent<TMP_Text>();

                    if (cardsCountText != null)
                        cardsCountText.text = _cardsQuantity[cardId].ToString();

                    _cardPreviews[cardId].CardsCountWindow.SetActive(true);
                    break;
                }
            }

            LayoutRebuilder.MarkLayoutForRebuild(_bottomPanel);
        }
    }
}