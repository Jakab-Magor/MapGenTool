using MapGenTool.Generic;
using System.ComponentModel.DataAnnotations;

namespace MapGenTool.Generators.RoomBasedGenerators;

public class BSPTree(int partitionCount) : ILevelGenerator
{
    [Range(0, int.MaxValue, MinimumIsExclusive = true)]
    public int HalfPartitionCount { get; set; } = partitionCount;

    public TileTypes[,] Generate(int width, int height, int seed)
    {
        TileTypes[,] grid = new TileTypes[width, height];
        Random rng = new(seed);

        Room canvas = new Room(new IntVector2(0, 0), new IntVector2(width, height));
        BSPNode root = new(null, canvas);
        root.Split(HalfPartitionCount, true);

        var leaves = root.GetLeaves();
        for (int i = 0; i < leaves.Length; i++)
        {
            var leaf = leaves[i];

            leaf.InnerRoom = GetRandomRoom(ref rng, leaf.Bounds);
            DrawRoom(ref grid, leaf.InnerRoom);

            if (i != 0)
                DrawCorridors(ref grid, leaves[i - 1].InnerRoom, leaf.InnerRoom);
        }

        return grid;
    }

    private void DrawRoom(ref TileTypes[,] grid, Room room)
    {
        IntVector2 pos = room.Position;
        IntVector2 otherCorner = pos + room.Size;

        for (int y = pos.y; y < otherCorner.y; y++)
            for (int x = pos.x; x < otherCorner.x; x++)
                grid[x, y] = TileTypes.Space;
    }

    private void DrawCorridors(ref TileTypes[,] grid, Room a, Room b)
    {
        IntVector2 aCenter = a.Position + (a.Size / 2);
        IntVector2 bCenter = b.Position + (b.Size / 2);

        int xDiff = Math.Abs(bCenter.x - aCenter.x);
        int yDiff = Math.Abs(bCenter.y - aCenter.y);

        if (xDiff > yDiff) // connect horrizontally first
        {
            int xMin = Math.Min(aCenter.x, bCenter.x);
            int xMax = Math.Max(aCenter.x, bCenter.x);
            for (int x = xMin; x <= xMax; x++)
                grid[x, aCenter.y] = TileTypes.Space;

            int yMin = Math.Min(aCenter.y, bCenter.y);
            int yMax = Math.Max(aCenter.y, bCenter.y);
            for (int y = yMin; y <= yMax; y++)
                grid[bCenter.x, y] = TileTypes.Space;
        }
        else // connect vertically first
        {
            int yMin = Math.Min(aCenter.y, bCenter.y);
            int yMax = Math.Max(aCenter.y, bCenter.y);
            for (int y = yMin; y <= yMax; y++)
                grid[aCenter.x, y] = TileTypes.Space;

            int xMin = Math.Min(aCenter.x, bCenter.x);
            int xMax = Math.Max(aCenter.x, bCenter.x);
            for (int x = xMin; x <= xMax; x++)
                grid[x, bCenter.y] = TileTypes.Space;
        }
    }

    private Room GetRandomRoom(ref Random rng, Room bounds)
    {
        IntVector2 boundsPos = bounds.Position;
        IntVector2 boundsSize = bounds.Size - IntVector2.One;

        IntVector2 size = new(
            x: rng.Next(2, boundsSize.x - 1),
            y: rng.Next(2, boundsSize.y - 1));
        IntVector2 pos = new(
            x: rng.Next(0, boundsSize.x - size.x - 1),
            y: rng.Next(0, boundsSize.y - size.y - 1));
        pos += boundsPos;

        Room r = new(pos, size);
        return r;
    }
}
