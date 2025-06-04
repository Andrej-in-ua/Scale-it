using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;

namespace ECS.Components
{
    public struct PathRequest : IComponentData
    {
        public int2 Start;
        public int2 End;
    }
}