namespace MapGenTool.Generators;

public static partial class Noise {
    public static byte[,] WhiteNoise(int width, int height, int seed) {
        byte[,] tiles = new byte[width, height];
        Random rng = new(seed);

        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++) {
                byte value = (byte)rng.Next(0, 256);
                tiles[x, y] = value;
            }

        return tiles;
    }
}
