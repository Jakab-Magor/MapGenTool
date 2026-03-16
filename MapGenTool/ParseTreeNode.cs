using MapGenTool.Generators;

namespace MapGenTool;

public class ParseTreeNode(ParseTreeNode? parent,string[] contents) {
    public ParseTreeNode? parent = parent;
    public string[] contents = contents;
    public ParseTreeNode? left = null;
    public ParseTreeNode? right = null;
}
