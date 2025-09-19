namespace MapGenTool;

public struct TileGrid
{
    public int Height => _grid.GetLength(1);
    public int Width => _grid.GetLength(0);

    public readonly TileTypes this[int x, int y] => _grid[x, y];
    private readonly TileTypes[,] _grid = null!;

    public TileGrid(TileTypes[,] grid)
    {
        this._grid = grid;
    }
}
