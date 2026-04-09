namespace MapGenTool.Generators;

public static partial class Misc {
    public static Tiles[,] ThresholdClamper(int width, int height, int seed, byte[,] baseGrid, float threshold) {
        Tiles[,] result = new Tiles[width, height];

        byte min = byte.MaxValue;
        byte max = byte.MinValue;
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                byte v = baseGrid[x, y];
                if (v < min) {
                    min = v;
                }
                if (v > max) {
                    max = v;
                }
            }
        }

        byte scaledThreshold = (byte)(threshold * (max-min) + min);

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                result[x, y] = baseGrid[x, y] < scaledThreshold ? Tiles.Wall : Tiles.Space;
            }
        }

        return result;
    }
}
