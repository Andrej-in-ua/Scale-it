using ECS.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;
using UnityEngine;

namespace ECS.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(AStarPathfinderSystem))]
    public partial struct PathVisualizerSystem : ISystem
    {
        private Entity _visualPrefab;

        public void OnCreate(ref SystemState state)
        {
            // Requires prefab with Mesh + Material
            state.RequireForUpdate<VisualPrefabTag>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            if (_visualPrefab == Entity.Null)
            {
                _visualPrefab = SystemAPI
                    .QueryBuilder()
                    .WithAll<VisualPrefabTag>()
                    .Build()
                    .GetSingletonEntity();

                Debug.Log($"visual prefab is found : {_visualPrefab != Entity.Null} ");
            }

            foreach (var (buffer, entity) in SystemAPI
                         .Query<DynamicBuffer<PathResult>>()
                         .WithEntityAccess()
                         .WithNone<PathVisualizedTag>())
            {
                if (buffer.Length == 0)
                {
                    continue;
                }

                Debug.Log("buffer = " + buffer.Length);

                for (int i = 0; i < buffer.Length; i++)
                {
                    var pos = buffer[i].Cell;
                    var visual = ecb.Instantiate(_visualPrefab);

                    ecb.SetComponent(visual, new LocalTransform
                    {
                        Position = new float3(pos.x + 0.5f, pos.y + 0.5f, 0), // center in cell
                        Rotation = quaternion.identity,
                        Scale = 0.4f
                    });

                    // Optional: color gradient along the path
                    float t = i / (float)buffer.Length;
                    var color = new float4(1 - t, t, 0.1f, 1f); // red to green
                    ecb.SetComponent(visual, new URPMaterialPropertyBaseColor { Value = color });
                }

                ecb.AddComponent<PathVisualizedTag>(entity);
            }

            ecb.Playback(state.EntityManager);
            // ecb.Dispose();
        }
    }

    public struct PathVisualizedTag : IComponentData
    {
    }

    public struct VisualPrefabTag : IComponentData
    {
    }
}