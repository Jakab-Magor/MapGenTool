using MapGenTool;
using MapGenTool.Generators;
using MapGenTool.Generators.CellurarAutomata;
using MapGenTool.Generators.ErosionGenerators;
using MapGenTool.Generators.NoiseGenerators;
using MapGenTool.Generators.RoomGenerators;
using System.Diagnostics;

int width = 120;
int height = 30;
float scale = MathF.Min(1980 / width, 1080 / height);
int seed = 0x_0020;

VoronoiNoiseGenerator voronoi = new(10);
SobelEdgeDetection sobel = new (voronoi.Generate);
ThresholdClamper clamper = new(sobel.Generate, 0.4f);
BSPTree bsp = new(partitionCount:8);
DrunkardsWalk drunkardsWalk = new(agents: 5, iterations: 10, stepLength: 7);
ConwaysLife conway = new(bsp.Generate, 1);

//ILevelGenerator generator = new ThresholdClamper(sobel.Generate,0.0505f);
Console.WriteLine("Starting generation.");

var test = clamper.Generate(width,height,seed);

string path = Path.GetFullPath("./test1.png");
MapDrawer.DrawBitMap(path, test, scale, (int)scale);
Console.WriteLine($"Succesfully created map at {path}");
Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
