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

        // Mesh + material
        Mesh mesh = CubeMeshGenerator.Create();
        Material material = Resources.Load<Material>("BuildingMaterial");

        var renderDesc = new RenderMeshDescription(
            ShadowCastingMode.Off,
            receiveShadows: false
        );

        EntityArchetype archetype = em.CreateArchetype(
            typeof(LocalTransform),
            typeof(WorldPosition),
            typeof(SpatialCell),
            typeof(BuildingData),
            typeof(PostTransformMatrix)
        );

        var renderMeshArray = new RenderMeshArray(
            new[] { material },
            new[] { mesh }
        );

        const float spacing = 10f; // meters between buildings (TEMP)

        for (int i = 0; i < 10000; i++)
        {
            Entity e = em.CreateEntity(archetype);

            uint seed = (uint)(i * 928371 + 12345);
            var rng = new Unity.Mathematics.Random(seed);

            byte floors = (byte)rng.NextInt(1, 20);
            float height = floors * BuildingConstants.FloorHeight;

            // Continuous world position
            float x = (i % 100) * spacing;
            float z = (i / 100) * spacing;
            float2 worldPos = new float2(x, z);

            // Authoritative world position
            em.SetComponentData(e, new WorldPosition
            {
                Value = worldPos
            });

            // Spatial acceleration cell
            em.SetComponentData(e, new SpatialCell
            {
                Value = (int2)math.floor(worldPos / SpatialConstants.CellSize)
            });

            // Transform (visual only)
            em.SetComponentData(e,
                LocalTransform.FromPositionRotationScale(
                    new float3(worldPos.x, height * 0.5f, worldPos.y),
                    quaternion.identity,
                    1f
                )
            );

            // Non-uniform scale for building height
            em.SetComponentData(e,
                new PostTransformMatrix
                {
                    Value = float4x4.Scale(new float3(1f, height, 1f))
                }
            );

            // Building simulation data
            em.SetComponentData(e, new BuildingData
            {
                Zone = (ZoningType)rng.NextInt(0, 4),
                Floors = floors,
                Height = height,
                Seed = seed,
                Occupancy = rng.NextFloat(0.3f, 1f)
            });

            // Rendering
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
