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

RootCommand rootCommand = new("MapGenTool");
rootCommand.Options.Add(widthOption);
rootCommand.Options.Add(heightOption);
rootCommand.Options.Add(scaleOption);
rootCommand.Options.Add(seedOption);
rootCommand.Arguments.Add(pathArgument);

var voronoi = new GeneratorCommand<byte>(new("voronoi", "Generates a voronoi noise texture. (byte[*.*])"));
voronoi.Command.Options.Add(
    new Option<int>("--grid-scale", "-G")
    {
        Description = "The scale of the grid in which the seeds are placed."
    });
var sobel = new GeneratorCommand<byte, byte>(new("sobel", "Takes input and runs Sobel edge detection on it. (byte[*.*])"));
var bsp = new GeneratorCommand<Tiles>(new("bsp", "Generates room and corridors in subdivided edges. (Tiles[*.*])"));
bsp.Command.Options.Add(
    new Option<int>("--particion-count, -P")
    {
        Description = "How many partitions the image should be split into. The number of room equals 2 to the power of n.",
        Required = true
    });
var clamper = new GeneratorCommand<byte, Tiles>(new("clamper", "Clamps given byte[*,*] into Tiles[*,*] walls and spaces. (Tiles[*.*])"));
clamper.Command.Options.Add(
    new Option<float>("--threshold", "-T")
    {
        Description = "The threshold at which to place spases insted of walls",
        Required=true,
        DefaultValueFactory = parseResult => 0.5f
    });
var conway = new GeneratorCommand<Tiles, Tiles>(new("conway", "Runs Conway's game of life on give Tiles[*,*] where spaces are alive cells. (Tiles[*.*])"));
conway.Command.Options.Add(
    new Option<int>("--iterations", "-I")
    {
        Description = "The number of iterations the simulation should run for before output.",
        Required = true,
    });
//var drunkardsWalk = new GeneratorCommand<Tiles, Tiles>(new("drunkards", "Runs drunkards walk algorythm on given image. Drunkard can start on any space cells. (Tiles[*.*])"));
var simpleNoise = new GeneratorCommand<Tiles>(new("simple-nosie", "Gets a simple randomized noise. (Tiles[*.*])"));

IGeneratorCommand[] generatorCommands = [
    voronoi,
    sobel,
    bsp,
    clamper,
    conway,
    //drunkardsWalk,
    simpleNoise
];
foreach (var generatorCommand in generatorCommands)
{
    rootCommand.Subcommands.Add(generatorCommand.Command);

    var supportedCommands = generatorCommands
        .Where(
            c => c.IsInputSupported(
                generatorCommand.GetType().GenericTypeArguments.Last()));
    foreach (var c in supportedCommands)
    {
        generatorCommand.Command.Subcommands.Add(c);
    }
}


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

List<>
foreach (var generatorCommand in generatorCommands)
{

}

Console.WriteLine("Starting generation...");
//var test = clamper.Generate(width, height, seed);

MapDrawer.DrawBitMap(path, test, scale, (int)scale);
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
