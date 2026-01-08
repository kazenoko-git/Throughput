using Unity.Entities;
using Unity.Mathematics;

public struct RoadSegment : IBufferElementData
{
    public float2 Start;
    public float2 End;
    public float Length;
}
