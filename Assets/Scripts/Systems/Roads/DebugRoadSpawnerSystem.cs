using Unity.Entities;
using Unity.Mathematics;

public partial struct DebugRoadSpawnerSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        var em = state.EntityManager;

        // Create a road entity
        Entity road = em.CreateEntity(
            typeof(RoadData),
            typeof(RoadRenderTag),
            typeof(RoadMesh)
        );

        // Basic road configuration
        em.SetComponentData(road, new RoadData
        {
            Type = RoadType.Street,
            LanesLeft = 1,
            LanesRight = 1,
            LaneWidth = 3.5f
        });

        // Mark mesh dirty so it gets built
        em.SetComponentData(road, new RoadMesh
        {
            Dirty = true
        });

        // Add control points
        var points = em.AddBuffer<RoadControlPoint>(road);

        // 🔹 STRAIGHT ROAD (uncomment this)
        points.Add(new RoadControlPoint { Value = new float2(0, 0) });
        points.Add(new RoadControlPoint { Value = new float2(80, 0) });

        // 🔹 CURVED ROAD (comment straight, uncomment this)
        /*
        points.Add(new RoadControlPoint { Value = new float2(0, 0) });
        points.Add(new RoadControlPoint { Value = new float2(40, 25) });
        points.Add(new RoadControlPoint { Value = new float2(80, 0) });
        */
        UnityEngine.Debug.Log("Road spawned");

        state.Enabled = false;
    }
}
