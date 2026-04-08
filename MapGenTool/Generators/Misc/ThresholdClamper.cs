namespace MapGenTool.Generators;

public static partial class Misc {
    public static Tiles[,] ThresholdClamper(int width, int height, int seed, byte[,] baseGrid,float threshold) {
        Tiles[,] result = new Tiles[width, height];

        byte scaledThreshold = (byte)(threshold * byte.MaxValue);

        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                result[x, y] = baseGrid[x, y] < scaledThreshold ? Tiles.Wall : Tiles.Space;

        return result;
    }
}
