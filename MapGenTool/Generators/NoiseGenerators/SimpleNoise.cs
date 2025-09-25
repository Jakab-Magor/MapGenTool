using MapGenTool.Generators;

namespace MapGenTool.Generators.NoiseGenerators;

public class SimpleNoise : ILevelGenerator
{
    private readonly int _maxTile = 2;

    public SimpleNoise()
    {
    }

    public SimpleNoise(int maxTile)
    {
        _maxTile = maxTile;
    }


    public Tiles[,] Generate(int width, int height, int seed)
    {
        Tiles[,] tiles = new Tiles[width, height];
        Random rng = new(seed);

        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                Tiles value = (Tiles)rng.Next(_maxTile);
                tiles[x, y] = value;
            }

        return tiles;
    }
}
