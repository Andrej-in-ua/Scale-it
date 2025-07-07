using Unity.Entities;
using ECS.Systems;

namespace View.GameTable
{
    public class QuadVisualBaker : Baker<QuadVisualAuthoring>
    {
        public override void Bake(QuadVisualAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Renderable);
            AddComponent<VisualPrefabTag>(entity);
        }
    }
}