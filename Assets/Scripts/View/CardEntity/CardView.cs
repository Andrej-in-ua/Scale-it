using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.U2D;

namespace View.CardEntity
{
    public class CardView : MonoBehaviour
    {
        // [SerializeField] public SpriteAtlas cardAtlas;
        
        // [Header("References")]
        // public GameObject background;

        public Entity entity;
        public EntityManager entityManager;

        void Update()
        {
            if (entityManager.Exists(entity))
            {
                var pos = entityManager.GetComponentData<LocalTransform>(entity).Position;
                transform.position = new Vector3(pos.x, pos.y, 0);
               // Debug.Log(transform.position);
            }
        }
    }
}
