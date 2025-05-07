using UnityEngine;
using UnityEngine.Tilemaps;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

// TODO: THIS IS EXAMPLE CODE, REMOVE IT LATER!
public class CardEntityFactory : MonoBehaviour
{
    public GameObject cardPrefab;
    public Grid grid;

    private void Start()
    {
        int2 gridPosition = new int2(3, 5);
        Vector3 worldPosition = grid.CellToWorld(new Vector3Int(3, 5, 0)) + new Vector3(0.5f, 0.5f, 0);
        CreateCard(gridPosition, worldPosition);
    }

    public Entity CreateCard(int2 gridPosition, Vector3 worldPosition)
    {
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        var entity = entityManager.CreateEntity();

        entityManager.AddComponentData(entity, new CardTile
        {
            GridPosition = gridPosition
        });

        entityManager.AddComponentData(entity, new LocalTransform
        {
            Position = new float3(worldPosition.x, worldPosition.y, 0),
            Rotation = quaternion.identity,
            Scale = 1f
        });

        if (cardPrefab != null)
        {
            var view = Instantiate(cardPrefab, worldPosition, Quaternion.identity);
            var cardView = view.GetComponent<CardView>();
            if (cardView != null)
            {
                cardView.entity = entity;
                cardView.entityManager = entityManager;
            }

            entityManager.AddComponentObject(entity, view);
        }

        return entity;
    }
}

public struct CardTile : IComponentData
{
    public int2 GridPosition;
}

public struct Socket : IBufferElementData
{
    public enum Type { Input, Output }
    public Type SocketType;
    public int2 LocalOffset;
    public Entity ConnectedTo;
}