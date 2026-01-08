using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using Throughput.Plots;

public partial struct PlotGenerationSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var em = state.EntityManager;
        //if (SystemAPI.HasSingleton<PlotsGeneratedTag>())
        //return;
        UnityEngine.Debug.Log(
            $"PlotGenerationSystem running. Roads found: " +
            SystemAPI.QueryBuilder()
                .WithAll<RoadData, RoadSimTag>()
                .Build()
                .CalculateEntityCount()
        );

        var roads = SystemAPI
            .QueryBuilder()
            .WithAll<RoadData, RoadSimTag>()
            .Build()
            .ToEntityArray(Allocator.Temp);

        foreach (var road in roads)
        {   
            if (!em.HasBuffer<RoadSegment>(road))
                continue;
            var segments = em.GetBuffer<RoadSegment>(road);
            var roadData = em.GetComponentData<RoadData>(road);
            UnityEngine.Debug.Log($"Segments count: {segments.Length}");
            float roadHalfWidth =
                (roadData.LanesLeft + roadData.LanesRight) *
                roadData.LaneWidth * 0.5f;
            UnityEngine.Debug.Log("Generating plots for road");

            foreach (var seg in segments)
            {
                CreatePlot(em, seg, roadHalfWidth, leftSide: true);
                CreatePlot(em, seg, roadHalfWidth, leftSide: false);
            }
        }

        roads.Dispose();
        //em.CreateEntity(typeof(PlotsGeneratedTag));


    }

    static void CreatePlot(
        EntityManager em,
        RoadSegment seg,
        float roadHalfWidth,
        bool leftSide)
    {
        float2 dir = math.normalize(seg.End - seg.Start);
        float2 normal = new float2(-dir.y, dir.x);

        if (!leftSide)
            normal = -normal;

        float plotDepth = 30f; // meters (TEMP)

        float2 a = seg.Start + normal * roadHalfWidth;
        float2 b = seg.End   + normal * roadHalfWidth;
        float2 c = b + normal * plotDepth;
        float2 d = a + normal * plotDepth;

        Entity plot = em.CreateEntity(typeof(PlotData));
        var verts = em.AddBuffer<PlotVertex>(plot);

        verts.Add(new PlotVertex { Value = a });
        verts.Add(new PlotVertex { Value = b });
        verts.Add(new PlotVertex { Value = c });
        verts.Add(new PlotVertex { Value = d });

        em.SetComponentData(plot, new PlotData
        {
            Zone = ZoningType.Residential, // TEMP
            Area = plotDepth * math.length(b - a)
        });
        UnityEngine.Debug.Log("Plot created");
    }
}
