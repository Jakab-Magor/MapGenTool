namespace MapGenTool.Generators;
public static partial class Misc {
    public static byte[,] Multiply(int width, int height, byte[,] a, byte[,] b) {
        byte[,] r = new byte[width, height];
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                r[x, y] = (byte)(a[x, y] * b[x, y] / 255);
            }
        }
        return r;
    }
    public static byte[,] Add(int width, int height, byte[,] a, byte[,] b) {
        byte[,] r = new byte[width, height];
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                int v = a[x, y] + b[x, y];
                if (v > 255) {
                    v = 255;
                }
                r[x, y] = (byte)v;
            }
        }
        return r;
    }
    public static byte[,] Subtract(int width, int height, byte[,] a, byte[,] b) {
        byte[,] r = new byte[width, height];
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                int v = b[x, y] - a[x, y];
                if (v < 0) {
                    v = 0;
                }
                r[x, y] = (byte)v;
            }
        }
        return r;
    }
    public static byte[,] Divide(int width, int height, byte[,] a, byte[,] b) {
        byte[,] r = new byte[width, height];
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                r[x, y] = (byte)(b[x, y] / a[x, y]);
            }
        }
        return r;
    }
    public static Tiles[,] Union(int width, int height, Tiles[,] a, Tiles[,] b) {
        Tiles[,] r = new Tiles[width, height];
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                r[x, y] = a[x, y] | b[x, y];
            }
        }
        return r;
    }
    public static Tiles[,] Intersect(int width, int height, Tiles[,] a, Tiles[,] b) {
        Tiles[,] r = new Tiles[width, height];
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                r[x, y] = b[x, y] & a[x, y];
            }
        }
        return r;
    }
    public static Tiles[,] Except(int width, int height, Tiles[,] a, Tiles[,] b) {
        Tiles[,] r = new Tiles[width, height];
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                r[x, y] = b[x, y] & ~(b[x, y] & a[x, y]);
            }
        }
        return r;
    }
}
