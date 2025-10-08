using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace MapGenTool.Generators;

class ThresholdClamper() : IGenerator<Tiles>
{
    public bool UsesInput => true;
    public byte ArgsCount => 1;


    [Range(0, 1, MinimumIsExclusive = false, MaximumIsExclusive = false)]
    public float Threshold { get; set; } = default;
    private byte[,] _baseGrid = null!;

    public Type InputType => typeof(byte);

    public Tiles[,] Generate(int width, int height, int seed)
    {
        Tiles[,] result = new Tiles[width, height];

        byte scaledThreshold = (byte)(Threshold * byte.MaxValue);

        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                result[x, y] = _baseGrid[x, y] < scaledThreshold ? Tiles.Wall : Tiles.Space;

        return result;
    }

    public void Parse(params string[] args)
    {
        Threshold = float.Parse(args[0],CultureInfo.InvariantCulture);
    }

    public void SetBaseGrid<T>(T[,] basegrid) where T : IConvertible
    {
        _baseGrid = IGenerator<byte>.CastGrid<T>(basegrid);
    }
}
