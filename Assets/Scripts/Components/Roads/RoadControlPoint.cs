using Unity.Entities;
using Unity.Mathematics;

public struct RoadControlPoint : IBufferElementData
{
    public float2 Value; // world-space XZ
}
