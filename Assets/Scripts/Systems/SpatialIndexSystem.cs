using Unity.Entities;
using Unity.Mathematics;

public partial struct SpatialIndexSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (pos, cell) in
            SystemAPI.Query<RefRO<WorldPosition>, RefRW<SpatialCell>>())
        {
            int2 newCell = (int2)math.floor(
                pos.ValueRO.Value / SpatialConstants.CellSize
            );

            cell.ValueRW.Value = newCell;
        }
    }
}
