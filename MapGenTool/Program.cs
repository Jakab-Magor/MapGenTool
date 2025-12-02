using MapGenTool;
using MapGenTool.Generators;
using System.CommandLine;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Xml.Linq;

/// ----------------------------------------------
/// CLI tooling
/// ----------------------------------------------
Option<int> widthOption = new("--width", "-W") {
    Description = "Width of the map",
    Required = true
};
Option<int> heightOption = new("--height", "-H") {
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
/*Option<bool> benchmarkingOption = new("--benchmark", "-b") {
    Description = "Whether the program should display benchmarking for each pass.",
    Required = false,
    DefaultValueFactory = parseResult => false
};*/
Option<Verbosity> verbosityOption = new("--verbose", "-v") {
    Description = "Verbosity of the output",
    Required = false,
    DefaultValueFactory = parseResult => Verbosity.Generators
};
Option<bool> displayImageOption = new("--display", "-d") {
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
//rootCommand.Options.Add(benchmarkingOption);
rootCommand.Options.Add(verbosityOption);
rootCommand.Options.Add(displayImageOption);
rootCommand.Arguments.Add(pathArgument);
rootCommand.Arguments.Add(pipelineArgument);

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
//bool displayBenchmark = results.GetValue(benchmarkingOption);
Verbosity verbosity = results.GetValue(verbosityOption);
bool displayImageInExplorer = results.GetValue(displayImageOption);

int seed = results.GetValue(seedOption);

/// ----------------------------------------------
/// Pipeline parsing
/// ----------------------------------------------
Console.WriteLine("Starting generation...");
string[] pipelineArgsStrings = results.GetValue(pipelineArgument) ?? [];

byte[,] byteGrid = null!;
Tiles[,] tileGrid = null!;
Type lastType = null!;

ParseTreeNode parseRoot = new(null, default);
// Construct Parsetree
for (int i = 0; i < pipelineArgsStrings.Length; i++) {
    
}

for (int i = 0; i < pipelineArgsStrings.Length; i++) {
    string name = pipelineArgsStrings[i];

    switch (name) {
        case "voronoi":
            if (i > 0) goto notFirstGeneratorError;
            break;
        case "sobel":
            if (i == 0) goto firstGeneratorError;
            break;
        case "perwitt":
            if (i == 0) goto firstGeneratorError;
            break;
        case "bsp":
            if (i > 0) goto notFirstGeneratorError;
            break;
        case "treshold-clamper":
            if (i == 0) goto firstGeneratorError;
            break;
        case "conways":
            if (i == 0) goto firstGeneratorError;
            break;
        case "drunkards-walk":
            if (i == 0) goto firstGeneratorError;
            break;
        case "simple-noise":
            break;
        case "basic-rooms":
            break;
        case "inverter":
            if (i == 0) goto firstGeneratorError;
            break;
        case "byte-inverter":
            if (i == 0) goto firstGeneratorError;
            break;
        case "overlap-rooms":
            break;
        case "prefab":
            break;
        default:
            Console.Error.WriteLine($"Invalid pipeline. Unknown generator {name}.");
            return 1;
    }

    Console.Error.WriteLine($"Invalid pipeline. Generator used at start '{name}' which requires input.");

    Console.Error.WriteLine($"Invalid pipeline. Generator '{name}' cannot be used with an input.");

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

    i = argsOffsetIndex - 1;
    StringBuilder lineBuilder = new();
    if (verbosity.HasFlag((Verbosity)2)) {
        lineBuilder.AppendLine($"\t{i + 1}.{name} ({String.Join(", ", genArgs)})");
    }
    if (verbosity.HasFlag((Verbosity)8)) {
        TimeSpan time = sWatch.Elapsed;
        lineBuilder.AppendLine($": {time.TotalMilliseconds}ms");
    }

    string[] fetchArgs(int count) {
        string[] genArgs = new string[count];
        int argsOffsetIndex = i + 1;
        for (int argOffset = 0; argOffset < genArgs.Length && argsOffsetIndex < pipelineArgsStrings.Length; argOffset++, argsOffsetIndex++) {
            genArgs[argOffset] = pipelineArgsStrings[argsOffsetIndex];
        }
        /*if (argsOffsetIndex >= pipelineArgsStrings.Length) {
            Console.Error.WriteLine($"Invalid pipeline arguments. Generator '{name}' requires {genArgs.Length} argument(s).");
            return 1;
        }*/
        return genArgs;
    }
}

/// ----------------------------------------------
/// Generating image
/// ----------------------------------------------

string[] fileNameSegments = fileInfo.Name.Split('.');
StringBuilder pathBuilder = new();
int counter = 1;
do {
    pathBuilder.Clear();
    pathBuilder.Append(fileNameSegments[0]);
    pathBuilder.Append('_');
    pathBuilder.Append(counter);
    for (int i = 1; i <= fileNameSegments.Length - 1; i++) {
        pathBuilder.Append('.');
        pathBuilder.Append(fileNameSegments[i]);
    }
    pathBuilder.Insert(0, '\\');
    pathBuilder.Insert(0, fileInfo.DirectoryName);
    counter++;
} while (File.Exists(pathBuilder.ToString()));

string path = pathBuilder.ToString();
if (lastType == typeof(byte))
    MapDrawer.DrawBitMap(path, byteGrid, scale, (int)scale);
else
    MapDrawer.DrawBitMap(path, tileGrid, scale, (int)scale);

if (verbosity.HasFlag(Verbosity.Finished))
    Console.WriteLine($"Successfully created map at {path}");

if (!displayImageInExplorer)
    return 0;

Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
return 0;

/// -----------------------------------
/// TODO:
/// -----------------------------------
/// - Diffusion limited aggregation
/// - Dijkstra map
/// - Fused room placer
/// - Wave function collapse
/// - Refactor to not use polymorphism
/// 