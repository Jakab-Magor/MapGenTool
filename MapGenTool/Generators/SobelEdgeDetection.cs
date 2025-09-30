namespace MapGenTool.Generators;

public class SobelEdgeDetection : IGenerator<byte,byte>
{

    private static readonly int[,] s_gx = new int[3, 3]
    {
        {-1,0,1 },
        {-2,0,2 },
        {-1,0,1 }
    };
    private static readonly int[,] s_gy = new int[3, 3]
    {
        {-1,-2,-1 },
        {0,0,0 },
        {1,2,1 }
    };

    public byte[,] Generate(byte[,] baseGrid,int width, int height, int seed)
    {
        byte[,] resultGrid = new byte[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int sumx = GetConvolutedValue(x, y, baseGrid, s_gx);
                int sumy = GetConvolutedValue(x, y, baseGrid, s_gy);

                float gradient = MathF.Sqrt(sumx * sumx + sumy * sumy);
                resultGrid[x, y] = (byte)Math.Clamp(gradient, 0, 255);
                //resultGrid[x, y] = (byte)Math.Clamp(sumx+sumy, 0, 255); // very funky but not what we're looking for.
            }
        }

        return resultGrid;
    }

    private int GetConvolutedValue(int x, int y, byte[,] baseGrid, int[,] convolutionMatrix)
    {
        int width = baseGrid.GetLength(0);
        int height = baseGrid.GetLength(1);
        int sum = 0;

        for (int ky = -1; ky <= 1; ky++)
        {
            for (int kx = -1; kx <= 1; kx++)
            {
                int ix = Math.Clamp(x + kx, 0, width - 1);
                int iy = Math.Clamp(y + ky, 0, height - 1);

                sum += (int)baseGrid[ix, iy] * convolutionMatrix[ky + 1, kx + 1];
            }
        }

        return sum;
    }
}
