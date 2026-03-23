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

        int srcW = grid.GetLength(0);
        int srcH = grid.GetLength(1);

        int w = (int)(srcW * scale);
        int h = (int)(srcH * scale);

        bool drawGrid = gridSize > 1;

        using Bitmap bmp = new(w, h, PixelFormat.Format24bppRgb);

        Rectangle rect = new(0, 0, w, h);
        BitmapData data = bmp.LockBits(rect, ImageLockMode.WriteOnly, bmp.PixelFormat);

        int stride = data.Stride;
        float invScale = 1f / scale;

        unsafe {
            byte* basePtr = (byte*)data.Scan0;

            for (int y = 0; y < h; y++) {
                int gy = (int)(y * invScale);
                byte* row = basePtr + y * stride;

                for (int x = 0; x < w; x++) {
                    int gx = (int)(x * invScale);

                    Color c = s_tileColors[grid[gx, gy]];

                    if (drawGrid && (x % gridSize == gridSize - 1 || y % gridSize == gridSize - 1)) {
                        c = s_gridColor;
                    }

                    int idx = x * 3;
                    row[idx + 0] = c.B;
                    row[idx + 1] = c.G;
                    row[idx + 2] = c.R;
                }
            }
        }

        bmp.UnlockBits(data);
        bmp.Save(path, s_outputFormat);
    }

    public static readonly Color[] RandomColors = new Color[256] {
        ColorTranslator.FromHtml("#000000"), ColorTranslator.FromHtml("#FF0000"), ColorTranslator.FromHtml("#00FFFF"),
        ColorTranslator.FromHtml("#00FF00"), ColorTranslator.FromHtml("#FF00FF"), ColorTranslator.FromHtml("#0000FF"),
        ColorTranslator.FromHtml("#FFFF00"), ColorTranslator.FromHtml("#FF4000"), ColorTranslator.FromHtml("#00BFFF"),
        ColorTranslator.FromHtml("#40FF00"), ColorTranslator.FromHtml("#BF00FF"), ColorTranslator.FromHtml("#0040FF"),
        ColorTranslator.FromHtml("#BFFF00"), ColorTranslator.FromHtml("#FF8000"), ColorTranslator.FromHtml("#0080FF"),
        ColorTranslator.FromHtml("#80FF00"), ColorTranslator.FromHtml("#8000FF"), ColorTranslator.FromHtml("#0000BF"),
        ColorTranslator.FromHtml("#FFBF00"), ColorTranslator.FromHtml("#FFC000"), ColorTranslator.FromHtml("#00FFC0"),
        ColorTranslator.FromHtml("#00C000"), ColorTranslator.FromHtml("#C000FF"), ColorTranslator.FromHtml("#0000C0"),
        ColorTranslator.FromHtml("#C0FF00"), ColorTranslator.FromHtml("#FF6000"), ColorTranslator.FromHtml("#0090FF"),
        ColorTranslator.FromHtml("#60FF00"), ColorTranslator.FromHtml("#9000FF"), ColorTranslator.FromHtml("#0030FF"),
        ColorTranslator.FromHtml("#DFFF00"), ColorTranslator.FromHtml("#FF2000"), ColorTranslator.FromHtml("#00E0FF"),
        ColorTranslator.FromHtml("#20FF00"), ColorTranslator.FromHtml("#E000FF"), ColorTranslator.FromHtml("#0010FF"),
        ColorTranslator.FromHtml("#EFFF00"), ColorTranslator.FromHtml("#FF9F00"), ColorTranslator.FromHtml("#0060FF"),
        ColorTranslator.FromHtml("#9FFF00"), ColorTranslator.FromHtml("#6000FF"), ColorTranslator.FromHtml("#00009F"),
        ColorTranslator.FromHtml("#FFDF00"), ColorTranslator.FromHtml("#FF3F00"), ColorTranslator.FromHtml("#00C0FF"),
        ColorTranslator.FromHtml("#3FFF00"), ColorTranslator.FromHtml("#C000FF"), ColorTranslator.FromHtml("#0020FF"),
        ColorTranslator.FromHtml("#DFFF00"), ColorTranslator.FromHtml("#FF7F00"), ColorTranslator.FromHtml("#0080FF"),
        ColorTranslator.FromHtml("#7FFF00"), ColorTranslator.FromHtml("#8000FF"), ColorTranslator.FromHtml("#00007F"),
        ColorTranslator.FromHtml("#FFFF3F"), ColorTranslator.FromHtml("#FF1F00"), ColorTranslator.FromHtml("#00E0DF"),
        ColorTranslator.FromHtml("#1FFF00"), ColorTranslator.FromHtml("#E000DF"), ColorTranslator.FromHtml("#001FDF"),
        ColorTranslator.FromHtml("#DFE000"), ColorTranslator.FromHtml("#FF5F00"), ColorTranslator.FromHtml("#009FDF"),
        ColorTranslator.FromHtml("#5FFF00"), ColorTranslator.FromHtml("#9F00DF"), ColorTranslator.FromHtml("#003FDF"),
        ColorTranslator.FromHtml("#DF9F00"), ColorTranslator.FromHtml("#FF9F3F"), ColorTranslator.FromHtml("#0060DF"),
        ColorTranslator.FromHtml("#9FFF3F"), ColorTranslator.FromHtml("#6000DF"), ColorTranslator.FromHtml("#003F9F"),
        ColorTranslator.FromHtml("#DFDF00"), ColorTranslator.FromHtml("#FF003F"), ColorTranslator.FromHtml("#00FFDF"),
        ColorTranslator.FromHtml("#00DF3F"), ColorTranslator.FromHtml("#DF00FF"), ColorTranslator.FromHtml("#003F00"),
        ColorTranslator.FromHtml("#DF00DF"), ColorTranslator.FromHtml("#FF7F3F"), ColorTranslator.FromHtml("#0080DF"),
        ColorTranslator.FromHtml("#7FFF3F"), ColorTranslator.FromHtml("#8000DF"), ColorTranslator.FromHtml("#003F7F"),
        ColorTranslator.FromHtml("#DFDF3F"), ColorTranslator.FromHtml("#FF5F3F"), ColorTranslator.FromHtml("#009FDF"),
        ColorTranslator.FromHtml("#5FFF3F"), ColorTranslator.FromHtml("#9F00DF"), ColorTranslator.FromHtml("#003FDF"),
        ColorTranslator.FromHtml("#DF9F3F"), ColorTranslator.FromHtml("#FFBF3F"), ColorTranslator.FromHtml("#0040DF"),
        ColorTranslator.FromHtml("#BFFF3F"), ColorTranslator.FromHtml("#4000DF"), ColorTranslator.FromHtml("#003FBF"),
        ColorTranslator.FromHtml("#DFDF7F"), ColorTranslator.FromHtml("#FF1F3F"), ColorTranslator.FromHtml("#00DFDF"),
        ColorTranslator.FromHtml("#1FDF3F"), ColorTranslator.FromHtml("#DF00DF"), ColorTranslator.FromHtml("#001F3F"),
        ColorTranslator.FromHtml("#DF00BF"), ColorTranslator.FromHtml("#FF3F3F"), ColorTranslator.FromHtml("#00BFFF"),
        ColorTranslator.FromHtml("#3FFF3F"), ColorTranslator.FromHtml("#BF00DF"), ColorTranslator.FromHtml("#003FBF"),
        ColorTranslator.FromHtml("#DFBF3F"), ColorTranslator.FromHtml("#FF7F7F"), ColorTranslator.FromHtml("#0080BF"),
        ColorTranslator.FromHtml("#7FFF7F"), ColorTranslator.FromHtml("#8000BF"), ColorTranslator.FromHtml("#003F7F"),
        ColorTranslator.FromHtml("#BFBF3F"), ColorTranslator.FromHtml("#FF9F7F"), ColorTranslator.FromHtml("#0060BF"),
        ColorTranslator.FromHtml("#9FFF7F"), ColorTranslator.FromHtml("#6000BF"), ColorTranslator.FromHtml("#003F60"),
        ColorTranslator.FromHtml("#BFBF7F"), ColorTranslator.FromHtml("#FFDF7F"), ColorTranslator.FromHtml("#0020BF"),
        ColorTranslator.FromHtml("#DFFF7F"), ColorTranslator.FromHtml("#2000BF"), ColorTranslator.FromHtml("#001F60"),
        ColorTranslator.FromHtml("#BFBFBF"), ColorTranslator.FromHtml("#FF0000"), ColorTranslator.FromHtml("#00FFFF"),
        ColorTranslator.FromHtml("#00FF00"), ColorTranslator.FromHtml("#FF00FF"), ColorTranslator.FromHtml("#0000FF"),
        ColorTranslator.FromHtml("#FFFF00"), ColorTranslator.FromHtml("#FF4000"), ColorTranslator.FromHtml("#00BFFF"),
        ColorTranslator.FromHtml("#40FF00"), ColorTranslator.FromHtml("#BF00FF"), ColorTranslator.FromHtml("#0040FF"),
        ColorTranslator.FromHtml("#BFFF00"), ColorTranslator.FromHtml("#FF8000"), ColorTranslator.FromHtml("#0080FF"),
        ColorTranslator.FromHtml("#80FF00"), ColorTranslator.FromHtml("#8000FF"), ColorTranslator.FromHtml("#0000BF"),
        ColorTranslator.FromHtml("#FFBF00"), ColorTranslator.FromHtml("#FFC000"), ColorTranslator.FromHtml("#00FFC0"),
        ColorTranslator.FromHtml("#00C000"), ColorTranslator.FromHtml("#C000FF"), ColorTranslator.FromHtml("#0000C0"),
        ColorTranslator.FromHtml("#C0FF00"), ColorTranslator.FromHtml("#FF6000"), ColorTranslator.FromHtml("#0090FF"),
        ColorTranslator.FromHtml("#60FF00"), ColorTranslator.FromHtml("#9000FF"), ColorTranslator.FromHtml("#0030FF"),
        ColorTranslator.FromHtml("#DFFF00"), ColorTranslator.FromHtml("#FF2000"), ColorTranslator.FromHtml("#00E0FF"),
        ColorTranslator.FromHtml("#20FF00"), ColorTranslator.FromHtml("#E000FF"), ColorTranslator.FromHtml("#0010FF"),
        ColorTranslator.FromHtml("#EFFF00"), ColorTranslator.FromHtml("#FF9F00"), ColorTranslator.FromHtml("#0060FF"),
        ColorTranslator.FromHtml("#9FFF00"), ColorTranslator.FromHtml("#6000FF"), ColorTranslator.FromHtml("#00009F"),
        ColorTranslator.FromHtml("#FFDF00"), ColorTranslator.FromHtml("#FF3F00"), ColorTranslator.FromHtml("#00C0FF"),
        ColorTranslator.FromHtml("#3FFF00"), ColorTranslator.FromHtml("#C000FF"), ColorTranslator.FromHtml("#0020FF"),
        ColorTranslator.FromHtml("#DFFF00"), ColorTranslator.FromHtml("#FF7F00"), ColorTranslator.FromHtml("#0080FF"),
        ColorTranslator.FromHtml("#7FFF00"), ColorTranslator.FromHtml("#8000FF"), ColorTranslator.FromHtml("#00007F"),
        ColorTranslator.FromHtml("#FFFF3F"), ColorTranslator.FromHtml("#FF1F00"), ColorTranslator.FromHtml("#00E0DF"),
        ColorTranslator.FromHtml("#1FFF00"), ColorTranslator.FromHtml("#E000DF"), ColorTranslator.FromHtml("#001FDF"),
        ColorTranslator.FromHtml("#DFE000"), ColorTranslator.FromHtml("#FF5F00"), ColorTranslator.FromHtml("#009FDF"),
        ColorTranslator.FromHtml("#5FFF00"), ColorTranslator.FromHtml("#9F00DF"), ColorTranslator.FromHtml("#003FDF"),
        ColorTranslator.FromHtml("#DF9F00"), ColorTranslator.FromHtml("#FF9F3F"), ColorTranslator.FromHtml("#0060DF"),
        ColorTranslator.FromHtml("#9FFF3F"), ColorTranslator.FromHtml("#6000DF"), ColorTranslator.FromHtml("#003F9F"),
        ColorTranslator.FromHtml("#DFDF00"), ColorTranslator.FromHtml("#FF003F"), ColorTranslator.FromHtml("#00FFDF"),
        ColorTranslator.FromHtml("#00DF3F"), ColorTranslator.FromHtml("#DF00FF"), ColorTranslator.FromHtml("#003F00"),
        ColorTranslator.FromHtml("#DF00DF"), ColorTranslator.FromHtml("#FF7F3F"), ColorTranslator.FromHtml("#0080DF"),
        ColorTranslator.FromHtml("#7FFF3F"), ColorTranslator.FromHtml("#8000DF"), ColorTranslator.FromHtml("#003F7F"),
        ColorTranslator.FromHtml("#DFDF3F"), ColorTranslator.FromHtml("#FF5F3F"), ColorTranslator.FromHtml("#009FDF"),
        ColorTranslator.FromHtml("#5FFF3F"), ColorTranslator.FromHtml("#9F00DF"), ColorTranslator.FromHtml("#003FDF"),
        ColorTranslator.FromHtml("#DF9F3F"), ColorTranslator.FromHtml("#FFBF3F"), ColorTranslator.FromHtml("#0040DF"),
        ColorTranslator.FromHtml("#BFFF3F"), ColorTranslator.FromHtml("#4000DF"), ColorTranslator.FromHtml("#003FBF"),
        ColorTranslator.FromHtml("#DFDF7F"), ColorTranslator.FromHtml("#FF1F3F"), ColorTranslator.FromHtml("#00DFDF"),
        ColorTranslator.FromHtml("#1FDF3F"), ColorTranslator.FromHtml("#DF00DF"), ColorTranslator.FromHtml("#001F3F"),
        ColorTranslator.FromHtml("#DF00BF"), ColorTranslator.FromHtml("#FF3F3F"), ColorTranslator.FromHtml("#00BFFF"),
        ColorTranslator.FromHtml("#3FFF3F"), ColorTranslator.FromHtml("#BF00DF"), ColorTranslator.FromHtml("#003FBF"),
        ColorTranslator.FromHtml("#DFBF3F"), ColorTranslator.FromHtml("#FF7F7F"), ColorTranslator.FromHtml("#0080BF"),
        ColorTranslator.FromHtml("#7FFF7F"), ColorTranslator.FromHtml("#8000BF"), ColorTranslator.FromHtml("#003F7F"),
        ColorTranslator.FromHtml("#BFBF3F"), ColorTranslator.FromHtml("#FF9F7F"), ColorTranslator.FromHtml("#0060BF"),
        ColorTranslator.FromHtml("#9FFF7F"), ColorTranslator.FromHtml("#6000BF"), ColorTranslator.FromHtml("#003F60"),
        ColorTranslator.FromHtml("#BFBF7F"), ColorTranslator.FromHtml("#FFDF7F"), ColorTranslator.FromHtml("#0020BF"),
        ColorTranslator.FromHtml("#DFFF7F"), ColorTranslator.FromHtml("#2000BF"), ColorTranslator.FromHtml("#001F60"),
        ColorTranslator.FromHtml("#BFBFBF"), ColorTranslator.FromHtml("#FFFFFF"), ColorTranslator.FromHtml("#FF00AA"),
        ColorTranslator.FromHtml("#00FFAA")
    };
    public static void DrawBitMap(string path, byte[,] grid, float scale = 1, int gridSize = 0, bool randomizeColor = false) {
        if (string.IsNullOrEmpty(path))
            throw new ArgumentException($"'{nameof(path)}' cannot be null or empty.", nameof(path));

        int w = (int)(grid.GetLength(0) * scale);
        int h = (int)(grid.GetLength(1) * scale);

        using Bitmap bmp = new(w, h, PixelFormat.Format24bppRgb);

        var rect = new Rectangle(0, 0, w, h);
        var data = bmp.LockBits(rect, ImageLockMode.WriteOnly, bmp.PixelFormat);

        int stride = data.Stride;
        unsafe {
            byte* ptr = (byte*)data.Scan0;

            for (int y = 0; y < h; y++) {
                int gy = (int)(y / scale);
                byte* row = ptr + y * stride;

                for (int x = 0; x < w; x++) {
                    int gx = (int)(x / scale);
                    byte value = grid[gx, gy];

                    byte r, g, b;

                    if (randomizeColor) {
                        var c = RandomColors[value];
                        r = c.R; g = c.G; b = c.B;
                    }
                    else {
                        r = g = b = value;
                    }

                    if (gridSize > 1 && (x % gridSize == gridSize - 1 || y % gridSize == gridSize - 1)) {
                        r = s_gridColor.R;
                        g = s_gridColor.G;
                        b = s_gridColor.B;
                    }

                    row[x * 3 + 0] = b;
                    row[x * 3 + 1] = g;
                    row[x * 3 + 2] = r;
                }
            }
        }

        bmp.UnlockBits(data);

        bmp.Save(path, s_outputFormat);
    }
}
