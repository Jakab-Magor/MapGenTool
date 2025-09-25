using MapGenTool.Generic;

namespace MapGenTool.Generators.RoomGenerators;

internal class BSPNode(
    BSPNode? parent,
    Room bounds,
    BSPNode? left = null,
    BSPNode? right = null)
{
    public BSPNode? Parent { get; set; } = parent;
    public BSPNode? Left { get; set; } = left;
    public BSPNode? Right { get; set; } = right;
    public Room Bounds { get; init; } = bounds;
    public Room InnerRoom { get; set; }

    public bool IsLeaf => Left is null && Right is null;
    public bool IsRoot => Parent is null;

    public BSPNode[] GetLeaves()
    {
        if (IsLeaf)
            return [this];

        return [.. Left?.GetLeaves() ?? [], .. Right?.GetLeaves() ?? []];
    }
    public void Split(int remaining, bool isVerticalSplit)
    {
        if (remaining == 0)
            return;

        IntVector2 leftSize;
        IntVector2 rightSize;
        IntVector2 rightPos;
        if (isVerticalSplit)
        {
            leftSize = new IntVector2(Bounds.Size.x / 2, Bounds.Size.y);
            rightSize = new IntVector2(Bounds.Size.x - leftSize.x, Bounds.Size.y);
            rightPos = new IntVector2(Bounds.Position.x + leftSize.x, Bounds.Position.y);
        }
        else
        {
            leftSize = new IntVector2(Bounds.Size.x, Bounds.Size.y / 2);
            rightSize = new IntVector2(Bounds.Size.x, Bounds.Size.y - leftSize.y);
            rightPos = new IntVector2(Bounds.Position.x, Bounds.Position.y+leftSize.y);
        }

        Room l = new(Bounds.Position, leftSize);
        Room r = new(rightPos, rightSize);

        Left = new(this, l);
        Right = new(this, r);

        Left.Split(remaining /2, !isVerticalSplit);
        Right.Split(remaining /2, !isVerticalSplit);
    }
}
