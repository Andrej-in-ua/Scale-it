using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class CardView : MonoBehaviour
{
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