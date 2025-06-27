using ECS.Components;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct PathResultLoggerSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (buffer, entity) in SystemAPI
                     .Query<DynamicBuffer<PathResult>>()
                     .WithEntityAccess()
                     .WithNone<PathLoggedTag>()) // Only log if not tagged
        {
            if (state.EntityManager.HasComponent<PathRequest>(entity))
                continue;

            if (buffer.Length == 0)
            {
                //Debug.LogWarning($"[PathLogger] Empty path buffer on entity {entity.Index}!");
                continue;
            }

            Debug.Log($"[PathLogger] Entity {entity.Index} - path length: {buffer.Length}");
            for (int i = 0; i < buffer.Length; i++)
            {
                Debug.Log($"  [{i}] Cell: {buffer[i].Cell}");
            }
            
            ecb.AddComponent<PathLoggedTag>(entity);
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}

public struct PathLoggedTag : IComponentData {}