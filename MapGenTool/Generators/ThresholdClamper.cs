using System.ComponentModel.DataAnnotations;

namespace MapGenTool.Generators;

class ThresholdClamper(Func<int, int, int, byte[,]> generator, float threshold) : ILevelGenerator
{
    private readonly Func<int, int, int, byte[,]> _generator = generator;
    [Range(0, 1, MinimumIsExclusive = false, MaximumIsExclusive = false)]
    public float Threshold { get; set; } = threshold;

    public Tiles[,] Generate(int width, int height, int seed)
    {
        byte[,] baseGrid = _generator.Invoke(width, height, seed);
        Tiles[,] result = new Tiles[width, height];

        byte scaledThreshold = (byte)(Threshold * byte.MaxValue);

        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                result[x, y] = baseGrid[x, y] < scaledThreshold ? Tiles.Wall : Tiles.Space;

        return result;
    }
}
