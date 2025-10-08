namespace MapGenTool.Generators;

public interface IGenerator
{
    public abstract byte ArgsCount { get; }
    public abstract bool UsesInput { get; }
    public Type InputType { get; }
    public abstract void Parse(params string[] args);
    public void SetBaseGrid<T>(T[,] basegrid) where T: IConvertible;
}
public interface IGenerator<TType> : IGenerator
{
    public TType[,] Generate(int width, int height, int seed);
    protected static TType[,] CastGrid<TIn>(TIn[,] a) where TIn : IConvertible
    {
        int w = a.GetLength(0);
        int h = a.GetLength(1);
        TType[,] r = new TType[w, h];
        Type t = typeof(TType);
        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                r[x, y] = (TType)Convert.ChangeType(a[x, y], t);
            }
        }

        return r;
    }
}
