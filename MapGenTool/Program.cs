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
Option<bool> randomizeColorOption = new("--random-color", "-r") {
    Description = "Randomize colors on grayscale images",
    Required = false,
    DefaultValueFactory = parseResult => false
};
// unimplemented due to needing a rewrite of whole printing
// would ideally print everything regardless of verbosity
/*Option<bool> generateLogOption = new("--log") {
    Description = "Output full verbosity text to log with same name as file",
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

RootCommand rootCommand = new("MapGenTool is a mapgenerator which generates a png image to the specified path with parameters. MapGenTool uses a pipeline to determine the order of operations. The pipeline works much like an image generator. To show all generators and their parameters use the \"--help\" or \"-h\" arguments.");
rootCommand.Options.Add(widthOption);
rootCommand.Options.Add(heightOption);
rootCommand.Options.Add(scaleOption);
rootCommand.Options.Add(seedOption);
rootCommand.Options.Add(randomizeColorOption);
//rootCommand.Options.Add(generateLogOption);
rootCommand.Options.Add(verbosityOption);
rootCommand.Options.Add(displayImageOption);
rootCommand.Arguments.Add(pathArgument);
rootCommand.Arguments.Add(pipelineArgument);


/// ----------------------------------------------
/// Generator instantiation for pipeline parsing
/// ----------------------------------------------
Dictionary<string, GeneratorInfo> tokens = new(){
    { "voronoi",            new (GeneratorTypes.First, inputTypes: [], returnType: typeof(byte), "Voronoi noise", "size")},
    { "sobel",              new (GeneratorTypes.Follower, inputTypes: [typeof(byte)], returnType: typeof(byte), "Sobel edge detection algorythm")},
    { "perwitt",            new (GeneratorTypes.Follower, inputTypes: [typeof(byte)], returnType: typeof(byte), "Perwitt edge detection algorythm")},
    { "bsp",                new (GeneratorTypes.First, inputTypes: [], returnType: typeof(Tiles), "Binary space partitioned rooms", "partition_count")},
    { "treshold-clamper",   new (GeneratorTypes.Follower, inputTypes: [typeof(byte)], returnType: typeof(Tiles), "Seperates grayscale into tiles along treshold", "treshold (0-1)")},
    { "conways",            new (GeneratorTypes.Follower, inputTypes: [typeof(Tiles)], returnType: typeof(Tiles), "Conways game of life simulation", "iterations")},
    { "drunkards-walk",     new (GeneratorTypes.Follower, inputTypes: [typeof(Tiles)], returnType: typeof(Tiles), "Drunkard's walk erosion simulation", "agent", "iterations", "step_size")},
    { "simple-noise",       new (GeneratorTypes.First, inputTypes: [], returnType: typeof(byte), "Basic white noise")},
    { "basic-rooms",        new (GeneratorTypes.First, inputTypes: [], returnType: typeof(Tiles), "Generates rooms discards any overlapping and tries again.", "room_count", "room_min_size", "room_max_size")},
    { "inverter",           new (GeneratorTypes.Follower, inputTypes: [typeof(Tiles)], returnType: typeof(Tiles), "Inverts space and wall tiles")},
    { "byte-inverter",      new (GeneratorTypes.Follower, inputTypes: [typeof(byte)], returnType: typeof(byte), "Inverts grayscale values")},
    { "overlap-rooms",      new (GeneratorTypes.First, inputTypes: [], returnType: typeof(Tiles), "Generates overlapping rooms. Any room fully inside others discarded and done again", "room_count", "room_min_size", "room_max_size")},
    { "prefab-pattern",     new (GeneratorTypes.First, inputTypes: [], returnType: typeof(Tiles), "Uses prefab defined pattern. Repeats pattern.", "prefab_path")},
    { "checkerboard",       new (GeneratorTypes.First, inputTypes: [], returnType: typeof(byte), "Grayscale checkerboard with given light and dark values", "dark_shade (0-255)", "light_shade (0-255)")},
    { "perlin",             new (GeneratorTypes.First, inputTypes: [], returnType: typeof(byte), "Perlin blue noise", "size")},
    { "connect",            new (GeneratorTypes.Follower, inputTypes: [typeof(Tiles)], returnType: typeof(Tiles), "Cull any volumes smaller than treshold. Connect the rest to the closest volume", "culling_treshold")},
    { "prefab-room",        new (GeneratorTypes.First, inputTypes: [], returnType: typeof(Tiles), "Places prefabs like rooms. If overlap discard and try again.", "room_count", "prefab_path")},
    { "*",                  new (GeneratorTypes.Binary, inputTypes: [typeof(byte), typeof(Tiles)], returnType: null, "Multiply two byte maps OR get intersection of two tile maps")},
    { "+",                  new (GeneratorTypes.Binary, inputTypes: [typeof(byte), typeof(Tiles)], returnType: null, "Add two byte maps OR get union of two tile maps")},
    { "-",                  new (GeneratorTypes.Binary, inputTypes: [typeof(byte), typeof(Tiles)], returnType: null, "Subtract two byte maps OR give left except right tile maps")},
    { "/",                  new (GeneratorTypes.Binary, inputTypes: [typeof(byte)], returnType: typeof(byte), "Divide two byte maps")},
    { "expand",             new (GeneratorTypes.Follower, inputTypes: [typeof(Tiles)], returnType: typeof(Tiles), "Blobifying diffusion slow and angular.", "iterations") },
    { "blobify",            new (GeneratorTypes.Follower, inputTypes: [typeof(Tiles)], returnType: typeof(Tiles), "Blobifying diffusion fast and circular.", "blob_radius") },
    { "reverse-clamper",    new (GeneratorTypes.Follower, inputTypes: [typeof(Tiles)], returnType: typeof(byte), "Converts tiles to bytes with low and high values for wall and space", "low (0-255)", "high (0-255)") },
    { "bel-zab",            new (GeneratorTypes.Follower, inputTypes: [typeof(byte)], returnType: typeof(byte), "The Belousov Zhabotinsky Reaction as a celurar automata as proposed by A.K.Dewdney [WARNING MIGHT CRASH WITH BAD ARGS]", "iterations" , "k1 (0-255)", "k2 (0-255)", "ill-state  (0-255)", "g  (0-255)") },
};

/// ----------------------------------------------
/// Parsing
/// ----------------------------------------------
ParseResult results = rootCommand.Parse(args);

// ugly solution to help but I don't care
if (results.Tokens.Any(t => t.Value == "--help" || t.Value == "-h")) {
    rootCommand.Parse("-h").Invoke();
    Console.WriteLine("Generator usage:");
    Console.WriteLine("  Start a pipeline with 'first' type generators. Followers can only be put after.");
    Console.WriteLine("    first:                    follower:                ");
    Console.WriteLine("    [generator name] <params> [generator name] <params>");
    Console.WriteLine();
    Console.WriteLine("  Binary generators can be use by putting a pipeline on each side of it.");
    Console.WriteLine("               binary:                             ");
    Console.WriteLine("    [pipeline] [generator name] <params> [pipeline]");
    Console.WriteLine();
    Console.WriteLine("Precedence:  If the order of generators needs to be changed use round brackets '(' and ')', otherwise the order is the following:");
    Console.WriteLine("  1.      2.         4.       3.      5.        ");
    Console.WriteLine("  [first] [follower] [binary] [first] [follower]");
    Console.WriteLine();
    Console.WriteLine("  1.      2.         5.         3.      4.          ");
    Console.WriteLine("  [first] [follower] [binary] ( [first] [follower] )");
    Console.WriteLine();
    Console.WriteLine("Generators: ");
    foreach (var token in tokens) {
        Console.WriteLine($"  {token.Key,-34}{token.Value.shortDescription,-60}");
        Console.WriteLine($"{"",-38}Usage: {token.Value.generatorType.ToString().ToLower()}");
        switch (token.Value.inputTypes.Length) {
            case 0:
                break;
            case 1:
                Console.WriteLine($"{"",-38}Takes: {token.Value.inputTypes[0].Name.ToLower()}");
                break;
            default:
                Console.WriteLine($"{"",-38}Takes: {String.Join(", ", token.Value.inputTypes.Select(i => i.Name.ToLower()))}");
                break;
        }
        if (token.Value.returnType is null) {
            Console.WriteLine($"{"",-38}Returns: input type");
        } else {
            Console.WriteLine($"{"",-38}Returns: {token.Value.returnType.Name.ToLower()}");
        }
        Console.Write($"{"",-38}Parameters: ");
        var prms = token.Value.parameters;
        if (prms.Length == 0) {
            Console.WriteLine('-');
        } else {
            Console.WriteLine(String.Join(", ", prms));
        }
        Console.WriteLine();
    }
    return 2;
}

if (results.Errors.Any() || results.GetValue(pathArgument) is not FileInfo fileInfo) {
    foreach (var error in results.Errors)
        Console.Error.WriteLine(error.Message);

    rootCommand.Parse("-h").Invoke();

    return 2;
}

int width = results.GetValue(widthOption);
int height = results.GetValue(heightOption);

float scale = results.GetValue(scaleOption);
bool randomizeColor = results.GetValue(randomizeColorOption);
//bool generateLog = results.GetValue(generateLogOption);
Verbosity verbosity = results.GetValue(verbosityOption);
bool displayImageInExplorer = results.GetValue(displayImageOption);

int seed = results.GetValue(seedOption);

/// ----------------------------------------------
/// Pipeline parsing
/// ----------------------------------------------
Console.WriteLine("Starting generation...");
List<string> rawPipeline = [.. results.GetValue(pipelineArgument) ?? []];
for (int i = 0; i < rawPipeline.Count; i++) {
    var token = rawPipeline[i];
    for (int j = 0; j < token.Length; j++) {
        if (token[j] != '(' && token[j] != ')') {
            continue;
        }

        List<string> splitTokens = new(3);
        if (j > 0) {
            splitTokens.Add(token.Substring(0, j ));
        }
        splitTokens.Add(token[j].ToString());
        if (j < token.Length - 1) {
            splitTokens.Add(token.Substring(j + 1, token.Length - j - 1));
        }

        rawPipeline.RemoveAt(i);
        rawPipeline.InsertRange(i, splitTokens);
        break;
    }
}
string[] pipeline = [.. rawPipeline];
Stack<byte[,]> byteStack = new();
Stack<Tiles[,]> tileStack = new();

Type? lastType;
#if DEBUG
(lastType, _) = Parse(0, null, 0, 0);

if (lastType is null) {
    Console.Error.WriteLine("Pipeline Error. Empty pipeline.");
    return 1;
}
#else
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
#endif

(Type? returnType, int nextIdx) Parse(int idx, Type? inT, int inP, int nesting) {
    Type? t = null;
    int p = 0;
    while (idx < pipeline.Length) {
        ref string name = ref pipeline[idx];

        if (name == "(") {
            (t, int n) = Parse(idx + 1, t, 0, nesting + 1);
            idx = n - 1;
            continue;
        }
        if (name == ")") {
            return (t, idx + 1);
        }

        if (!tokens.TryGetValue(name, out GeneratorInfo? info)) {
            throw new ArgumentException($"Pipeline parsing error. Unknown generator {name}.");
        }

        int paramOffset = info.parameters.Length;
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

        StringBuilder lineBuilder = new("\t");
        if (verbosity.HasFlag((Verbosity)4)) {
            for (int nests = 0; nests < nesting; nests++) {
                lineBuilder.Append(' ');
            }
        }
        if (verbosity.HasFlag((Verbosity)2)) {
            lineBuilder.Append($"{name} ({String.Join(", ", generatorArgs)})");
        }
        if (verbosity.HasFlag((Verbosity)8) || verbosity.HasFlag((Verbosity)16)) {
            lineBuilder.Append(": ");
        }
        Console.Write(lineBuilder.ToString());
        lineBuilder.Clear();

        TextWriter stdOut = Console.Out;
        (int colonX, int colonY) = Console.GetCursorPosition();
        if (!verbosity.HasFlag((Verbosity)16)) {
            Console.WriteLine();
            Console.SetOut(TextWriter.Null);
        } else {
            Console.WriteLine();
            Console.SetOut(new GeneratorLoggerWriter(stdOut, "\t\t"));
        }

        Stopwatch sWatch = new();
        t = info.returnType ?? t;

        try {
            sWatch.Start();
            /// Include argument name for clarity
            switch (name) {
                case "voronoi":
                    byteStack.Push(Noise.Voronoi(width, height, seed,
                        chunkSize: int.Parse(generatorArgs[0])));
                    break;
                case "sobel":
                    byteStack.Push(EdgeDetection.Sobel(width, height, seed, byteStack.Pop()));
                    break;
                case "perwitt":
                    byteStack.Push(EdgeDetection.Prewitt(width, height, seed, byteStack.Pop()));
                    break;
                case "bsp":
                    tileStack.Push(Rooms.BSPRooms(width, height, seed,
                        halfPartitionCount: int.Parse(generatorArgs[0])));
                    break;
                case "treshold-clamper":
                    tileStack.Push(Misc.ThresholdClamper(width, height, seed, byteStack.Pop(),
                        threshold: float.Parse(generatorArgs[0], CultureInfo.InvariantCulture)));
                    break;
                case "conways":
                    tileStack.Push(CellurarAutomata.ConwaysGameOfLife(width, height, tileStack.Pop(),
                        iterations: int.Parse(generatorArgs[0])));
                    break;
                case "drunkards-walk":
                    tileStack.Push(Erosion.DrunkardsWalk(width, height, seed, tileStack.Pop(),
                        agents: int.Parse(generatorArgs[0]),
                        iterations: int.Parse(generatorArgs[1]),
                        steps: int.Parse(generatorArgs[2])));
                    break;
                case "simple-noise":
                    byteStack.Push(Noise.WhiteNoise(width, height, seed));
                    break;
                case "basic-rooms":
                    tileStack.Push(Rooms.BasicRooms(width, height, seed,
                        roomsCount: int.Parse(generatorArgs[0]),
                        minSize: int.Parse(generatorArgs[1]),
                        maxSize: int.Parse(generatorArgs[2])));
                    break;
                case "inverter":
                    tileStack.Push(Misc.Inverter(width, height, seed, tileStack.Pop()));
                    break;
                case "byte-inverter":
                    byteStack.Push(Misc.ByteInverter(width, height, seed, byteStack.Pop()));
                    break;
                case "overlap-rooms":
                    tileStack.Push(Rooms.OverlapRooms(width, height, seed,
                        roomCount: int.Parse(generatorArgs[0]),
                        minSize: int.Parse(generatorArgs[1]),
                        maxSize: int.Parse(generatorArgs[2])));
                    break;
                case "prefab-pattern":
                    tileStack.Push(Patterns.PrefabPattern(width, height, seed,
                        pathString: generatorArgs[0]));
                    break;
                case "checkerboard":
                    byteStack.Push(Patterns.Checkerboard(width, height, seed,
                        dark: byte.Parse(generatorArgs[0]),
                        light: byte.Parse(generatorArgs[1])));
                    break;
                case "perlin":
                    byteStack.Push(Noise.Perlin2D(width, height, seed,
                        size: int.Parse(generatorArgs[0])));
                    break;
                case "connect":
                    tileStack.Push(Misc.Connector(width, height, tileStack.Pop(),
                        cullingTreshold: int.Parse(generatorArgs[0])));
                    break;
                case "+":
                    if (t == typeof(byte)) {
                        byteStack.Push(Misc.Add(width, height, byteStack.Pop(), byteStack.Pop()));
                        t = typeof(byte);
                    } else {
                        tileStack.Push(Misc.Union(width, height, tileStack.Pop(), tileStack.Pop()));
                        t = typeof(Tiles);
                    }
                    break;
                case "*":
                    if (t == typeof(byte)) {
                        byteStack.Push(Misc.Multiply(width, height, byteStack.Pop(), byteStack.Pop()));
                        t = typeof(byte);
                    } else {
                        tileStack.Push(Misc.Intersect(width, height, tileStack.Pop(), tileStack.Pop()));
                        t = typeof(Tiles);
                    }
                    break;
                case "-":
                    if (t == typeof(byte)) {
                        byteStack.Push(Misc.Subtract(width, height, byteStack.Pop(), byteStack.Pop()));
                        t = typeof(byte);
                    } else {
                        tileStack.Push(Misc.Except(width, height, tileStack.Pop(), tileStack.Pop()));
                        t = typeof(Tiles);
                    }
                    break;
                case "/":
                    byteStack.Push(Misc.Divide(width, height, byteStack.Pop(), byteStack.Pop()));
                    break;
                case "prefab-room":
                    tileStack.Push(Rooms.PrefabRooms(width, height, seed,
                        roomsCount: int.Parse(generatorArgs[0]),
                        pathString: generatorArgs[1]));
                    break;
                case "expand":
                    tileStack.Push(Erosion.Expand(width, height, tileStack.Pop(),
                        iterations: int.Parse(generatorArgs[0])));
                    break;
                case "blobify":
                    tileStack.Push(Erosion.Blobify(width, height, tileStack.Pop(),
                        radius: int.Parse(generatorArgs[0])));
                    break;
                case "reverse-clamper":
                    byteStack.Push(Misc.TilesToGrayscale(width, height, tileStack.Pop(),
                        low: byte.Parse(generatorArgs[0]),
                        high: byte.Parse(generatorArgs[1])));
                    break;
                case "bel-zab":
                    byteStack.Push(CellurarAutomata.BelousovZhabotinskyReaction(width, height, byteStack.Pop(),
                        ));
                    break;
            }
            sWatch.Stop();
        }
#if !DEBUG
        catch (IndexOutOfRangeException) {
            throw new IndexOutOfRangeException($"Invalid pipeline. Not enough arguments provided to \"{name}\".");
        }
        catch (FormatException formatE) {
            throw new FormatException($"Invalid pipeline. Cannot interpret {formatE} as valid value to \"{name}\".");
        }
        catch (OverflowException overflowE) {
            throw new OverflowException($"Invalid pipeline. Value of {overflowE} is too low or too high \"{name}\".");
        }
        catch (InvalidOperationException iOpE) {
            throw new InvalidOperationException($"Invalid pipeline. No valid input provided to \"{name}\". {iOpE.Message}");
        } 
#endif
        catch {
            throw;
        }


        Console.SetOut(stdOut);
        if (verbosity.HasFlag((Verbosity)8)) {
            (int beforeX, int beforeY) = Console.GetCursorPosition();
            Console.SetCursorPosition(colonX, colonY);
            TimeSpan time = sWatch.Elapsed;
            lineBuilder.Append($"{time.TotalMilliseconds}ms");
            Console.WriteLine(lineBuilder.ToString());
            Console.SetCursorPosition(beforeX, beforeY);
        }
        idx++;

        if (p < inP) {
            break;
        }
    }

    return (t, idx);
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
    backgroundWorker.DoWork += (_, _) => MapDrawer.DrawBitMap(path, byteStack.Pop(), scale, (int)scale, randomizeColor);
else
    backgroundWorker.DoWork += (_, _) => MapDrawer.DrawBitMap(path, tileStack.Pop(), scale, (int)scale);

Stopwatch drawStopWatch = new();
drawStopWatch.Start();
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
drawStopWatch.Stop();
Console.SetCursorPosition(0, beforeTop);

if (verbosity.HasFlag(Verbosity.Finished)) {
    TimeSpan drawTime = drawStopWatch.Elapsed;
    Console.WriteLine($"Successfully created map at {path} in {drawTime:ss\\.ffff}s");
}

if (!displayImageInExplorer)
    return 0;

Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
return 0;
