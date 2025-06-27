using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using ECS.Components;

namespace ECS.Systems
{
    [BurstCompile]
    [UpdateAfter(typeof(GridSystem))]
    public partial struct AStarPathfinderSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

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

            private const float MoveCost = 10f;
            private const float TurnPenalty = 50f;

            public void Execute(Entity entity, [EntityIndexInQuery] int sortKey, in PathRequest request)
            {
                int startCost = GridCosts.TryGetValue(request.Start, out var sCost) ? (int)sCost : 0;
                int endCost = GridCosts.TryGetValue(request.End, out var eCost) ? (int)eCost : 0;

                if (startCost < 0 || endCost < 0)
                {
                    UnityEngine.Debug.LogWarning($"[AStar] Start or End not walkable (cost < 0): {request.Start} -> {request.End}");
                    Ecb.RemoveComponent<PathRequest>(sortKey, entity);
                    return;
                }

                var openSet = new NativeMinHeap<PathNode>(128, Allocator.Temp);
                var gScore = new NativeParallelHashMap<PathNode, float>(128, Allocator.Temp);
                var cameFrom = new NativeParallelHashMap<PathNode, PathNode>(128, Allocator.Temp);
                var path = new NativeList<int2>(Allocator.Temp);

                FixedList64Bytes<int2> directions = new FixedList64Bytes<int2>
                {
                    new int2(1, 0),
                    new int2(-1, 0),
                    new int2(0, 1),
                    new int2(0, -1)
                };

                var start = new PathNode { Pos = request.Start, Dir = int2.zero };
                gScore[start] = 0;
                openSet.Enqueue(start, Heuristic(request.Start, request.End));

                var iterations = 0;

                while (!openSet.IsEmpty)
                {
                    iterations++;
                    var current = openSet.Dequeue();

                    if (current.Pos.Equals(request.End))
                    {
                        var node = current;
                        while (cameFrom.TryGetValue(node, out var prev))
                        {
                            path.Add(node.Pos);
                            node = prev;
                        }
                        path.Add(request.Start);
                        break;
                    }

                    foreach (var dir in directions)
                    {
                        var neighborPos = current.Pos + dir;

                        float cellCost = GridCosts.TryGetValue(neighborPos, out var rawCost)
                            ? rawCost
                            : 0; // default walkable

                        if (cellCost < 0)
                            continue; // not walkable

                        var neighbor = new PathNode { Pos = neighborPos, Dir = dir };

                        float turnCost = (current.Dir.Equals(int2.zero) || current.Dir.Equals(dir)) ? 0 : TurnPenalty;
                        float tentativeG = gScore[current] + MoveCost + turnCost + cellCost;

                        if (!gScore.TryGetValue(neighbor, out var existingG) || tentativeG < existingG)
                        {
                            cameFrom[neighbor] = current;
                            gScore[neighbor] = tentativeG;
                            float f = tentativeG + Heuristic(neighborPos, request.End);
                            openSet.Enqueue(neighbor, f);
                        }
                    }
                }

                if (path.Length == 0)
                {
                    UnityEngine.Debug.LogWarning($"[AStar] No path found from {request.Start} to {request.End}");
                }
                else
                {
                    //UnityEngine.Debug.Log($"[AStar] Iterations {iterations}");

                    for (int i = path.Length - 1; i >= 0; i--)
                    {
                        //UnityEngine.Debug.Log($"[AStar] Found path {path[i]}");
                        Ecb.AppendToBuffer(sortKey, entity, new PathResult { Cell = path[i] });
                    }
                }

                path.Dispose();
                openSet.Dispose();
                gScore.Dispose();
                cameFrom.Dispose();

                Ecb.RemoveComponent<PathRequest>(sortKey, entity);
            }

            private static float Heuristic(int2 a, int2 b)
            {
                return math.abs(a.x - b.x) + math.abs(a.y - b.y); // Manhattan distance
            }

            private struct PathNode : IEquatable<PathNode>
            {
                public int2 Pos;
                public int2 Dir;

                public bool Equals(PathNode other) => Pos.Equals(other.Pos) && Dir.Equals(other.Dir);
                public override int GetHashCode() => (int)(math.hash(Pos) ^ math.hash(Dir));
            }
        }
    }
}
