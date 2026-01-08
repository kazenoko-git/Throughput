using Unity.Entities;
using Unity.Transforms;

public partial struct MoveSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        float dt = SystemAPI.Time.DeltaTime;

        foreach (var transform in SystemAPI.Query<RefRW<LocalTransform>>())
        {
            transform.ValueRW.Position.y += dt;
        }
    }
}
