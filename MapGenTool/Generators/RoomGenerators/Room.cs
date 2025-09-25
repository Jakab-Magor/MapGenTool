using MapGenTool.Generic;

namespace MapGenTool.Generators.RoomGenerators;

internal record struct Room(IntVector2 Position, IntVector2 Size)
{
    public override string? ToString()
    {
        return $"pos: {Position} [size: {Size}]";
    }
}
