using Unity.Entities;

public struct RoadData : IComponentData
{
    public RoadType Type;

    public byte LanesLeft;
    public byte LanesRight;

    public float LaneWidth; // meters
}
