using ECS.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace ECS.Systems.Jobs
{
    [BurstCompile]
    public struct AStarJob : IJob
    {
        public int2 Start;
        public int2 End;
        [ReadOnly] public NativeParallelHashMap<int2, half>.ReadOnly GridCosts;
        public EntityCommandBuffer.ParallelWriter Ecb;
        public Entity RequestEntity;
        public int JobIndex;

        public void Execute()
        {
            var path = new NativeList<int2>(Allocator.Temp);

            for (int i = path.Length - 1; i >= 0; i--)
            {
                Ecb.AppendToBuffer(JobIndex, RequestEntity, new PathResult { Cell = path[i] });
            }

            path.Dispose();
        }
    }
}