using System.CommandLine;

namespace MapGenTool;

internal interface IGeneratorCommand
{
    public Command Command { get; set; }
    public bool IsInputSupported(Type type);
}
internal class GeneratorCommand<TOut>(Command command) :IGeneratorCommand
{
    public Command Command { get; set; } = command;

    public bool IsInputSupported(Type type) => false;
}
internal class GeneratorCommand<TIn,TOut>(Command command) : IGeneratorCommand
{
    public Command Command { get; set; } = command;

    public bool IsInputSupported(Type type) => type == typeof(TIn);
}
internal class GeneratorCommand<TIn1,TIn2, TOut>(Command command) : IGeneratorCommand
{
    public Command Command { get; set; } = command;

    public bool IsInputSupported(Type type)
    {
        return
            type == typeof(TIn1) ||
            type == typeof(TIn2);
    }
}
