using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace ECS.Components
{
    public struct CardPositionComponent : IComponentData
    {
        public bool IsOnGrid;
        public int2 GridPosition;

        public static CardPositionComponent Default => new CardPositionComponent
        {
            IsOnGrid = false,
            GridPosition = new int2(int.MinValue, int.MinValue)
        };

        public static implicit operator Vector3(CardPositionComponent component)
        {
            return new Vector3(component.GridPosition.x, component.GridPosition.y, 0);
        }
    }
}
