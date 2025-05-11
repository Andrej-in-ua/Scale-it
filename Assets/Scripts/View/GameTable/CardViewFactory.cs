using ECS.Components;
using Services;
using Unity.Entities;
using UnityEngine;

namespace View.GameTable
{
    public class CardViewFactory
    {
        private readonly IAssetProviderService _assetProviderService;

        private GameObject CardEntityPrefab => _cardEntityPrefab ??=
            _assetProviderService.LoadAssetFromResources<GameObject>(Constants.CardViewPath);

        private GameObject _cardEntityPrefab;

        public CardViewFactory(IAssetProviderService assetProviderService)
        {
            _assetProviderService = assetProviderService;
        }

        public CardView Create(int cardId, Vector3 worldPosition = default)
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var entity = entityManager.CreateEntity();

            var cardComponent = new CardComponent(cardId);
            entityManager.AddComponentData(entity, cardComponent);

            var view = Object.Instantiate(CardEntityPrefab, worldPosition, Quaternion.identity);
            view.name = "Card_" + cardComponent.Card.cardID;

            var cardView = view.GetComponent<CardView>();
            cardView.BakeEntity(entity, entityManager);
            
            cardView.Name.text = cardComponent.Card.name;

            entityManager.AddComponentObject(entity, view);

            return cardView;
        }
    }
}