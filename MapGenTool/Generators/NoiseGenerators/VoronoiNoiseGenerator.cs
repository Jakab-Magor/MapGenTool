using MapGenTool.Generic;

namespace MapGenTool.Generators;

public static partial class Noise {
    public static byte[,] Voronoi(int width, int height, int seed, int chunkSize) {
        byte[,] tiles = new byte[width, height];
        Random rng = new(seed);

        int seedsWidth = (int)Math.Ceiling(width / (float)chunkSize);
        int seedsHeight = (int)Math.Ceiling(height / (float)chunkSize);

        IntVector2[,] seeds = new IntVector2[seedsWidth, seedsHeight];
        for (int x = 0; x < seedsWidth; x++) {
            for (int y = 0; y < seedsHeight; y++) {
                IntVector2 relativePositionInChunk = new(rng.Next(0, chunkSize), rng.Next(0, chunkSize));
                seeds[x, y] = relativePositionInChunk;
            }
        }
        float maxDist = new IntVector2(chunkSize, chunkSize).Magnitude2;

        for (int y = 0, yInChunk = 0, chunkY = 0; y < height; y++, yInChunk++) {
            if (yInChunk >= chunkSize) {
                yInChunk = 0;
                chunkY++;
            }

            for (int x = 0, xInChunk = 0, chunkX = 0; x < width; x++, xInChunk++) {
                if (xInChunk >= chunkSize) {
                    xInChunk = 0;
                    chunkX++;
                }

                float minDist2 = float.MaxValue;
                for (int i = -1; i <= 1; i++) {
                    for (int j = -1; j <= 1; j++) {
                        int l = chunkX + i;
                        int m = chunkY + j;
                        if (l < 0 || m < 0 || l >= seedsWidth || m >= seedsHeight)
                            continue;

                        IntVector2 seedPos = seeds[l, m];
                        seedPos += new IntVector2(i, j) * chunkSize;
                        IntVector2 tilePosInChunk = new(xInChunk, yInChunk);

                        IntVector2 differenceVector = (tilePosInChunk - seedPos);
                        float distance2 = differenceVector.Magnitude2;

                        if (distance2 < minDist2)
                            minDist2 = distance2;
                    }
                }

                float normalisedDistance = minDist2 / maxDist;

                tiles[x, y] = (byte)(normalisedDistance * 255);
            }
        }

        return tiles;
    }
}
