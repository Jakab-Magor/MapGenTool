using System.CommandLine;
using MapGenTool;
using MapGenTool.Generators;
using MapGenTool.Generators.CellurarAutomata;
using MapGenTool.Generators.ErosionGenerators;
using MapGenTool.Generators.NoiseGenerators;
using MapGenTool.Generators.RoomGenerators;
using System.Diagnostics;

Option<int> widthOption = new("--width", "-w")
{
    Description = "Width of the map",
    Required = true
};
Option<int> heightOption = new("--height", "-h")
{
    Description = "Height of the map",
    Required = true
};
Option<float> scaleOption = new("--scale", "-S")
{
    Description = "Scale factor for the image",
    Required = false,
    DefaultValueFactory = parseResult => 1f
};
Option<int> seedOption = new("--seed", "-s")
{
    Description = "Seed for generation",
    Required = false,
    DefaultValueFactory = parseResult => Random.Shared.Next()
};
Argument<FileInfo> pathArgument = new("path")
{
    Description = "Output path for the image"
};
Argument<string[]> pipelineArgument = new("generator-pipeline")
{
    Description = "Pipeline for the generators. Syntax: <generator-name> [value]"
};

RootCommand rootCommand = new("MapGenTool");
rootCommand.Options.Add(widthOption);
rootCommand.Options.Add(heightOption);
rootCommand.Options.Add(scaleOption);
rootCommand.Options.Add(seedOption);
rootCommand.Arguments.Add(pathArgument);
rootCommand.Arguments.Add(pipelineArgument);

GeneratorCommand<IGenerator>[] generators = [
    new GeneratorCommand<VoronoiNoiseGenerator>("voronoi"),
    new GeneratorCommand<SobelEdgeDetection>("sobel"),
    new GeneratorCommand<BSPTree>("bsp"),
    new GeneratorCommand<ThresholdClamper>("treshold-clamper"),
    new GeneratorCommand<ConwaysLife>("conways"),
    new GeneratorCommand<DrunkardsWalk>("drunkards-walk"),
    new GeneratorCommand<SimpleNoise>("simple-noise"),
    ];

ParseResult results = rootCommand.Parse(args);

if (results.Errors.Any() || results.GetValue(pathArgument) is not FileInfo fileInfo)
{
    foreach (var error in results.Errors)
        Console.Error.WriteLine(error.Message);

    return 1;
}

int width = results.GetValue(widthOption);
int height = results.GetValue(heightOption);

float scale = results.GetValue(scaleOption);

int seed = results.GetValue(seedOption);
string path = Path.GetFullPath(fileInfo.FullName);
string[] pipelineArgs = results.GetValue(pipelineArgument) ?? [];

Console.WriteLine("Starting generation...");
//var test = clamper.Generate(width, height, seed);

//MapDrawer.DrawBitMap(path, test, scale, (int)scale);
Console.WriteLine($"Successfully created map at {path}");

if (File.Exists(path))
{
    Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
}
else
{
    Console.WriteLine("Failed to find the image file.");
}

return 0;

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
/// - Benchmarking included for algorythms
