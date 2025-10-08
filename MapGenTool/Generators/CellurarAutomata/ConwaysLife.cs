
namespace MapGenTool.Generators.CellurarAutomata;

public class ConwaysLife : IGenerator<Tiles>
{
    public int Iterations { get; set; }
    private Tiles[,] _baseGrid = null!;

    public bool UsesInput => true;

    public byte ArgsCount => 1;

    public Type InputType => typeof(Tiles);

    public Tiles[,] Generate(int width, int height, int seed)
    {
        Tiles[,] prevGrid = _baseGrid;
        Tiles[,] nextGrid = new Tiles[width, height];

        for (int i = 0; i < Iterations; i++)
        {
            StepGrid(prevGrid, ref nextGrid, width, height);
            prevGrid = nextGrid;
        }

        return prevGrid;
    }

    private void StepGrid(Tiles[,] prevGrid, ref Tiles[,] nextGrid, int width, int height)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                nextGrid[x, y] = NextStepValue(prevGrid, x, y, width, height);
            }
        }
    }

    private Tiles NextStepValue(Tiles[,] grid, int x, int y, int width, int height)
    {
        int aliveNeighbours = 0;

        for (int p = -1; p <= 1; p++)
        {
            for (int q = -1; q <= 1; q++)
            {
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

    public void Parse(params string[] args)
    {
        Iterations = int.Parse(args[0]);
    }

    public void SetBaseGrid<T>(T[,] basegrid) where T: IConvertible
    {
        _baseGrid = IGenerator<Tiles>.CastGrid<T>(basegrid);
    }
}
