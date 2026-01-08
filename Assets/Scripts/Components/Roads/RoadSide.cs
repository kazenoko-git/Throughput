using System;

[Flags]
public enum RoadSide : byte
{
    None  = 0,
    Left  = 1,
    Right = 2,
    Both  = Left | Right
}
