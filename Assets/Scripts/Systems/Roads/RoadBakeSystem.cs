using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;

public partial struct RoadBakeSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var em = state.EntityManager;

        // -------------------------------
        // PHASE 1: Ensure RoadSegment buffer exists
        // -------------------------------
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var entity in
            SystemAPI.QueryBuilder()
                     .WithAll<RoadControlPoint>()
                     .WithNone<RoadSegment>()
                     .Build()
                     .ToEntityArray(Allocator.Temp))
        {
            ecb.AddBuffer<RoadSegment>(entity);
        }

        ecb.Playback(em);
        ecb.Dispose();

        // -------------------------------
        // PHASE 2: Populate RoadSegment buffer
        // -------------------------------
        foreach (var (points, segments) in
            SystemAPI.Query<
                DynamicBuffer<RoadControlPoint>,
                DynamicBuffer<RoadSegment>>())
        {
            segments.Clear();

            for (int i = 0; i < points.Length - 1; i++)
            {
                float2 a = points[i].Value;
                float2 b = points[i + 1].Value;

                segments.Add(new RoadSegment
                {
                    Start = a,
                    End = b,
                    Length = math.distance(a, b)
                });
            }
        }
    }
}
