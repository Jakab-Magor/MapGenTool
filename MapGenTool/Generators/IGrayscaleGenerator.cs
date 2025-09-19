namespace MapGenTool.Generators;

interface IGrayscaleGenerator
{
    public byte[,] Generate(int width, int height, int seed);
}
