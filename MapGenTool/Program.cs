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
string path = "./test1.png";

VoronoiNoiseGenerator voronoi = new(10);
SobelEdgeDetection sobel = new(voronoi.Generate);
ThresholdClamper clamper = new(sobel.Generate, 0.4f);
BSPTree bsp = new(partitionCount: 8);
DrunkardsWalk drunkardsWalk = new(bsp.Generate, agents: 200, iterations: 10, stepLength: 2);
ConwaysLife conway = new(bsp.Generate, 1);

Console.WriteLine("Starting generation.");

var test = drunkardsWalk.Generate(width, height, seed);

string fullPath = Path.GetFullPath(path);
MapDrawer.DrawBitMap(fullPath, test, scale, (int)scale);
Console.WriteLine($"Succesfully created map at {fullPath}");
if (File.Exists(fullPath))
    Process.Start(new ProcessStartInfo(fullPath) { UseShellExecute = true });
else
    return 1;

return 0;
