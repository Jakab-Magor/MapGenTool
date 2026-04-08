using MapGenTool.Generic;

namespace MapGenTool.Generators;

public static partial class Erosion {
    public static Tiles[,] Blobify(int width, int height, Tiles[,] baseGrid, int radius) {
        int n = (int)Math.Ceiling(radius * radius * Math.PI);
        (int x, int y)[] circle = new (int x, int y)[n];

        for (int y = -radius; y <= radius; y++) {
            for (int x = -radius; x <= radius; x++) {
                if (x * x + y * y <= radius * radius) {
                    circle[--n] = ((x, y));
                }
            }
        }

        Tiles[,] copy = (Tiles[,])baseGrid.Clone();

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {

                if (baseGrid[x, y] == Tiles.Space) {
                    // Only on edges not on tiles surrounded by spaces
                    for (int i = 0; i < circle.Length; i++) {
                        (int dx, int dy) = circle[i];
                        int nx = x + dx;
                        int ny = y + dy;

                        if (nx < 0 || nx >= width || ny < 0 || ny >= height) {
                            continue;
                        }

                        copy[nx, ny] = Tiles.Space;
                    }
                }
            }
        }

        return copy;
    }
}
