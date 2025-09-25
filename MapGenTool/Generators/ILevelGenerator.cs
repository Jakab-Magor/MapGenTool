namespace MapGenTool.Generators;

public interface ILevelGenerator
{
    public Tiles[,] Generate(int width, int height, int seed);
}
