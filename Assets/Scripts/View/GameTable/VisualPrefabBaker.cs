using ECS.Systems;
using Unity.Entities;
using UnityEngine;

namespace View.GameTable
{
    public class VisualPrefabBaker : MonoBehaviour
    {
        public class Baker : Baker<VisualPrefabBaker>
        {
            public override void Bake(VisualPrefabBaker tag)
            {
                var entity = GetEntity(TransformUsageFlags.Renderable);
                AddComponent<VisualPrefabTag>(entity);
            }
        }
    }
}