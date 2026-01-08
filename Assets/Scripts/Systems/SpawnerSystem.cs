using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

public partial struct SpawnerSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        var em = state.EntityManager;

        // Create mesh + material
        Mesh mesh = CubeMeshGenerator.Create();
        Material material = Resources.Load<Material>("BuildingMaterial");

        // Rendering description (NEW API)
        var renderDesc = new RenderMeshDescription(
            ShadowCastingMode.Off,
            receiveShadows: false
        );

        // Base entity
        EntityArchetype archetype = em.CreateArchetype(
            typeof(LocalTransform)
        );

        // Register mesh & material with Entities Graphics
        var renderMeshArray = new RenderMeshArray(
            new[] { material },
            new[] { mesh }
        );

        for (int i = 0; i < 10000; i++)
        {
            Entity e = em.CreateEntity(archetype);

            em.SetComponentData(e,
                LocalTransform.FromPosition(new float3(i % 100, 0, i / 100))
            );

            // Add rendering components
            RenderMeshUtility.AddComponents(
                e,
                em,
                renderDesc,
                renderMeshArray,
                MaterialMeshInfo.FromRenderMeshArrayIndices(0, 0)
            );
        }

        state.Enabled = false;
    }
}
