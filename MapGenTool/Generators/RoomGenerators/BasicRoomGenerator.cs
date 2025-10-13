
using MapGenTool.Generic;
using System.Drawing;

namespace MapGenTool.Generators.RoomGenerators;

class BasicRoomGenerator : IGenerator<Tiles> {
    public byte ArgsCount => 3;

    public bool UsesInput => false;
    public int RoomsCount { get; set; }
    public int MinSize { get; set; }
    public int MaxSize { get; set; }

    public Type InputType => throw new NotImplementedException();

    public Tiles[,] Generate(int width, int height, int seed) {
        Tiles[,] grid = new Tiles[width, height];
        Random rng = new(seed);
        List<Room> rooms = [];

        long terminationCount = RoomsCount * RoomsCount * RoomsCount;
        int tryCount = 0;

        while (rooms.Count < RoomsCount && tryCount < terminationCount) {
            IntVector2 size = new(rng.Next(MinSize, MaxSize - 1), rng.Next(MinSize, MaxSize - 1));
            IntVector2 pos = new(rng.Next(0, width - size.x - 1), rng.Next(0, height - size.y - 1));
            Room tmpRoom = new(pos, size);
            int j = 0;
            while (j < rooms.Count && !IsOverLapping(rooms[j], tmpRoom)) {
                j++;
            }
            if (j < rooms.Count) {
                tryCount++;
                continue;
            }
            if (rooms.Count > 0)
                DrawCorridors(ref grid, rooms[^1], tmpRoom);
            rooms.Add(tmpRoom);

            DrawRoom(ref grid, tmpRoom);
            Console.WriteLine(tryCount);
            tryCount = 0;
        }
        Console.WriteLine($"terminated at {tryCount} tries");

        return grid;
    }
    private void DrawRoom(ref Tiles[,] grid, Room room) {
        IntVector2 pos = room.Position;
        IntVector2 otherCorner = pos + room.Size;

        for (int y = pos.y; y < otherCorner.y; y++)
            for (int x = pos.x; x < otherCorner.x; x++)
                grid[x, y] = Tiles.Space;
    }

    private bool IsOverLapping(Room a, Room b) {
        IntVector2 aPos = a.Position + IntVector2.One;
        IntVector2 aEnd = aPos + a.Size + IntVector2.One;
        IntVector2 bPos = b.Position + IntVector2.One;
        IntVector2 bEnd = bPos + b.Size + IntVector2.One;

        bool aLeftToB = aEnd.x < bPos.x;
        bool aRightToB = aPos.x > bEnd.x;
        bool aAboveB = aEnd.y < bPos.y;
        bool aBelowB = aPos.y > bPos.y;

        if ((aLeftToB || aRightToB) && (aAboveB || aBelowB))
            return false;

        return true;
    }
    private void DrawCorridors(ref Tiles[,] grid, Room a, Room b) {
        IntVector2 aCenter = a.Position + (a.Size / 2);
        IntVector2 bCenter = b.Position + (b.Size / 2);

        int xDiff = Math.Abs(bCenter.x - aCenter.x);
        int yDiff = Math.Abs(bCenter.y - aCenter.y);

        if (xDiff > yDiff) // connect horrizontally first
        {
            int xMin = Math.Min(aCenter.x, bCenter.x);
            int xMax = Math.Max(aCenter.x, bCenter.x);
            for (int x = xMin; x <= xMax; x++)
                grid[x, aCenter.y] = Tiles.Space;

            int yMin = Math.Min(aCenter.y, bCenter.y);
            int yMax = Math.Max(aCenter.y, bCenter.y);
            for (int y = yMin; y <= yMax; y++)
                grid[bCenter.x, y] = Tiles.Space;
        }
        else // connect vertically first
        {
            int yMin = Math.Min(aCenter.y, bCenter.y);
            int yMax = Math.Max(aCenter.y, bCenter.y);
            for (int y = yMin; y <= yMax; y++)
                grid[aCenter.x, y] = Tiles.Space;

            int xMin = Math.Min(aCenter.x, bCenter.x);
            int xMax = Math.Max(aCenter.x, bCenter.x);
            for (int x = xMin; x <= xMax; x++)
                grid[x, bCenter.y] = Tiles.Space;
        }
    }

    public void Parse(params string[] args) {
        RoomsCount = int.Parse(args[0]);
        MinSize = int.Parse(args[1]);
        MaxSize = int.Parse(args[2]);
    }

    public void SetBaseGrid<T>(T[,] basegrid) where T : IConvertible {
        throw new NotImplementedException();
    }
}
