using MapGenTool;
using MapGenTool.Generators;
using System.CommandLine;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Text;

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
/// Generator instantiation for pipeline parsing
/// ----------------------------------------------
Dictionary<string, GeneratorInfo> tokens = new(){
    { "voronoi",            new (paramCount: 1, GeneratorTypes.First,    returnType: typeof(byte))},
    { "sobel",              new (paramCount: 0, GeneratorTypes.Follower, returnType: typeof(byte))},
    { "perwitt",            new (paramCount: 0, GeneratorTypes.Follower, returnType: typeof(byte))},
    { "bsp",                new (paramCount: 1, GeneratorTypes.First,    returnType: typeof(Tiles))},
    { "treshold-clamper",   new (paramCount: 1, GeneratorTypes.Follower, returnType: typeof(Tiles))},
    { "conways",            new (paramCount: 1, GeneratorTypes.Follower, returnType: typeof(Tiles))},
    { "drunkards-walk",     new (paramCount: 3, GeneratorTypes.Follower, returnType: typeof(Tiles))},
    { "simple-noise",       new (paramCount: 0, GeneratorTypes.First,    returnType: typeof(byte))},
    { "basic-rooms",        new (paramCount: 0, GeneratorTypes.First,    returnType: typeof(Tiles))},
    { "inverter",           new (paramCount: 0, GeneratorTypes.Follower, returnType: typeof(Tiles))},
    { "byte-inverter",      new (paramCount: 0, GeneratorTypes.Follower, returnType: typeof(byte))},
    { "overlap-rooms",      new (paramCount: 3, GeneratorTypes.First,    returnType: typeof(Tiles))},
    { "prefab",             new (paramCount: 1, GeneratorTypes.First,    returnType: typeof(Tiles))},
    { "multiply",           new (paramCount: 0, GeneratorTypes.Binary,   returnType: typeof(byte))},
    { "checkerboard",       new (paramCount: 2, GeneratorTypes.First,    returnType: typeof(byte))},
    { "perlin",             new (paramCount: 1, GeneratorTypes.First,    returnType: typeof(byte))},
    { "validate",           new (paramCount: 0, GeneratorTypes.Follower, returnType: typeof(byte))},
};

/// ----------------------------------------------
/// Pipeline parsing
/// ----------------------------------------------
Console.WriteLine("Starting generation...");
string[] pipeline = results.GetValue(pipelineArgument) ?? [];
Stack<byte[,]> byteStack = new();
Stack<Tiles[,]> tileStack = new();

