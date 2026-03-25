
namespace MapGenTool.Generators;

public static partial class CellurarAutomata {
    public static Tiles[,] ConwaysGameOfLife(int width, int height, int seed, Tiles[,] baseGrid, int iterations) {
        Tiles[,] prevGrid = baseGrid;
        Tiles[,] nextGrid = new Tiles[width, height];

        static Tiles NextStepValue(Tiles[,] grid, int x, int y, int width, int height) {
            int aliveNeighbours = 0;

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

                    if (grid[i, j] == Tiles.Space)
                        aliveNeighbours += 1;
                }
            }

            if (aliveNeighbours > 1 && aliveNeighbours < 4)
                return Tiles.Space;
            return Tiles.Wall;
        }

        for (int i = 0; i < iterations; i++) {
            // Step the grid
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    nextGrid[x, y] = NextStepValue(prevGrid, x, y, width, height);
                }
            }
            prevGrid = nextGrid;
        }

        return prevGrid;
    }
}
