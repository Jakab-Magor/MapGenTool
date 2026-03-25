using MapGenTool.Generic;
using System.Numerics;

namespace MapGenTool.Generators;

public static partial class Noise {
    public static byte[,] Perlin2D(int width, int height, int seed, int size) {
        byte[,] result = new byte[width, height];
        Random rng = new(seed);

        int gridWidth = (int)Math.Ceiling(width / (float)size);
        int gridHeight = (int)Math.Ceiling(height / (float)size);
        Vector2[,] grads = new Vector2[gridWidth, gridHeight];

        // Generate gradients
        /*for (int gY = 0; gY < gridHeight; gY++) {
            for (int gX = 0; gX < gridWidth; gX++) {
                var randVector = new Vector2(
                    x: rng.NextSingle() * rng.Next(2) == 1 ? 1 : -1,
                    y: rng.NextSingle() * rng.Next(2) == 1 ? 1 : -1);
                grads[gX, gY] = randVector;
            }
        }*/

        // Generate random unit gradients
        for (int gx = 0; gx < gridWidth; gx++) {
            for (int gy = 0; gy < gridHeight; gy++) {
                double angle = rng.NextDouble() * Math.PI * 2;
                grads[gx, gy] = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
            }
        }

        // Fade function for smooth interpolation
        float Fade(float t) => t * t * t * (t * (t * 6 - 15) + 10);

        // Linear interpolation
        float Lerp(float a, float b, float t) => a + t * (b - a);

        // Dot product helper
        float Dot(Vector2 g, float x, float y) => g.X * x + g.Y * y;

        /*
           gy0          gy0          gy1          gy1
           ^            ^            ^            ^
           |            |            |            |
           |  (gx0,gy0) *------------* (gx1,gy0)  |
           |            |            |            |
           |            |            |            |
           |            |   (lx,ly)  |            |
           |            |     x,y    |            |
           |            |            |            |
           |  (gx0,gy1) *------------* (gx1,gy1)  |
           |            |            |            |
           +---------------------------------------------> gx
         */

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {

                // Grid cell coordinates
                int gx0 = x / size;
                int gy0 = y / size;
                int gx1 = gx0 + 1;
                int gy1 = gy0 + 1;

                // Clamp to gradient grid
                if (gx1 >= gridWidth) gx1 = gridWidth - 1;
                if (gy1 >= gridHeight) gy1 = gridHeight - 1;

                // Local position inside the cell
                float lx = (x % size) / (float)size;
                float ly = (y % size) / (float)size;

                // Fade curves
                float fx = Fade(lx);
                float fy = Fade(ly);

                // Distance vectors to corners
                float dx0 = lx;
                float dy0 = ly;
                float dx1 = lx - 1;
                float dy1 = ly - 1;

                // Dot products with corner gradients
                float n00 = Dot(grads[gx0, gy0], dx0, dy0);
                float n10 = Dot(grads[gx1, gy0], dx1, dy0);
                float n01 = Dot(grads[gx0, gy1], dx0, dy1);
                float n11 = Dot(grads[gx1, gy1], dx1, dy1);

                // Interpolate horizontally
                float ix0 = Lerp(n00, n10, fx);
                float ix1 = Lerp(n01, n11, fx);

                // Interpolate vertically
                float value = Lerp(ix0, ix1, fy);

                // Normalize from [-1,1] to [0,255]
                value = (value + 1f) * 0.5f;
                value = Math.Clamp(value, 0f, 1f);

                result[x, y] = (byte)(value * 255);
            }
        }

        return result;
    }
}