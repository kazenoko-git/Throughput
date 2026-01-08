using Unity.Entities;

public struct RoadSidewalk : IComponentData
{
    public RoadSide Sides;

    public float WalkwayWidth;
    public float GrassWidth;
}
