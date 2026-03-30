using MapGenTool.Generic;
using System.ComponentModel;
using System.Net.Http.Headers;

namespace MapGenTool.Generators;

public static partial class Rooms {
    public static Tiles[,] PrefabRooms(int width, int height, int seed, int roomsCount, string pathString) {
        const char s_headerChar = '!';
        const char s_headerSeperatorChar = ',';
        const char s_commentChar = '#';

        FileInfo prefabFile = new(pathString);
        using StreamReader sR = new(prefabFile.FullName);

        string? header = sR.ReadLine();
        if (header is null)
            throw new InvalidDataException("Empty file provided.");
        if (header[0] != s_headerChar)
            throw new InvalidDataException("No header found at start of file. Start with #");
        string[] headerInfo = header.TrimStart(s_headerChar).Split(s_headerSeperatorChar, StringSplitOptions.TrimEntries);
        string dataType = headerInfo[0].ToLower();
        int prefabWidth = int.Parse(headerInfo[1]);
        int prefabHeight = int.Parse(headerInfo[2]);
        int numberOfPrefabs = 1;
        if (headerInfo[3] is not null)
            numberOfPrefabs = int.Parse(headerInfo[3]);
        Tiles[][,] prefabs = new Tiles[numberOfPrefabs][,];

        switch (dataType) {
            case "tiles":
                ReadTilesPrefab();
                break;
            default:
                throw new InvalidDataException($"Datatype '{dataType}' provided in header is not supported.");
        }
        void ReadTilesPrefab() {
            const int tileLength = 3;
            char[] buffer = new char[tileLength];
            for (int p = 0; p < prefabs.Length; p++) {
                prefabs[p] = new Tiles[prefabWidth, prefabHeight];
                for (int y = 0; y < prefabHeight; y++) {
                    for (int x = 0; x < prefabWidth; x++) {
                        int charL = sR.Read(buffer, 0, tileLength);
                        if (buffer[0] == s_commentChar) {
                            y--;
                            break;
                        }
                        if (charL != tileLength)
                            throw new InvalidOperationException("Char length was not at the desired length.");
                        Tiles value = (Tiles)byte.Parse(buffer);
                        prefabs[p][x, y] = value;
                    }
                    _ = sR.ReadLine();
                    if (sR.EndOfStream)
                        goto end_for;
                }
            }
            end_for:;
        }

        Random rng = new(seed);
        var grid = new Tiles[width, height];
        int[,] usedSpace = new int[width, height];
        int generated = 0;
        long terminationCount = roomsCount * roomsCount * roomsCount;
        long tryCount = 0;

        while (generated < roomsCount && tryCount < terminationCount) {
            int k = rng.Next(0, prefabs.Length);
            Tiles[,] pref = prefabs[k];

            int x1 = rng.Next(0, width - prefabWidth - 1);
            int y1 = rng.Next(0, height - prefabHeight - 1);

            int x2 = x1 + prefabWidth;
            int y2 = y1 + prefabHeight;

            if (isOverlapping()) {
                tryCount++;
                continue;
            }

            bool isOverlapping() {
                for (int y = y1; y < y2; y++) {
                    for (int x = x1; x < x2; x++) {
                        if (usedSpace[x, y] != 0) {
                            return true;
                        }
                    }
                }
                return false;
            }

            for (int y = y1; y < y2; y++) {
                int j = y - y1;
                for (int x = x1; x < x2; x++) {
                    int i = x - x1;

                    usedSpace[x, y] = k + 1;
                    grid[x, y] = pref[i, j];
                }
            }

            if (tryCount > 0) {
                Console.WriteLine($"#{generated}: attempted: {tryCount}");
            }
            tryCount = 0;
            generated++;
        }
        Console.WriteLine();
        if (tryCount >= terminationCount) {
            Console.WriteLine($"terminated at {tryCount} tries");
        } else {
            Console.WriteLine($"generated all {roomsCount} rooms");
        }

        return grid;
    }
}
