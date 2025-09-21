using MapGenTool.Generic;
using System.ComponentModel.DataAnnotations;

namespace MapGenTool.Generators.RoomBasedGenerators;

public class BSPTree(int partitionCount) : ILevelGenerator
{
    [Range(0, int.MaxValue, MinimumIsExclusive = true)]
    public int PartitionCount { get; set; } = partitionCount;

    public TileTypes[,] Generate(int width, int height, int seed)
    {
        TileTypes[,] grid = new TileTypes[width, height];
        Random rng = new(seed);

        BinaryTreeNode<Room?> root = new(null, null);
        int h = (int)Math.Ceiling(Math.Log2(PartitionCount));

        int leafCount = 0;

        var currentNode = root;
        BinaryTreeNode<Room?> lastGeneratedNode = null!;
        int currentLevel = 0;

        while (currentNode != root || currentNode.Right is null)
        {
            if (currentLevel == h)
            {
                int ch2 = (int)Math.Pow(currentLevel, 2);
                int currentWidth = width / ch2;
                int currentHeight = height / ch2;

                currentNode.Value = GetRandomRoom(ref rng,currentWidth, currentHeight);
                DrawRoom(ref grid, (Room)currentNode.Value);

                if (lastGeneratedNode is not null)
                    DrawCorridors(ref grid, (Room)currentNode.Value, (Room)lastGeneratedNode.Value!);

                lastGeneratedNode = currentNode;

                currentNode = currentNode.Parent;
                currentLevel--;
                leafCount++;
            }

            if (leafCount == PartitionCount)
                break;

            if (currentNode!.Left is null)
            {
                currentNode.Left = new(currentNode, null);
                currentNode = currentNode.Left;
                currentLevel++;
                continue;
            }

            if (currentNode.Right is null)
            {
                currentNode.Right = new(currentNode, null);
                currentNode = currentNode.Right;
                currentLevel++;
            }
        }

        return grid;
    }

    private void DrawRoom(ref TileTypes[,] grid, Room room)
    {
        IntVector2 pos = room.Position;
        IntVector2 otherCorner = pos + room.Size;

        for (int y = pos.y; y < otherCorner.y; y++)
            for(int x = pos.x; x < otherCorner.x; x++)
                grid[x, y] = TileTypes.Space;
    }

    private void DrawCorridors(ref TileTypes[,] grid, Room a, Room b)
    {

    }

    private Room GetRandomRoom(ref Random rng, int gridWidth, int gridHeight)
    {
        gridWidth--;
        gridHeight--;

        IntVector2 size = new(
            x: rng.Next(2, gridWidth),
            y: rng.Next(2, gridHeight));
        IntVector2 pos = new(
            x: rng.Next(0, gridWidth - size.x),
            y: rng.Next(0, gridHeight - size.y));

        Room r = new(size, pos);
        return r;
    }
}
