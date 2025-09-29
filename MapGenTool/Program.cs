using MapGenTool;
using MapGenTool.Generators;
using MapGenTool.Generators.CellurarAutomata;
using MapGenTool.Generators.ErosionGenerators;
using MapGenTool.Generators.NoiseGenerators;
using MapGenTool.Generators.RoomGenerators;

int width = 120;
int height = 30;
float scale = MathF.Min(1980 / width, 1080 / height);
int seed = 0x_0020;

VoronoiNoiseGenerator voronoi = new(10);
SobelEdgeDetection sobel = new (voronoi.Generate);
ThresholdClamper clamper = new(sobel.Generate, 0.04f);
BSPTree bsp = new(partitionCount:2);
DrunkardsWalk drunkardsWalk = new(agents: 5, iterations: 10, stepLength: 7);
ConwaysLife conway = new(bsp.Generate, 2);

//ILevelGenerator generator = new ThresholdClamper(sobel.Generate,0.0505f);
Console.WriteLine("Starting generation.");

var test = conway.Generate(width,height,seed);
string path = MapDrawer.DrawBitMap("./test1.png", test, scale, (int)scale);
Console.WriteLine($"Succesfully created map at {path}");
//Process.Start("explorer.exe", path);

/// -----------------------------------
/// TODO:
/// -----------------------------------
/// - Own Erosion algorythm
/// - Fix drunkards walk to be drunkards walk
/// - Drunkard's with base texture
/// - Diffusion limited aggregation
/// - Perlin noise
/// - Simplex noise
/// - Dijkstra map
/// - Simple room placer
/// - Fused room placer
/// - Prefab based generator
/// - Wave function collapse
/// - Map Drawer file handling
///     - Name files so no overrides
///     - Open image editor when finished
/// - args
/// 
/// -----------------------------------
/// Questions to Orosz:
/// -----------------------------------
/// - Documentation generraly
/// - Documentation in comments
/// - Code readability
/// - Optimization
///     - Multithreading and GPU
/// - Making GUI tester for testing
