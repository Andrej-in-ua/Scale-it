using Unity.Entities;
using Unity.Mathematics;

namespace ECS.Components
{
    public struct PathResult : IBufferElementData
    {
        public int2 Cell;
    }
}