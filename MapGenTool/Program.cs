using System.CommandLine;
using MapGenTool;
using MapGenTool.Generators;
using MapGenTool.Generators.CellurarAutomata;
using MapGenTool.Generators.ErosionGenerators;
using MapGenTool.Generators.NoiseGenerators;
using MapGenTool.Generators.RoomGenerators;
using System.Diagnostics;

Option<int> widthOption = new(
    "--width", "-w")
{
    Description = "Width of the map",
    Required = true,
    DefaultValueFactory = _ => 120
};

Option<int> heightOption = new(
    "--height", "-h")
{
    Description = "Height of the map",
    Required = true,
    DefaultValueFactory = _ => 30
};

Option<float> scaleOption = new(
    "--scale", "-S")
{
    Description = "Scale factor for the image",
    Required = false,
    DefaultValueFactory = _ => 1f,
};

Option<int> seedOption = new(
    "--seed", "-s")
{
    Description = "Seed for generation",
    Required = false,
    DefaultValueFactory = _ => Random.Shared.Next()
};

Argument<string> pathArgument = new(
    "path")
{
    Description = "Output path for the image"
};

RootCommand rootCommand = new("MapGenTool CLI")
{
    widthOption,
    heightOption,
    scaleOption,
    seedOption,
    pathArgument
};

rootCommand.SetAction(parseResult =>
{
    int width = parseResult.GetValue(widthOption);
    int height= parseResult.GetValue(heightOption);

    float scale= parseResult.GetValue(scaleOption);

    int seed= parseResult.GetValue(seedOption);
    string path= parseResult.GetValue(pathArgument);

    VoronoiNoiseGenerator voronoi = new(10);
    SobelEdgeDetection sobel = new(voronoi.Generate);
    ThresholdClamper clamper = new(sobel.Generate, 0.4f);

    Console.WriteLine("Starting generation...");
    var test = clamper.Generate(width, height, seed);

    string fullPath = Path.GetFullPath(path + ".png");
    MapDrawer.DrawBitMap(fullPath, test, scale, (int)scale);
    Console.WriteLine($"Successfully created map at {fullPath}");

    if (File.Exists(fullPath))
    {
        Process.Start(new ProcessStartInfo(fullPath) { UseShellExecute = true });
    }
    else
    {
        Console.WriteLine("Failed to find the image file.");
    }

}, widthOption, heightOption, scaleOption, seedOption, pathArgument);

return rootCommand.Parse(args).Invoke();
