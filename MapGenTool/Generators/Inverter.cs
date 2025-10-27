
namespace MapGenTool.Generators;

public class Inverter : IGenerator<Tiles> {
    public byte ArgsCount => 0;

    public bool UsesInput => true;

    public Type InputType => typeof(Tiles);
    private Tiles[,] _baseGrid = null!;

    public Tiles[,] Generate(int width, int height, int seed) {
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                ref Tiles tile = ref _baseGrid[x, y];
                switch (tile) {
                    case Tiles.Wall:
                        tile = Tiles.Space;
                        break;
                    case Tiles.Space:
                        tile = Tiles.Wall;
                        break;
                    default:
                        break;
                }
            }
        }

        return _baseGrid;
    }

    public void Parse(params string[] args) {
    }

    public void SetBaseGrid<T>(T[,] basegrid) where T : IConvertible {
        _baseGrid = IGenerator<Tiles>.CastGrid<T>(basegrid);
    }
}
