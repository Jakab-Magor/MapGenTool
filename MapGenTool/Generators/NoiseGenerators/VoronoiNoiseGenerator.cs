using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MapGenTool.Generic;

namespace MapGenTool.Generators.NoiseGenerators;

public class VoronoiNoiseGenerator(int tilesPerChunk) : IGrayscaleGenerator
{
    [Range(minimum: 1, maximum: int.MaxValue, MinimumIsExclusive = true)]
    public int TilesPerChunk { get; set; } = tilesPerChunk;

    public byte[,] Generate(int width, int height, int seed)
    {
        byte[,] tiles = new byte[width, height];
        Random rng = new(seed);

        int seedsWidth = (int)Math.Ceiling(width / (float)TilesPerChunk);
        int seedsHeight = (int)Math.Ceiling(height / (float)TilesPerChunk);

        IntVector2[,] seeds = new IntVector2[seedsWidth, seedsHeight];
        for (int x = 0; x < seedsWidth; x++)
            for (int y = 0; y < seedsHeight; y++)
            {
                IntVector2 relativePositionInChunk = new(rng.Next(0, TilesPerChunk), rng.Next(0, TilesPerChunk));
                seeds[x, y] = relativePositionInChunk;
            }

        for (int y = 0, yInChunk = 0, chunkY = 0; y < height; y++, yInChunk++)
        {
            if (yInChunk >= TilesPerChunk)
            {
                yInChunk = 0;
                chunkY++;
            }

            for (int x = 0, xInChunk = 0, chunkX = 0; x < width; x++, xInChunk++)
            {
                if (xInChunk >= TilesPerChunk)
                {
                    xInChunk = 0;
                    chunkX++;
                }

                float minDist = float.MaxValue;
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        IntVector2 seedPos;
                        try
                        {
                            seedPos = seeds[chunkX + i, chunkY + j];
                        }
                        catch (IndexOutOfRangeException)
                        {
                            continue;
                        }
                        seedPos += new IntVector2(i, j) * TilesPerChunk;
                        IntVector2 tilePosInChunk = new(xInChunk, yInChunk);

                        IntVector2 differenceVector = (tilePosInChunk - seedPos);
                        float distance = differenceVector.Magnitude;

                        if (distance < minDist)
                            minDist = distance;
                    }
                }

                float normalisedDistance = minDist / new IntVector2(TilesPerChunk, TilesPerChunk).Magnitude;

                tiles[x, y] = (byte)(normalisedDistance * 255);
            }
        }

        return tiles;
    }

}
