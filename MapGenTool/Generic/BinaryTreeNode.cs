namespace MapGenTool.Generic;

public class BinaryTreeNode<T>(
    BinaryTreeNode<T>? parent,
    T? value,
    BinaryTreeNode<T>? left = null,
    BinaryTreeNode<T>? right = null)
{
    public BinaryTreeNode<T>? Parent { get; set; } = parent;
    public BinaryTreeNode<T>? Left { get; set; } = left;
    public BinaryTreeNode<T>? Right { get; set; } = right;
    public T? Value = value;

    public bool IsLeaf => Left is null && Right is null;
    public bool IsRoot => Parent is null;
}
