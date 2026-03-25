namespace MapGenTool.Generators;

public static partial class Patterns {
    public static byte[,] Checkerboard(int width, int height, int seed, byte dark, byte light) {
        byte[,] tiles = new byte[width, height];

        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++) {
                bool val = (x + y) % 2 == 0;
                tiles[x, y] = val ? dark : light;
            }

        return tiles;
    }
}
