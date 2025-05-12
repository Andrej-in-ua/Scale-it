using System.Collections.Generic;
using UnityEngine;

namespace View.GameTable
{
    public class CardViewPool
    {
        private readonly CardViewFactory _cardViewFactory;

        private Transform _container;
        private Queue<CardView> _pool = new Queue<CardView>();

        public CardViewPool(CardViewFactory cardViewFactory)
        {
            _cardViewFactory = cardViewFactory;
        }

        public void Construct(int initialSize = 32)
        {
            _container = new GameObject("CardViewPool").transform;
            _container.SetParent(null);

            for (var i = 0; i < initialSize; i++)
            {
                var cardView = CreateNew(-1);
                cardView.gameObject.SetActive(false);
                _pool.Enqueue(cardView);
            }
        }

        public CardView GetCardView(int cardID, bool active = false)
        {
            AssertConstructed();

            var cardView = _pool.Count > 0
                ? _pool.Dequeue()
                : CreateNew(cardID);

            cardView.SetCardId(cardID);
            cardView.gameObject.SetActive(active);

            return cardView;
        }

        public void ReturnCardView(CardView cardView)
        {
            AssertConstructed();

            cardView.gameObject.SetActive(false);
            _pool.Enqueue(cardView);
        }

        private CardView CreateNew(int cardID)
        {
            return _cardViewFactory.Create(_container, cardID);
        }

        private void AssertConstructed()
        {
            if (_container == null)
                throw new System.Exception("CardViewPool is not constructed. Call Construct() first.");
        }

        public void Destruct()
        {
            if (_container != null)
            {
                if (_container.gameObject)
                {
                    Object.Destroy(_container.gameObject);
                }

                _container = null;
            }
        }
    }
}