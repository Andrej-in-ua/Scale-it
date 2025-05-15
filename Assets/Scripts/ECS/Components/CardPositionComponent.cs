using Unity.Entities;
using Unity.Mathematics;

namespace ECS.Components
{
    public struct CardPositionComponent : IComponentData
    {
        public int2 GridPosition;
    }
}