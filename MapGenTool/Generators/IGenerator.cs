namespace MapGenTool.Generators;

public interface IGenerator<out T>
{
    public T[,] Generate(int width, int height, int seed);
}
public interface IGenerator<in TIn, out TOut> : IGenerator<TOut>
{
    public TOut[,] Generate(TIn[,] baseGrid,int width, int height, int seed);
}
