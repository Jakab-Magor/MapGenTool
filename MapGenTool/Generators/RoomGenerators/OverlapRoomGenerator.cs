
using MapGenTool.Generic;

namespace MapGenTool.Generators.RoomGenerators;

class OverlapRoomGenerator : IGenerator<Tiles> {
    public byte ArgsCount => 3;

    public bool UsesInput => false;

    public Type InputType => throw new NotImplementedException();
    public int RoomCount { get; set; }
    public int MinSize { get; set; }
    public int MaxSize { get; set; }

    public Tiles[,] Generate(int width, int height, int seed) {
        Random rng = new(seed);
        Tiles[,] grid = new Tiles[width, height];
        Room[] rooms = new Room[RoomCount];
        // Index of i contains connected room index
        // if 2 is connected to 3 then cRoomIndxs[3] = 2
        // reverse order because forward for
        int[] cRoomIndxs = new int[RoomCount];

        for (int i = 0; i < RoomCount; i++) {
            IntVector2 size = new(rng.Next(MinSize, MaxSize - 1), rng.Next(MinSize, MaxSize - 1));
            IntVector2 pos = new(rng.Next(0, width - size.x - 1), rng.Next(0, height - size.y - 1));
            Room tmpRoom = new(pos, size);

            bool insideAnother = false;
            bool overLapsAnother = false;
            for (int j = i - 1; j >= 0; j--) {
                Room curRoom = rooms[j];
                if (IsInside(curRoom, tmpRoom)) {
                    insideAnother = true;
                    break;
                }
                if (IsOverLapping(curRoom, tmpRoom)) {
                    cRoomIndxs[i] = j;
                    overLapsAnother = true;
                    break;
                }
            }
            if (insideAnother) {
                i--;
                continue;
            }
            if (!overLapsAnother) {
                cRoomIndxs[i] = -1;
            }

            rooms[i] = tmpRoom;
            DrawRoom(ref grid, tmpRoom);
        }
        for (int i = RoomCount - 1; i >= 0; i--) {
            if (cRoomIndxs[i] == -1) {
                int j = RoomCount - 1;
                while (cRoomIndxs[j]!=-1) {
                    j = cRoomIndxs[j];
                }
                cRoomIndxs[j] = i;

                DrawCorridors(ref grid, rooms[i], rooms[j]);
            }
        }

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
        IntVector2 aPos = a.Position;
        IntVector2 aEnd = aPos + a.Size;
        IntVector2 bPos = b.Position;
        IntVector2 bEnd = bPos + b.Size;

        bool aLeftToB = aEnd.x < bPos.x;
        bool aRightToB = aPos.x > bEnd.x;
        bool aAboveB = aEnd.y < bPos.y;
        bool aBelowB = aPos.y > bPos.y;

        if ((aLeftToB || aRightToB) && (aAboveB || aBelowB))
            return false;

        return true;
    }
    private bool IsInside(Room a, Room b) {
        IntVector2 aPos = a.Position;
        IntVector2 aEnd = aPos + a.Size;
        IntVector2 bPos = b.Position;
        IntVector2 bEnd = bPos + b.Size;

        bool bInsideA = aPos.x <= bPos.x && aPos.y <= bPos.y && aEnd.x >= bEnd.x && aEnd.y >= bEnd.y;
        bool aInsideB = bPos.x <= aPos.x && bPos.y <= aPos.y && bEnd.x >= aEnd.x && bEnd.y >= aEnd.y;

        return bInsideA || aInsideB;
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
        RoomCount = int.Parse(args[0]);
        MinSize = int.Parse(args[1]);
        MaxSize = int.Parse(args[2]);
    }

    public void SetBaseGrid<T>(T[,] basegrid) where T : IConvertible {
        throw new NotImplementedException();
    }
}
