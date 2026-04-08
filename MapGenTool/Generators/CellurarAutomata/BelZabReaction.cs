
namespace MapGenTool.Generators;

public static partial class CellurarAutomata {
    public static byte[,] BelousovZhabotinskyReaction(int width, int height, byte[,] baseGrid, int iterations, byte k1, byte k2, byte n, byte g) {
        byte[,] prevGrid = baseGrid;
        byte[,] nextGrid = new byte[width, height];

        for (int i = 0; i < iterations; i++) {
            // Step the grid
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    nextGrid[x, y] = NextStepValue(prevGrid, x, y);
                }
            }
            (prevGrid, nextGrid) = (nextGrid, prevGrid);
        }

        return prevGrid;

        byte NextStepValue(byte[,] grid, int x, int y) {
            byte value = grid[x, y];

            // ill
            if (value==n) {
                return 0;
            }

            int infectedNeighbours = 0;
            int illNeighbours = 0;
            int sum = 0;
            for (int p = -1; p <= 1; p++) {
                for (int q = -1; q <= 1; q++) {
                    if (p == 0 && q == 0)
                        continue;

                    int i = x + p;
                    int j = y + q;

                    if (i < 0 || j < 0)
                        continue;
                    if (i >= width || j >= height)
                        continue;

                    byte val = grid[i, j];
                    if (val == n) {
                        illNeighbours++;
                    }
                    if (val != 0) {
                        infectedNeighbours++;
                    }
                    sum += val;
                }
            }

            // healthy
            if (value == 0) {
                int infectedVal = infectedNeighbours / k1;
                int illVal= illNeighbours / k2;
                return (byte)(infectedVal + illVal);
            }

            // infected
            return (byte)(sum / (infectedNeighbours + illNeighbours + 1) + g);
        }
    }
}
