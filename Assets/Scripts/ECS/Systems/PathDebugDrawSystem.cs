using ECS.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace ECS.Systems
{
    [WorldSystemFilter(WorldSystemFilterFlags.Editor)]
    public partial struct PathDebugDrawSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            // foreach (var buffer in SystemAPI.Query<DynamicBuffer<PathResult>>())
            // {
            //     for (int i = 0; i < buffer.Length - 1; i++)
            //     {
            //         var from = new float3(buffer[i].Cell.x + 0.5f, buffer[i].Cell.y + 0.5f, 0);
            //         var to = new float3(buffer[i + 1].Cell.x + 0.5f, buffer[i + 1].Cell.y + 0.5f, 0);
            //         UnityEngine.Debug.DrawLine(from, to, UnityEngine.Color.cyan, 10f, false);
            //     }
            // }
        }
    }
}