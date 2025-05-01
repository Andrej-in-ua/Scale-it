using UnityEngine;
using UnityEngine.Tilemaps;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class TilemapBaker : MonoBehaviour
{
    public Tilemap tilemap;
    public TileBase cardTile;
    public GameObject cardPrefab; // Optional view prefab

    private void Start()
    {
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        foreach (var pos in tilemap.cellBounds.allPositionsWithin)
        {
            if (!tilemap.HasTile(pos)) continue;

            var tile = tilemap.GetTile(pos);
            if (tile == cardTile)
            {
                var worldPos = tilemap.CellToWorld(pos);

                var entity = entityManager.CreateEntity();

                // ECS component
                entityManager.AddComponentData(entity, new CardTile
                {
                    CellPosition = new int2(pos.x, pos.y)
                });

                // Basic transform
                entityManager.AddComponentData(entity, new LocalTransform
                {
                    Position = new float3(worldPos.x + 0.5f, worldPos.y + 0.5f, 0),
                    Rotation = quaternion.identity,
                    Scale = 1f
                });

                // Optional: attach visual view
                if (cardPrefab != null)
                {
                    var go = Instantiate(cardPrefab, worldPos + new Vector3(0.5f, 0.5f, 0), Quaternion.identity);
                    entityManager.AddComponentObject(entity, go);
                }
            }
        }
    }
}

public struct CardTile : IComponentData
{
    public int2 CellPosition;
}