(Type? returnType, int nextIdx) Parse(int idx, Type? lastT, int lastP, int nesting) {
    Type? t = null;
    int p = 0;
    while (idx < pipeline.Length) {
        ref string name = ref pipeline[idx];

        if (name == "(") {
            p = 0;
            (t, int n) = Parse(idx + 1, lastT, p, nesting + 1);
            idx = n;
            continue;
        }
        if (name == ")") {
            break;
        }

        if (!tokens.TryGetValue(name, out GeneratorInfo? info)) {
            throw new ArgumentException($"Pipeline parsing error. Unknown generator {name}.");
        }

        int paramOffset = info.paramCount;
        string[] generatorArgs = new string[paramOffset];
        int idxOffset = idx + 1;
        paramOffset += idxOffset;
        for (int j = 0; idxOffset < paramOffset && idxOffset < pipeline.Length; idxOffset++, j++) {
            generatorArgs[j] = pipeline[idxOffset];
        }
        idx = idxOffset - 1;

        switch (info.generatorType) {
            case GeneratorTypes.First:
                if (t is not null) {
                    throw new ArgumentException($"Pipeline parsing error. Input given to {name} which does not need it.");
                }
                p = 1;
                break;
            case GeneratorTypes.Follower:
                if (t is null) {
                    throw new ArgumentException($"Pipeline parsing error. Invalid pipeline. No valid input provided to {name}.");
                }
                p = 1;
                break;
            case GeneratorTypes.Binary:
                if (t is null) {
                    throw new ArgumentException($"Pipeline parsing error. No valid generator provided to {name} on the left.");
                }
                p = 2;
                (t, int n) = Parse(paramOffset, null, p, nesting + 1);
                idx = n - 1;
                break;
        }

        Stopwatch sWatch = new();
        try {
            sWatch.Start();
            switch (name) {
                case "voronoi":
                    byteStack.Push(Noise.Voronoi(width, height, seed,
                        int.Parse(generatorArgs[0])));
                    break;
                case "sobel":
                    byteStack.Push(EdgeDetection.Sobel(width, height, seed, byteStack.Pop()));
                    break;
                case "perwitt":
                    byteStack.Push(EdgeDetection.Prewitt(width, height, seed, byteStack.Pop()));
                    break;
                case "bsp":
                    tileStack.Push(Rooms.BSPRooms(width, height, seed,
                        int.Parse(generatorArgs[0])));
                    break;
                case "treshold-clamper":
                    tileStack.Push(Misc.ThresholdClamper(width, height, seed, byteStack.Pop(),
                        float.Parse(generatorArgs[0], CultureInfo.InvariantCulture)));
                    break;
                case "conways":
                    tileStack.Push(CellurarAutomata.ConwaysGameOfLife(width, height, seed, tileStack.Pop(),
                        int.Parse(generatorArgs[0])));
                    break;
                case "drunkards-walk":
                    tileStack.Push(Erosion.DrunkardsWalk(width, height, seed, tileStack.Pop(),
                        int.Parse(generatorArgs[0]),
                        int.Parse(generatorArgs[1]),
                        int.Parse(generatorArgs[2])));
                    break;
                case "simple-noise":
                    byteStack.Push(Noise.WhiteNoise(width, height, seed));
                    break;
                case "basic-rooms":
                    tileStack.Push(Rooms.BasicRooms(width, height, seed,
                        int.Parse(generatorArgs[0]),
                        int.Parse(generatorArgs[1]),
                        int.Parse(generatorArgs[2])));
                    break;
                case "inverter":
                    tileStack.Push(Misc.Inverter(width, height, seed, tileStack.Pop()));
                    break;
                case "byte-inverter":
                    byteStack.Push(Misc.ByteInverter(width, height, seed, byteStack.Pop()));
                    break;
                case "overlap-rooms":
                    tileStack.Push(Rooms.OverlapRooms(width, height, seed,
                        int.Parse(generatorArgs[0]),
                        int.Parse(generatorArgs[1]),
                        int.Parse(generatorArgs[2])));
                    break;
                case "prefab":
                    tileStack.Push(Rooms.Prefabs(width, height, seed,
                        generatorArgs[0]));
                    break;
                case "multiply":
                    byteStack.Push(Misc.Multiply(width, height, seed, byteStack.Pop(), byteStack.Pop()));
                    break;
                case "checkerboard":
                    byteStack.Push(Patterns.Checkerboard(width, height, seed,
                        byte.Parse(generatorArgs[0]),
                        byte.Parse(generatorArgs[1])));
                    break;
                case "perlin":
                    byteStack.Push(Noise.Perlin2D(width, height, seed,
                        int.Parse(generatorArgs[0])));
                    break;
                case "validate":
                    byteStack.Push(Misc.Validate(width, height, tileStack.Pop()));
                    break;
            }
            sWatch.Stop();
        } catch (IndexOutOfRangeException ioore) {
            throw new IndexOutOfRangeException($"Invalid pipeline. Not enough arguments provided to \"{name}\".");
        } catch (FormatException formatE) {
            throw new FormatException($"Invalid pipeline. Cannot interpret {formatE} as valid value to \"{name}\".");
        } catch (OverflowException overflowE) {
            throw new OverflowException($"Invalid pipeline. Value of {overflowE} is too low or too high \"{name}\".");
        } catch (InvalidOperationException iOpE) {
            throw new InvalidOperationException($"Invalid pipeline. No valid input provided to \"{name}\". {iOpE.Message}");
        }

        StringBuilder lineBuilder = new("\t");
        if (verbosity.HasFlag((Verbosity)4)) {
            for (int nests = 0; nests < nesting; nests++) {
                lineBuilder.Append(' ');
            }
        }
        if (verbosity.HasFlag((Verbosity)2)) {
            lineBuilder.Append($"{name} ({String.Join(", ", generatorArgs)})");
        }
        if (verbosity.HasFlag((Verbosity)8)) {
            TimeSpan time = sWatch.Elapsed;
            lineBuilder.Append($": {time.TotalMilliseconds}ms");
        }
        Console.WriteLine(lineBuilder.ToString());

        t = info.returnType;
        idx++;

        if (p < lastP) {
            break;
        }
    }

    return (t, idx);
}
Type? lastType;
try {
    (lastType, _) = Parse(0, null, 0, 0);

    if (lastType is null) {
        Console.Error.WriteLine("Pipeline Error. Empty pipeline.");
        return 1;
    }
} catch (Exception e) {
    Console.Error.WriteLine(e.Message);
    return 1;
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

BackgroundWorker backgroundWorker = new();

if (lastType == typeof(byte))
    backgroundWorker.DoWork += (_, _) => MapDrawer.DrawBitMap(path, byteStack.Pop(), scale, (int)scale);
else
    backgroundWorker.DoWork += (_, _) => MapDrawer.DrawBitMap(path, tileStack.Pop(), scale, (int)scale);

backgroundWorker.RunWorkerAsync();

Console.Write("Drawing");
(int beforeLeft, int beforeTop) = Console.GetCursorPosition();
const int dotTimeout = 500;
const int maxDotsExclusive = 8;
StringBuilder empty = new();
for (int empt = 0; empt < maxDotsExclusive; empt++) {
    empty.Append(' ');
}
for (int dots = 1; backgroundWorker.IsBusy;) {
    if (dots == maxDotsExclusive) {
        Console.SetCursorPosition(beforeLeft, beforeTop);
        Console.WriteLine(empty.ToString());
        Console.SetCursorPosition(beforeLeft, beforeTop);
        dots = 1;
        continue;
    }
    Console.Write(".");
    Thread.Sleep(dotTimeout);
    dots++;
}
Console.SetCursorPosition(0, beforeTop);

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
/// - Floodfill
/// 