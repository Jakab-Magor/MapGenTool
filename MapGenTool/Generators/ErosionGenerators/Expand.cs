namespace MapGenTool.Generators;

public static partial class Erosion {
    public static Tiles[,] Expand(int width, int height, Tiles[,] baseGrid, int iterations) {
        int[] dx = { -1, 1, 0, 0 };
        int[] dy = { 0, 0, -1, 1 };

        Tiles[,] a = baseGrid;
        Tiles[,] b = (Tiles[,])a.Clone();

        while (iterations-- > 0) {

            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {

                    if (a[x, y] == Tiles.Space) {
                        // Only on edges not on tiles surrounded by spaces
                        for (int i = 0; i < dx.Length; i++) {
                            int nx = x + dx[i];
                            int ny = y + dy[i];

                            if (nx < 0 || nx >= width || ny < 0 || ny >= height) {
                                continue;
                            }

                            b[nx, ny] = Tiles.Space;
                        }
                    }
                }
            }

            (a, b) = (b, a);
        }

        return a;
    }
}
