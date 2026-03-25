namespace MapGenTool.Generators;
public static partial class Misc {
    public static byte[,] Multiply(int width, int height, int seed, byte[,] a, byte[,] b) {
        byte[,] r = new byte[width, height];
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                r[x, y] = (byte)(a[x, y] * b[x, y] / 255);
            }
        }
        return r;
    }
}
