using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.Collections;
using Throughput.Plots;

public partial struct PlotOutlineRenderSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {   
        UnityEngine.Debug.Log("PlotOutlineRenderSystem running");
        var em = state.EntityManager;

        var plots = SystemAPI
            .QueryBuilder()
            .WithAll<PlotData>()
            .WithNone<PlotRenderTag>()
            .Build()
            .ToEntityArray(Allocator.Temp);

        foreach (var plot in plots)
        {
            var verts = em.GetBuffer<PlotVertex>(plot);
            if (verts.Length < 2)
                continue;

            Mesh mesh = BuildOutlineMesh(verts);

            AttachRender(em, plot, mesh);

            em.AddComponent<PlotRenderTag>(plot);
            UnityEngine.Debug.Log("Rendering plot");
        }

        plots.Dispose();

    }

    // -----------------------------

    static Mesh BuildOutlineMesh(DynamicBuffer<PlotVertex> verts)
    {
        var mesh = new Mesh();

        int lineCount = verts.Length;
        var vertices = new Vector3[lineCount * 2];
        var indices = new int[lineCount * 2];

        int v = 0;
        int i = 0;

        for (int p = 0; p < verts.Length; p++)
        {
            float2 a = verts[p].Value;
            float2 b = verts[(p + 1) % verts.Length].Value;

            vertices[v]     = new Vector3(a.x, 0.05f, a.y);
            vertices[v + 1] = new Vector3(b.x, 0.05f, b.y);

            indices[i]     = v;
            indices[i + 1] = v + 1;

            v += 2;
            i += 2;
        }

        mesh.vertices = vertices;
        mesh.SetIndices(indices, MeshTopology.Lines, 0);
        mesh.RecalculateBounds();
        return mesh;
    }

    // -----------------------------

    static void AttachRender(EntityManager em, Entity entity, Mesh mesh)
    {
        Material mat = Resources.Load<Material>("PlotOutlineMaterial");

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
