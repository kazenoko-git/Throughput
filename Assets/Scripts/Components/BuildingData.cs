using Unity.Entities;

public struct BuildingData : IComponentData
{
    public ZoningType Zone;

    public byte Floors;        // 1–255 floors
    public float Height;       // cached for rendering
    public uint Seed;          // deterministic variation

    public float Occupancy;    // 0–1 (used later for economy)
}
