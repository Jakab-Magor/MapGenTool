namespace MapGenTool.Generators;

public class ByteInverter : IGenerator<byte> {
    public byte ArgsCount => 0;

    public bool UsesInput => true;

    public Type InputType => typeof(byte);
    private byte[,] _baseGrid = null!;

    public byte[,] Generate(int width, int height, int seed) {
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                _baseGrid[x, y] = (byte)(255 - _baseGrid[x, y]);
            }
        }

        return _baseGrid;
    }

    public void Parse(params string[] args) {
    }

    public void SetBaseGrid<T>(T[,] basegrid) where T : IConvertible {
        _baseGrid = IGenerator<byte>.CastGrid<T>(basegrid);
    }
}
