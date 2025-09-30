using System.ComponentModel.DataAnnotations;

namespace MapGenTool.Generators;

class ThresholdClamper(float threshold) : IGenerator<byte,Tiles>
{
    [Range(0, 1, MinimumIsExclusive = false, MaximumIsExclusive = false)]
    public float Threshold { get; set; } = threshold;

    public Tiles[,] Generate(Tiles[,] baseGrid,int width, int height, int seed)
    {
        Tiles[,] result = new Tiles[width, height];

        byte scaledThreshold = (byte)(Threshold * byte.MaxValue);

        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                result[x, y] = baseGrid[x, y] < scaledThreshold ? Tiles.Wall : Tiles.Space;

        return result;
    }
}
