using MapGenTool.Generators;

namespace MapGenTool.Generators.NoiseGenerators;

public class SimpleNoise() : IGenerator<byte>
{
    public bool UsesInput => false;

    public byte ArgsCount => 0;

    public Type InputType => throw new NotImplementedException();

    public byte[,] Generate(int width, int height, int seed)
    {
        byte[,] tiles = new byte[width, height];
        Random rng = new(seed);

        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                byte value = (byte)rng.Next(0,256);
                tiles[x, y] = value;
            }

        return tiles;
    }

    public void Parse(params string[] args) { }

    public void SetBaseGrid<T>(T[,] basegrid) where T : IConvertible
    {
        throw new NotImplementedException();
    }
}
