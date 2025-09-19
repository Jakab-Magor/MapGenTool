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


    public TileTypes[,] Generate(int width, int height, int seed)
    {
        TileTypes[,] tiles = new TileTypes[width, height];
        Random rng = new(seed);

        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                TileTypes value = (TileTypes)rng.Next(_maxTile);
                tiles[x, y] = value;
            }

        return tiles;
    }
}
