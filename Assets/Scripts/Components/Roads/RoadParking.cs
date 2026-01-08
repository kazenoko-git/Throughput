using Unity.Entities;

public enum ParkingType : byte
{
    Straight,
    Angled
}

public struct RoadParking : IComponentData
{
    public RoadSide Sides;
    public ParkingType Type;
}
