using ECS.Components;
using ECS.Systems.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace ECS.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct AStarPathfinderSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var gridMap = GridService.Map;

            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

            var job = new AStarJob
            {
                GridCosts = gridMap,
                Ecb = ecb
            };
            state.Dependency = job.ScheduleParallel(state.Dependency);
        }

        [BurstCompile]
        public partial struct AStarJob : IJobEntity
        {
            [ReadOnly] public NativeParallelHashMap<int2, half>.ReadOnly GridCosts;
            public EntityCommandBuffer.ParallelWriter Ecb;

            public void Execute(Entity entity, [EntityIndexInQuery] int sortKey, in PathRequest request)
            {
                var path = new NativeList<int2>(Allocator.Temp);
                var openSet = new NativeList<int2>(Allocator.Temp);
                var cameFrom = new NativeParallelHashMap<int2, int2>(128, Allocator.Temp);
                var gScore = new NativeParallelHashMap<int2, float>(128, Allocator.Temp);

                openSet.Add(request.Start);
                gScore[request.Start] = 0f;

                while (openSet.Length > 0)
                {
                    var current = openSet[0];
                    openSet.RemoveAtSwapBack(0);

                    if (current.Equals(request.End))
                    {
                        // Reconstruct path
                        path.Add(current);
                        while (cameFrom.TryGetValue(current, out var prev))
                        {
                            current = prev;
                            path.Add(current);
                        }

                        break;
                    }

                    foreach (var neighbor in GetNeighbors(current))
                    {
                        if (!GridCosts.TryGetValue(neighbor, out var cost)) continue;

                        var tentativeGScore = gScore[current] + cost;
                        if (!gScore.TryGetValue(neighbor, out var existingScore) || tentativeGScore < existingScore)
                        {
                            cameFrom[neighbor] = current;
                            gScore[neighbor] = tentativeGScore;
                            openSet.Add(neighbor);
                        }
                    }
                }

                for (int i = path.Length - 1; i >= 0; i--)
                {
                    Ecb.AppendToBuffer(sortKey, entity, new PathResult { Cell = path[i] });
                }

                path.Dispose();
                openSet.Dispose();
                cameFrom.Dispose();
                gScore.Dispose();

                Ecb.RemoveComponent<PathRequest>(sortKey, entity);
            }

            private static NativeList<int2> GetNeighbors(int2 cell)
            {
                var list = new NativeList<int2>(4, Allocator.Temp)
                {
                    cell + new int2(1, 0),
                    cell + new int2(-1, 0),
                    cell + new int2(0, 1),
                    cell + new int2(0, -1)
                };
                return list;
            }
        }
    }
}