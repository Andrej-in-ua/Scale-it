using System;
using Services;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Rendering;

namespace View.GameTable
{
    public class CardView : MonoBehaviour, IDraggable
    {
        public TMP_Text Name => GetComponentInChildren<TMP_Text>();
        public SortingGroup SortingGroup => GetComponent<SortingGroup>();
        
        public PlaceableReference PlaceableReference => new PlaceableReference
        {
            Object = this,
            ObjectSize = new Vector2Int(5, 7),
            Padding = 1,
            CellScale = 3,
        };

        public event Action<CardView> OnCardViewEnable;
        public event Action<CardView> OnCardViewDisable;
        public event Action<CardView> OnCardViewDestroy;

        public int CardId { get; private set; }
        public Entity Entity { get; private set; }
        public EntityManager EntityManager { get; private set; }
        
        public void BakeEntity(int cardId, Entity entity, EntityManager entityManager)
        {
            if (Entity != default)
                throw new System.Exception("Entity has already been set");

            CardId = cardId;
            Entity = entity;
            EntityManager = entityManager;
        }
        
        public void SetCardId(int cardId)
        {
            AssertBaked();
            
            if (gameObject.activeSelf)
                throw new System.Exception("Cannot set cardId when CardView is active");

            CardId = cardId;
        }

        private void OnEnable()
        {
            AssertBaked();

            OnCardViewEnable?.Invoke(this);
        }

        private void OnDisable()
        {
            AssertBaked();

            OnCardViewDisable?.Invoke(this);
        }

        private void OnDestroy()
        {
            if (World.DefaultGameObjectInjectionWorld is { IsCreated: true } && EntityManager.Exists(Entity))
            {
                OnCardViewDestroy?.Invoke(this);
            }
        }

        private void AssertBaked()
        {
            if (Entity == default)
                throw new System.Exception("Entity is not baked");
        
            if (!EntityManager.Exists(Entity))
                throw new System.Exception("Entity has already been destroyed");
        }
    }
}
