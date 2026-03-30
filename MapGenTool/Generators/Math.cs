namespace MapGenTool.Generators;
public static partial class Misc {
    public static byte[,] Multiply(int width, int height, (byte[,] a, byte[,] b) t) {
        byte[,] r = new byte[width, height];
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                r[x, y] = (byte)(t.a[x, y] * t.b[x, y] / 255);
            }
        }
        return r;
    }
    public static byte[,] Add(int width, int height, (byte[,] a, byte[,] b) t) {
        byte[,] r = new byte[width, height];
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                int v = t.a[x, y] + t.b[x, y];
                if (v > 255) {
                    v = 255;
                }
                r[x, y] = (byte)v;
            }
        }
        return r;
    }
    public static byte[,] Subtract(int width, int height, (byte[,] a, byte[,] b) t) {
        byte[,] r = new byte[width, height];
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                int v = t.a[x, y] - t.b[x, y];
                if (v < 0) {
                    v = 0;
                }
                r[x, y] = (byte)v;
            }
        }
        return r;
    }
    public static byte[,] Divide(int width, int height, (byte[,] a, byte[,] b) t) {
        byte[,] r = new byte[width, height];
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                r[x, y] = (byte)(t.a[x, y] / t.b[x, y]);
            }
        }
        return r;
    }
}
