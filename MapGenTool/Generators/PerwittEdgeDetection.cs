namespace MapGenTool.Generators;
public class PerwittEdgeDetection : IGenerator<byte> {
    private static readonly int[,] s_gx = new int[3, 3]
    {
        {1,0,-1 },
        {1,0,-1 },
        {1,0,-1 }
    };
    private static readonly int[,] s_gy = new int[3, 3]
    {
        {1,1,1 },
        {0,0,0 },
        {-1,-1,-1 }
    };

    public byte ArgsCount => 0;

    public bool UsesInput => true;

    public Type InputType => typeof(byte);
    private byte[,] _baseGrid = null!;

    public byte[,] Generate(int width, int height, int seed) {
        byte[,] resultGrid = new byte[width, height];

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                int sumx = GetConvolutedValue(x, y, _baseGrid, s_gx);
                int sumy = GetConvolutedValue(x, y, _baseGrid, s_gy);

                float gradient = MathF.Sqrt(sumx * sumx + sumy * sumy);
                resultGrid[x, y] = (byte)Math.Clamp(gradient, 0, 255);
                //resultGrid[x, y] = (byte)Math.Clamp(sumx+sumy, 0, 255); // very funky but not what we're looking for.
            }
        }

        return resultGrid;
    }

    public void Parse(params string[] args) { }

    public void SetBaseGrid<T>(T[,] basegrid) where T : IConvertible {
        _baseGrid = IGenerator<byte>.CastGrid<T>(basegrid);
    }

    private int GetConvolutedValue(int x, int y, byte[,] baseGrid, int[,] convolutionMatrix) {
        int width = baseGrid.GetLength(0);
        int height = baseGrid.GetLength(1);
        int sum = 0;

        for (int ky = -1; ky <= 1; ky++) {
            for (int kx = -1; kx <= 1; kx++) {
                int ix = Math.Clamp(x + kx, 0, width - 1);
                int iy = Math.Clamp(y + ky, 0, height - 1);

                sum += (int)baseGrid[ix, iy] * convolutionMatrix[ky + 1, kx + 1];
            }
        }

        return sum;
    }
}
