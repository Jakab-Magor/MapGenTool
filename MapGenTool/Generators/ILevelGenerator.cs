namespace MapGenTool.Generators;

public interface ILevelGenerator
{
    public TileTypes[,] Generate(int width, int height, int seed);
}
