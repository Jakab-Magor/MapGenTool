using System.CommandLine;
using MapGenTool;
using MapGenTool.Generators;
using MapGenTool.Generators.CellurarAutomata;
using MapGenTool.Generators.ErosionGenerators;
using MapGenTool.Generators.NoiseGenerators;
using MapGenTool.Generators.RoomGenerators;
using System.Diagnostics;

/// ----------------------------------------------
/// CLI tooling
/// ----------------------------------------------
Option<int> widthOption = new("--width", "-w") {
    Description = "Width of the map",
    Required = true
};
Option<int> heightOption = new("--height", "-h") {
    Description = "Height of the map",
    Required = true
};
Option<float> scaleOption = new("--scale", "-S") {
    Description = "Scale factor for the image",
    Required = false,
    DefaultValueFactory = parseResult => 1f
};
Option<int> seedOption = new("--seed", "-s") {
    Description = "Seed for generation",
    Required = false,
    DefaultValueFactory = parseResult => Random.Shared.Next()
};
Option<bool> benchmarkingOption = new("--benchmark", "-b") {
    Description = "Whether the program should display benchmarking for each pass.",
    Required = false,
    DefaultValueFactory = parseResult => false
};
Option<bool> displayImageOption = new Option<bool>("--display", "-d") {
    Description = "Open image in default image viewer after finished running.",
    Required = false,
    DefaultValueFactory = parseResult => false
};
Argument<FileInfo> pathArgument = new("path") {
    Description = "Output path for the image"
};
Argument<string[]> pipelineArgument = new("generator-pipeline") {
    Description = "Pipeline for the generators. Syntax: <generator-name> [value]"
};

RootCommand rootCommand = new("MapGenTool");
rootCommand.Options.Add(widthOption);
rootCommand.Options.Add(heightOption);
rootCommand.Options.Add(scaleOption);
rootCommand.Options.Add(seedOption);
rootCommand.Options.Add(benchmarkingOption);
rootCommand.Options.Add(displayImageOption);
rootCommand.Arguments.Add(pathArgument);
rootCommand.Arguments.Add(pipelineArgument);

/// ----------------------------------------------
/// Generator instantiation for pipeline parsing
/// ----------------------------------------------
Dictionary<string, IGenerator> generators = new(){
    { "voronoi", new VoronoiNoiseGenerator()},
    { "sobel", new SobelEdgeDetection()},
    { "bsp", new BSPTree()},
    { "treshold-clamper", new ThresholdClamper()},
    { "conways", new ConwaysLife()},
    { "drunkards-walk", new DrunkardsWalk()},
    { "simple-noise", new SimpleNoise()},
};

/// ----------------------------------------------
/// Parsing
/// ----------------------------------------------
ParseResult results = rootCommand.Parse(args);

if (results.Errors.Any() || results.GetValue(pathArgument) is not FileInfo fileInfo) {
    foreach (var error in results.Errors)
        Console.Error.WriteLine(error.Message);

    return 1;
}

int width = results.GetValue(widthOption);
int height = results.GetValue(heightOption);

float scale = results.GetValue(scaleOption);
bool displayBenchmark = results.GetValue(benchmarkingOption);
bool displayImageInExplorer = results.GetValue(displayImageOption);

int seed = results.GetValue(seedOption);
string path = Path.GetFullPath(fileInfo.FullName);

/// ----------------------------------------------
/// Pipeline parsing
/// ----------------------------------------------
Console.WriteLine("Starting generation...");
string[] pipelineArgsStrings = results.GetValue(pipelineArgument) ?? [];

byte[,] byteGrid = null!;
Tiles[,] tileGrid = null!;
Type lastType = null!;
for (int i = 0; i < pipelineArgsStrings.Length; i++) {
    string name = pipelineArgsStrings[i];
    IGenerator gen = generators[name];

    string[] genArgs = new string[gen.ArgsCount];
    int argOffset = 0;
    for (; argOffset < genArgs.Length; argOffset++) {
        genArgs[argOffset] = pipelineArgsStrings[i + argOffset + 1];
    }

    gen.Parse(genArgs);

    bool first = i == 0;
    if (!(first ^ gen.UsesInput)) {
        Console.Error.WriteLine($"Invalid pipeline. Generator used at start {name} which requires input OR input provided to generator that doesn't need one.");
        return 1;
    }
    if (!first) {
        if (gen.InputType != lastType) {
            Console.Error.WriteLine($"Invalid pipeline. Generator {name} uses {gen.InputType} as input, not {lastType}");
            return 1;
        }
        if (gen.InputType == typeof(byte))
            gen.SetBaseGrid(byteGrid);
        else
            gen.SetBaseGrid(tileGrid);
    }

    Stopwatch sWatch = new();
    if (gen is IGenerator<byte> bGen) {
        sWatch.Start();
        byteGrid = bGen.Generate(width, height, seed);
        sWatch.Stop();
        lastType = typeof(byte);
    }
    else {
        sWatch.Start();
        tileGrid = ((IGenerator<Tiles>)gen).Generate(width, height, seed);
        sWatch.Stop();
        lastType = typeof(Tiles);
    }

    i += argOffset;
    if (displayBenchmark) {
        TimeSpan time = sWatch.Elapsed;
        Console.WriteLine($"\t{name} ({String.Join(", ", genArgs)}): {time.TotalMilliseconds}ms");
    }
}


/// ----------------------------------------------
/// Generating image
/// ----------------------------------------------

if (lastType == typeof(byte))
    MapDrawer.DrawBitMap(path, byteGrid, scale, (int)scale);
else
    MapDrawer.DrawBitMap(path, tileGrid, scale, (int)scale);

Console.WriteLine($"Successfully created map at {path}");

if (!displayImageInExplorer)
    return 0;

if (File.Exists(path)) {
    Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
}
else {
    Console.WriteLine("Failed to find the image file.");
}

return 0;

/// -----------------------------------
/// TODO:
/// -----------------------------------
/// - Own Erosion algorythm
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
/// - Refactor to not use polymorphism
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
