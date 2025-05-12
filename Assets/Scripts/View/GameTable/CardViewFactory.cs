using ECS.Components;
using Services;
using Unity.Entities;
using UnityEngine;

namespace View.GameTable
{
    public class CardViewFactory
    {
        private readonly IAssetProviderService _assetProviderService;

        private GameObject CardViewPrefab => _cardViewPrefab ??=
            _assetProviderService.LoadAssetFromResourcesForceInactive<GameObject>(Constants.CardViewPath);

        private GameObject _cardViewPrefab;

        public CardViewFactory(IAssetProviderService assetProviderService)
        {
            _assetProviderService = assetProviderService;
        }

        public CardView Create(Transform parent, int cardId)
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var entity = entityManager.CreateEntity();

            var cardViewGameObject = Object.Instantiate(CardViewPrefab, parent);
            entityManager.AddComponentObject(entity, cardViewGameObject);

            var cardView = cardViewGameObject.GetComponent<CardView>();
            cardView.BakeEntity(cardId, entity, entityManager);

            // Card in pool must be reinitialized with default values and another cardId
            cardView.OnCardViewEnable += OnCardViewEnable;
            cardView.OnCardViewDisable += OnCardViewDisable;
            cardView.OnCardViewDestroy += OnCardViewDestroy;

            return cardView;
        }

        private static void OnCardViewEnable(CardView cardView)
        {
            var cardComponent = new CardComponent(cardView.CardId);
            cardView.EntityManager.AddComponentData(cardView.Entity, cardComponent);

            cardView.gameObject.name = "Card_" + cardComponent.Card.cardID;
            cardView.Name.text = cardComponent.Card.name;

            cardView.EntityManager.SetEnabled(cardView.Entity, true);
        }

        private static void OnCardViewDisable(CardView cardView)
        {
            cardView.EntityManager.SetEnabled(cardView.Entity, false);
        }

        private static void OnCardViewDestroy(CardView cardView)
        {
            cardView.EntityManager.DestroyEntity(cardView.Entity);
        }
    }
}