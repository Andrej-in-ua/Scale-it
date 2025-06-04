using ECS.Components;
using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct PathResultLoggerSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (buffer, entity) in SystemAPI.Query<DynamicBuffer<PathResult>>().WithEntityAccess())
        {
            foreach (var cell in buffer)
            {
                Debug.Log($"  Cell: {cell.Cell}");
            }
        }
    }
}