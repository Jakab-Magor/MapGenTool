
namespace MapGenTool.Generators;

public class PrefabGenerator : IGenerator<Tiles> {
    public byte ArgsCount => 1;

    public bool UsesInput => false;

    public Type InputType => throw new NotImplementedException();
    public FileInfo PrefabFile { get; set; } = null!;
    static readonly char s_headerChar = '!';
    static readonly char s_headerSeperatorChar = ',';
    static readonly char s_commentChar = '#';
    public Tiles[][,] Prefabs { get; set; } = null!;

    public Tiles[,] Generate(int width, int height, int seed) {
        Tiles[,] grid = new Tiles[width, height];

        StreamReader sR = new(PrefabFile.FullName);

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
        Prefabs = new Tiles[numberOfPrefabs][,];

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
            for (int p = 0; p < Prefabs.Length; p++) {
                Prefabs[p] = new Tiles[prefabWidth, prefabHeight];
                for (int y = 0; y < prefabHeight; y++) {
                    for (int x = 0; x < prefabWidth; x++) {
                        int charL = sR.Read(buffer, 0, tileLength);
                        if (buffer[0] == s_commentChar) {
                            y--;
                            break;
                        }
                        if (charL != tileLength)
                            throw new InvalidOperationException("Char length was not at the desired length.");
                        Tiles value = (Tiles)(Byte.Parse(buffer));
                        Prefabs[p][x, y] = value;
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

                int k = rng.Next(Prefabs.Length);
                Tiles[,] pref = Prefabs[k];
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
    public void Parse(params string[] args) {
        string pathString = args[0];
        PrefabFile = new(pathString);
    }

    public void SetBaseGrid<T>(T[,] basegrid) where T : IConvertible {
        throw new NotImplementedException();
    }
}
