using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

public partial struct RoadMeshBuildSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (meshFlag, entity) in
            SystemAPI.Query<RefRW<RoadMesh>>()
                     .WithEntityAccess())
        {
            if (!meshFlag.ValueRO.Dirty)
                continue;

            // 1. Read RoadData
            // 2. Read RoadSegment buffer
            // 3. Compute total width
            // 4. Extrude vertices along segments
            // 5. Build Mesh
            // 6. Attach via Entities Graphics
            // 7. Mark Dirty = false
        }
    }
}
