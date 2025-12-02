
namespace MapGenTool.Generators;

public static partial class Misc {
    public static byte[,] ByteInverter(int width, int height, int seed, byte[,] baseGrid) {
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                baseGrid[x, y] = (byte)~baseGrid[x, y];
            }
        }

        return baseGrid;
    }
    public static Tiles[,] Inverter(int width, int height, int seed, Tiles[,] baseGrid) {
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                ref Tiles tile = ref baseGrid[x, y];
                tile = ~tile;
            }
        }

        return baseGrid;
    }
}
