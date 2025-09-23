using MapGenTool.Generic;

namespace MapGenTool.Generators.RoomBasedGenerators;

internal record struct Room(IntVector2 Position, IntVector2 Size)
{
    public override string? ToString()
    {
        return $"pos: {Position} [size: {Size}]";
    }
}
