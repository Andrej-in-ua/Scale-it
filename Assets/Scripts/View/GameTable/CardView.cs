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
        
        public event Action<CardView> OnCardViewEnable;
        public event Action<CardView> OnCardViewDisable;
        public event Action<CardView> OnCardViewDestroy;

        public int CardId { get; private set; }
        public Entity Entity { get; private set; }
        public EntityManager EntityManager { get; private set; }

        private SortingGroup _sortingGroup;
        private string _originalSortingLayer;

        private void Awake()
        {
            _sortingGroup = GetComponent<SortingGroup>();
        }

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

        public void OnStartDrag()
        {
            // TODO: Release from grid
            _originalSortingLayer = _sortingGroup.sortingLayerName;
            _sortingGroup.sortingLayerName = IDraggable.DraggableViewSortingLayerName;

            transform.position = new Vector3(transform.position.x, transform.position.y, 100);
        }

        public void OnDrag(Vector3 mousePosition)
        {
            transform.position = new Vector3(mousePosition.x, mousePosition.y, transform.position.z);
        }

        public void OnStopDrag(Vector3 mousePosition)
        {
            // TODO: Snap to grid
            _sortingGroup.sortingLayerName = _originalSortingLayer;
        }
    }
}