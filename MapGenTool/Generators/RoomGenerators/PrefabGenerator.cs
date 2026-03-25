namespace MapGenTool.Generators;

public static partial class Rooms {
    public static Tiles[,] Prefabs(int width, int height, int seed, string pathString) {
        const char s_headerChar = '!';
        const char s_headerSeperatorChar = ',';
        const char s_commentChar = '#';

        Tiles[,] grid = new Tiles[width, height];
        FileInfo prefabFile = new(pathString);
        Tiles[][,] prefabs = null!;

        StreamReader sR = new(prefabFile.FullName);

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
        prefabs = new Tiles[numberOfPrefabs][,];

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
        int xChunksCount = width / prefabWidth;
        int widthSpacing = width % prefabWidth / xChunksCount;
        int xChunkSpaced = prefabWidth + widthSpacing;

        int yChunksCount = height / prefabHeight;
        int heightSpacing = height % prefabHeight / yChunksCount;
        int yChunkSpaced = prefabHeight + heightSpacing;

        for (int yChunk = 0; yChunk < yChunksCount; yChunk++) {
            int yChunkWorldPos = yChunk * yChunkSpaced;

            for (int xChunk = 0; xChunk < xChunksCount; xChunk++) {
                int xChunkWorldPos = xChunk * xChunkSpaced;

                int k = rng.Next(prefabs.Length);
                Tiles[,] pref = prefabs[k];
                for (int y = 0; y < prefabHeight; y++) {
                    for (int x = 0; x < prefabWidth; x++) {
                        int gridPosX = x + xChunkWorldPos;
                        int gridPosY = y + yChunkWorldPos;

                        grid[gridPosX, gridPosY] = pref[x, y];
                    }
                }
            }
        }

        sR.Close();
        return grid;
    }
}
