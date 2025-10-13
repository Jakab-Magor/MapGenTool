using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Drawing.Imaging;

namespace MapGenTool;

internal static class MapDrawer {
    private static readonly ImageFormat s_outputFormat = ImageFormat.Png;
    private static readonly string s_extension = ".png";
    private static readonly Dictionary<Tiles, Color> s_tileColors = new() {
        {Tiles.Space, Color.BlanchedAlmond},
        {Tiles.Wall, Color.Black },
        {(Tiles)2, Color.Blue },
        {(Tiles)3, Color.Green },
        {(Tiles)4, Color.Yellow },
        {(Tiles)5, Color.Red },
    };
    private static readonly Color s_gridColor = Color.Gray;

    public static void DrawBitMap(string path, Tiles[,] grid, float scale = 1, int gridSize = 0) {
        if (string.IsNullOrEmpty(path))
            throw new ArgumentException($"'{nameof(path)}' cannot be null or empty.", nameof(path));

        bool drawGrid = gridSize > 1;

        int scaledHeight = (int)MathF.Floor(grid.GetLength(1) * scale);
        int scaledWidth = (int)MathF.Floor(grid.GetLength(0) * scale);
        Bitmap bmp = new(scaledWidth, scaledHeight);

        for (int y = 0; y < scaledHeight; y++) {
            for (int x = 0; x < scaledWidth; x++) {
                int scaledX = (int)MathF.Floor(x / scale);
                int scaledY = (int)MathF.Floor(y / scale);

                Tiles tile = grid[scaledX, scaledY];
                Color color = s_tileColors[tile];

                if (drawGrid && (x % gridSize == gridSize - 1 || y % gridSize == gridSize - 1)) {
                    color = s_gridColor;
                }

                bmp.SetPixel(x, y, color);
            }
        }

        bmp.Save(path, s_outputFormat);
    }

    public static void DrawBitMap(string path, byte[,] grid, float scale = 1, int gridSize = 0) {
        if (string.IsNullOrEmpty(path))
            throw new ArgumentException($"'{nameof(path)}' cannot be null or empty.", nameof(path));

        bool drawGrid = gridSize > 1;

        int scaledHeight = (int)MathF.Floor(grid.GetLength(1) * scale);
        int scaledWidth = (int)MathF.Floor(grid.GetLength(0) * scale);
        Bitmap bmp = new(scaledWidth, scaledHeight);

        for (int y = 0; y < scaledHeight; y++) {
            for (int x = 0; x < scaledWidth; x++) {
                int scaledX = (int)MathF.Floor(x / scale);
                int scaledY = (int)MathF.Floor(y / scale);

                int tileValue = (int)grid[scaledX, scaledY];
                Color color = Color.FromArgb(tileValue, tileValue, tileValue);

                if (drawGrid && (x % gridSize == gridSize - 1 || y % gridSize == gridSize - 1)) {
                    color = s_gridColor;
                }

                bmp.SetPixel(x, y, color);
            }
        }

        bmp.Save(path, s_outputFormat);
    }
}
