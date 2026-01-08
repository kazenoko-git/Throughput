using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct SpawnerSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        var em = state.EntityManager;

        Entity prefab = em.CreateEntity(typeof(LocalTransform));

        for (int i = 0; i < 10000; i++)
        {
            Entity e = em.Instantiate(prefab);

            em.SetComponentData(e, new LocalTransform
            {
                Position = new float3(i % 100, 0, i / 100),
                Rotation = quaternion.identity,
                Scale = 1f
            });
        }

        // Run once only
        state.Enabled = false;
    }
}
