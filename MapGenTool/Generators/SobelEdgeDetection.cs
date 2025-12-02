namespace MapGenTool.Generators;

public static partial class Rooms {
    public static byte[,] Sobel(int width, int height, int seed, byte[,] baseGrid) {
        byte[,] resultGrid = new byte[width, height];

        int[,] gx = new int[3, 3] {
        {-1,0,1 },
        {-2,0,2 },
        {-1,0,1 }};

        int[,] gy = new int[3, 3] {
        {-1,-2,-1 },
        { 0, 0, 0 },
        { 1, 2, 1 }};

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {

                int sumx = 0, sumy = 0;
                for (int ky = -1; ky <= 1; ky++) {
                    for (int kx = -1; kx <= 1; kx++) {
                        int ix = Math.Clamp(x + kx, 0, width - 1);
                        int iy = Math.Clamp(y + ky, 0, height - 1);

                        sumx += baseGrid[ix, iy] * gx[ky + 1, kx + 1];
                        sumy += baseGrid[ix, iy] * gy[ky + 1, kx + 1];
                    }
                }

                float gradient = MathF.Sqrt(sumx * sumx + sumy * sumy);
                resultGrid[x, y] = (byte)Math.Clamp(gradient, 0, 255);
                //resultGrid[x, y] = (byte)Math.Clamp(sumx+sumy, 0, 255); // very funky but not what we're looking for.
            }
        }

        return resultGrid;
    }
}
