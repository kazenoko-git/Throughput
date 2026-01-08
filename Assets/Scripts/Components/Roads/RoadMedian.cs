using Unity.Entities;

public enum MedianType : byte
{
    None,
    Paved,
    Grass
}

public struct RoadMedian : IComponentData
{
    public MedianType Type;
    public float Width;
}
