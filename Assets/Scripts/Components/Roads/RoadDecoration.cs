using Unity.Entities;

[System.Flags]
public enum RoadTreePlacement : byte
{
    None   = 0,
    Left   = 1,
    Right  = 2,
    Median = 4
}

public struct RoadDecoration : IComponentData
{
    public RoadTreePlacement Trees;
    public bool Lighting;
}
