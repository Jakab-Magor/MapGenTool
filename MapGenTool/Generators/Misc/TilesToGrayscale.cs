
namespace MapGenTool.Generators;

public static partial class Misc {
    public static byte[,] TilesToGrayscale(int width, int height, Tiles[,] input, byte low, byte high) {
        byte[,] r=new byte[width, height];

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                r[x, y] = input[x, y] == Tiles.Space ? high : low;
            }
        }

        return r;
    }
}