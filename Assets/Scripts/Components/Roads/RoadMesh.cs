using Unity.Entities;

public struct RoadMesh : IComponentData
{
    public bool Dirty; // needs rebuild
}
