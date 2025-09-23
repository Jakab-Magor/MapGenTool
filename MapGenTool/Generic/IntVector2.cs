namespace MapGenTool.Generic;

internal record struct IntVector2(int x, int y)
{
    public static IntVector2 One => new IntVector2(1, 1);
    public static IntVector2 Zero => new IntVector2(0, 0);
    public static IntVector2 operator +(IntVector2 a, IntVector2 b) => new IntVector2(a.x + b.x, a.y + b.y);
    public static IntVector2 operator -(IntVector2 a, IntVector2 b) => new IntVector2(a.x - b.x, a.y - b.y);
    public static IntVector2 operator *(IntVector2 a, int s) => new IntVector2(a.x * s, a.y * s);
    public static IntVector2 operator /(IntVector2 a, int s) => new IntVector2(a.x / s, a.y / s);
    public readonly float Magnitude => MathF.Sqrt(x * x + y * y);

    public override string? ToString()
    {
        return $"({x}, {y})";
    }
}
