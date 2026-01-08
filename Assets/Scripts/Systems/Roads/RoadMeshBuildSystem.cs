using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.Collections;

public partial struct RoadMeshBuildSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var em = state.EntityManager;

        // 1️⃣ Collect roads that need rebuilding
        var roads = SystemAPI
            .QueryBuilder()
            .WithAll<RoadMesh, RoadRenderTag, RoadSegment, RoadData>()
            .Build()
            .ToEntityArray(Allocator.Temp);

        foreach (var entity in roads)
        {
            var roadMesh = em.GetComponentData<RoadMesh>(entity);
            if (!roadMesh.Dirty)
                continue;

            var segments = em.GetBuffer<RoadSegment>(entity);
            if (segments.Length == 0)
                continue;

            var road = em.GetComponentData<RoadData>(entity);

            float halfWidth =
                (road.LanesLeft + road.LanesRight) *
                road.LaneWidth * 0.5f;

            // 2️⃣ Build mesh (LOCAL ONLY, not stored on entity)
            Mesh mesh = BuildRoadMesh(segments, halfWidth);

            // 3️⃣ Attach rendering IMMEDIATELY
            AttachRender(em, entity, mesh);

            // 4️⃣ Mark clean
            roadMesh.Dirty = false;
            em.SetComponentData(entity, roadMesh);
        }

        roads.Dispose();
    }

    // --------------------------------------

    static Mesh BuildRoadMesh(
        DynamicBuffer<RoadSegment> segments,
        float halfWidth)
    {
        var mesh = new Mesh();

        int quadCount = segments.Length;
        var vertices = new Vector3[quadCount * 4];
        var triangles = new int[quadCount * 6];

        int v = 0;
        int t = 0;

        for (int i = 0; i < segments.Length; i++)
        {
            float2 a = segments[i].Start;
            float2 b = segments[i].End;

            float2 dir = math.normalize(b - a);
            float2 normal = new float2(-dir.y, dir.x);

            float2 leftA  = a + normal * halfWidth;
            float2 rightA = a - normal * halfWidth;
            float2 leftB  = b + normal * halfWidth;
            float2 rightB = b - normal * halfWidth;

            vertices[v + 0] = new Vector3(leftA.x, 0f, leftA.y);
            vertices[v + 1] = new Vector3(rightA.x, 0f, rightA.y);
            vertices[v + 2] = new Vector3(leftB.x, 0f, leftB.y);
            vertices[v + 3] = new Vector3(rightB.x, 0f, rightB.y);

            triangles[t + 0] = v + 0;
            triangles[t + 1] = v + 2;
            triangles[t + 2] = v + 1;

            triangles[t + 3] = v + 1;
            triangles[t + 4] = v + 2;
            triangles[t + 5] = v + 3;

            v += 4;
            t += 6;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }

    // --------------------------------------

    static void AttachRender(
        EntityManager em,
        Entity entity,
        Mesh mesh)
    {
        Material mat = Resources.Load<Material>("RoadMaterial");

        var renderDesc = new RenderMeshDescription(
            ShadowCastingMode.Off,
            receiveShadows: false
        );

        var rma = new RenderMeshArray(
            new[] { mat },
            new[] { mesh }
        );

        if (!em.HasComponent<LocalTransform>(entity))
        {
            em.AddComponentData(
                entity,
                LocalTransform.FromPosition(float3.zero)
            );
        }

        RenderMeshUtility.AddComponents(
            entity,
            em,
            renderDesc,
            rma,
            MaterialMeshInfo.FromRenderMeshArrayIndices(0, 0)
        );
    }
}
