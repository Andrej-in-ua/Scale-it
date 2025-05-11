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

        private Entity _entity;
        private EntityManager _entityManager;

        private SortingGroup _sortingGroup;

        private string _originalSortingLayer;

        private void Awake()
        {
            _sortingGroup = GetComponent<SortingGroup>();
        }

        public void BakeEntity(Entity entity, EntityManager entityManager)
        {
            if (_entity != default)
                throw new System.Exception("Entity has already been set");

            _entity = entity;
            _entityManager = entityManager;
        }

        private void OnDestroy()
        {
            if (World.DefaultGameObjectInjectionWorld is { IsCreated: true } && _entityManager.Exists(_entity))
            {
                _entityManager.DestroyEntity(_entity);
            }
        }

        // private void AssertBaked()
        // {
        //     if (_entity == default)
        //         throw new System.Exception("Entity is not baked");
        //
        //     if (_entityManager.Exists(_entity))
        //         throw new System.Exception("Entity has already been destroyed");
        // }

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