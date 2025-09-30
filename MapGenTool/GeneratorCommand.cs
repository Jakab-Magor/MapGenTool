using MapGenTool.Generators;
using System.Reflection;

namespace MapGenTool;

internal class GeneratorCommand<T>(string name) where T : IGenerator
{
    public string Name { get; init; } = name;
    private ConstructorInfo Constructor
    {
        get
        {
            field ??= typeof(T).GetConstructors().Single();
            return field;
        }
    } = null!;
    public ParameterInfo[] Parameters
    {
        get
        {
            field ??= Constructor.GetParameters();
            return field;
        }
    } = null!;

    public int GetArgsCounts() => Parameters.Length;

    public bool IsInputSupported<TIn>()
    {
        if (typeof(T) is IGenerator<TType>)
            return false;

        if (typeof(T) is IGenerator<TIn, TType>)
            return true;

        return false;
    }
}