
namespace MapGenTool.Generators;

public static partial class Misc {
    public static byte[,] ContrastBoost(int width, int height, byte[,] baseGrid, float brightness, float contrast) {
        byte[,] r = new byte[width, height];

        int sumBrightness = 0;
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                sumBrightness += baseGrid[x, y];
            }
        }

        byte midpoint = (byte)(sumBrightness/(width*height));

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                byte v = baseGrid[x, y];

                float newVal = (v - midpoint) * contrast + midpoint + brightness;
                newVal = Math.Clamp(newVal, byte.MinValue, byte.MaxValue);

                r[x, y] = (byte)newVal;
            }
        }

        return r;
    }
}