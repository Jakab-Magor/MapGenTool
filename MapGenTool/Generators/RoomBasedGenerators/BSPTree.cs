using MapGenTool.Generic;
using System.ComponentModel.DataAnnotations;

namespace MapGenTool.Generators.RoomBasedGenerators;

public class BSPTree : ILevelGenerator
{
    [Range(0, int.MaxValue, MinimumIsExclusive = true)]
    public int PartitionCount { get; set; }

    public TileTypes[,] Generate(int width, int height, int seed)
    {
        TileTypes[,] grid = new TileTypes[width, height];
        Random rng = new(seed);

        BinaryTreeNode<Room> root = new(null, null);
        for (int i = 0; i < PartitionCount; i++)
        {

        }
    }

    private Room GetRandomRoom(ref Random rng, IntVector2 maxRoomSize, int gridWidth, int gridHeight)
    {
        IntVector2 size = new(
            x: rng.Next(2, maxRoomSize.x),
            y: rng.Next(2, maxRoomSize.y));
        IntVector2 pos = new(
            x: rng.Next(0, gridWidth - size.x),
            y: rng.Next(0, gridHeight - size.y));

        Room r = new(size, pos);
        return r;
    }
}
