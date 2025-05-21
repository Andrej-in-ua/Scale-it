using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace ECS.Systems
{
     /// <summary>
    /// Public API for reader systems.
    /// </summary>
    public static class GridService
    {
        public static NativeParallelHashMap<int2, half>.ReadOnly Map;
    }

    /// <summary>
    /// One queued update (set/remove cell cost).
    /// </summary>
    public struct GridUpdate : IBufferElementData
    {
        public int2 Cell;
        public half Cost;
    }

    public struct GridUpdateQueueTag : IComponentData { }

    /// <summary>
    /// Infinite cost map with main‑thread batch updates (hundreds per frame are negligible).
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct GridSystem : ISystem
    {
        private NativeParallelHashMap<int2, half> _costMap;
        private Entity                             _queueSingleton;

        #region lifecycle
        public void OnCreate(ref SystemState state)
        {
            _costMap = new NativeParallelHashMap<int2, half>(1024, Allocator.Persistent);

            _queueSingleton = state.EntityManager.CreateEntity(
                typeof(GridUpdateQueueTag),
                typeof(GridUpdate));
        }

        public void OnDestroy(ref SystemState state)
        {
            if (_costMap.IsCreated)
                _costMap.Dispose();
        }
        #endregion

        #region update
        public void OnUpdate(ref SystemState state)
        {
            // Ensure previously scheduled jobs that might read the map are finished
            state.Dependency.Complete();

            var updates = state.EntityManager.GetBuffer<GridUpdate>(_queueSingleton);

            if (updates.Length > 0)
            {
                for (int i = 0; i < updates.Length; i++)
                {
                    var u = updates[i];
                    if (u.Cost == (half)0f)
                        _costMap.Remove(u.Cell);
                    else
                        _costMap[u.Cell] = u.Cost;
                }
                updates.Clear();
            }

            // Expose read‑only map for other systems (no pending writer jobs)
            GridService.Map = _costMap.AsReadOnly();

            // No jobs scheduled → no dependency to propagate
            state.Dependency = default;
        }
        #endregion
    }
}