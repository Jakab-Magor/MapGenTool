using MapGenTool;
using MapGenTool.Generators;
using MapGenTool.Generators.NoiseGenerators;
using MapGenTool.Generators.RoomBasedGenerators;

int width = 1980;
int height = 1080;
float scale = MathF.Min(1980 / width, 1080 / height);
int seed = 0x_0020;

VoronoiNoiseGenerator voronoi = new(100);
SobelEdgeDetection sobel = new (voronoi.Generate);
BSPTree bsp = new(30);

//ILevelGenerator generator = new ThresholdClamper(sobel.Generate,0.0505f);
TileGrid test = new(bsp.Generate(width,height,seed));

string path = MapDrawer.DrawBitMap("./", "test1", test, scale);
Console.WriteLine($"Succesfully created map at {path}");
