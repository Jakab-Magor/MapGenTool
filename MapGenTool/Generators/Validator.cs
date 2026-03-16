
namespace MapGenTool.Generators;

public static partial class Misc {
    public static byte[,] Validate(int width, int height, Tiles[,] input) {
        int[,] dijkstraMap = new int[width, height];

        Queue<(int, int)> dijsktraQ = new();
        (int, int) center = (width / 2, height / 2);
        dijsktraQ.Enqueue(center);

        int[] dx = { 1, -1, 0, 0 };
        int[] dy = { 0, 0, 1, -1 };

        while (dijsktraQ.Count > 0) {
            (int x, int y) = dijsktraQ.Dequeue();

            for (int i = 0; i < dx.Length; i++) {
                int nx = x + dx[i];
                int ny = y + dy[i];

                if (nx < 0 || nx >= width || ny < 0 || ny >= height)
                    continue;

                ref var next = ref dijkstraMap[nx, ny];

                if (next == 0) {
                    if (input[nx, ny] == Tiles.Space) {
                        next = -1;
                    } else {
                        int currentDist = dijkstraMap[x, y] + 1;
                        if (currentDist == 0) {
                            currentDist += 1;
                        }
                        next = currentDist;
                    }

                    dijsktraQ.Enqueue((nx, ny));
                }
            }
        }

        byte[,] result = new byte[width, height];
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                int val = dijkstraMap[x, y];
                result[x, y] = (byte)(val == -1 ? 0 : 255 - val*6);
            }
        }
        return result;
    }
}