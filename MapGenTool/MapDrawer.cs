using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Drawing.Imaging;

namespace MapGenTool;

internal static class MapDrawer
{
    private static readonly ImageFormat s_outputFormat = ImageFormat.Png;
    private static readonly string s_extension = ".png";
    private static readonly Dictionary<TileTypes, Color> s_tileColors = new Dictionary<TileTypes, Color>
    {
        {TileTypes.Space, Color.BlanchedAlmond},
        {TileTypes.Wall, Color.Black }
    };

    public static string DrawBitMap(
        string path,
        string fileName,
        TileGrid grid,
        [Range(minimum: 1, maximum: float.MaxValue, MinimumIsExclusive = false)] float scale = 1)
    {
        if (string.IsNullOrEmpty(path))
            throw new ArgumentException($"'{nameof(path)}' cannot be null or empty.", nameof(path));

        int scaledHeight = IntFloor(grid.Height * scale);
        int scaledWidth = IntFloor(grid.Width * scale);
        Bitmap bmp = new(scaledWidth, scaledHeight);

        for (int y = 0; y < scaledHeight; y++)
        {
            for (int x = 0; x < scaledWidth; x++)
            {
                int scaledX = IntFloor(x / scale);
                int scaledY = IntFloor(y / scale);

                TileTypes tile = grid[scaledX, scaledY];
                Color color = s_tileColors[tile];

                //int tileValue = (int)grid[scaledX, scaledY];
                //Color color = Color.FromArgb(tileValue, tileValue, tileValue);

                bmp.SetPixel(x, y, color);
            }
        }

        string fullPath = Path.Combine(path, fileName) + s_extension;
        bmp.Save(fullPath, s_outputFormat);
        return fullPath;
    }

    public static string DrawBitMap(
        string path,
        string fileName,
        byte[,] grid,
        [Range(minimum: 1, maximum: float.MaxValue, MinimumIsExclusive = false)] float scale = 1)
    {
        if (string.IsNullOrEmpty(path))
            throw new ArgumentException($"'{nameof(path)}' cannot be null or empty.", nameof(path));

        int scaledHeight = IntFloor(grid.GetLength(1) * scale);
        int scaledWidth = IntFloor(grid.GetLength(0) * scale);
        Bitmap bmp = new(scaledWidth, scaledHeight);

        for (int y = 0; y < scaledHeight; y++)
        {
            for (int x = 0; x < scaledWidth; x++)
            {
                int scaledX = IntFloor(x / scale);
                int scaledY = IntFloor(y / scale);

                int tileValue = (int)grid[scaledX, scaledY];
                Color color = Color.FromArgb(tileValue, tileValue, tileValue);

                bmp.SetPixel(x, y, color);
            }
        }

        string fullPath = Path.Combine(path, fileName) + s_extension;
        bmp.Save(fullPath, s_outputFormat);
        return fullPath;
    }

    private static int IntFloor(float a) => (int)MathF.Floor(a);
}
