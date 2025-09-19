using MapGenTool;
using MapGenTool.Generators;
using MapGenTool.Generators.NoiseGenerators;

int width = 1980;
int height = 1080;
float scale = MathF.Min(1980 / width, 1080 / height);
int seed = 0x_0020;

VoronoiNoiseGenerator voronoi = new(100);
SobelEdgeDetection sobel = new (voronoi.Generate);

ILevelGenerator generator = new ThresholdClamper(sobel.Generate,0.04f);
TileGrid test = new(generator.Generate(width,height,seed));

string path = MapDrawer.DrawBitMap("./", "test1", test, scale);
Console.WriteLine($"Succesfully created map at {path}");
