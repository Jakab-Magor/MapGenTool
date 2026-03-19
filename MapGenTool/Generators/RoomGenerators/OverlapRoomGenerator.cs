
using MapGenTool.Generic;

namespace MapGenTool.Generators;

static partial class Rooms {
    public static Tiles[,] OverlapRooms(int width, int height, int seed, int roomCount, int minSize, int maxSize) {
        Random rng = new(seed);
        Tiles[,] grid = new Tiles[width, height];
        Room[] rooms = new Room[roomCount];

        for (int i = 0; i < roomCount; i++) {
            IntVector2 size = new(rng.Next(minSize, maxSize - 1), rng.Next(minSize, maxSize - 1));
            IntVector2 pos = new(rng.Next(0, width - size.x - 1), rng.Next(0, height - size.y - 1));
            Room tmpRoom = new(pos, size);

            bool insideAnother = false;
            for (int j = i - 1; j >= 0; j--) {
                Room curRoom = rooms[j];
                if (IsInside(curRoom, tmpRoom)) {
                    insideAnother = true;
                    break;
                }
            }
            if (insideAnother) {
                i--;
                continue;
            }

            rooms[i] = tmpRoom;
            //draw room
            IntVector2 otherCorner = pos + size;

            for (int y = pos.y; y < otherCorner.y; y++)
                for (int x = pos.x; x < otherCorner.x; x++)
                    grid[x, y] = Tiles.Space;
        }

        return grid;

        static bool IsInside(Room a, Room b) {
            IntVector2 aPos = a.Position;
            IntVector2 aEnd = aPos + a.Size;
            IntVector2 bPos = b.Position;
            IntVector2 bEnd = bPos + b.Size;

            bool bInsideA = aPos.x <= bPos.x && aPos.y <= bPos.y && aEnd.x >= bEnd.x && aEnd.y >= bEnd.y;
            bool aInsideB = bPos.x <= aPos.x && bPos.y <= aPos.y && bEnd.x >= aEnd.x && bEnd.y >= aEnd.y;

            return bInsideA || aInsideB;
        }
    }
}
