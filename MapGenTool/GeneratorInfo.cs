namespace MapGenTool;
internal enum GeneratorTypes : byte {
    First,
    Follower,
    Binary
}
internal record class GeneratorInfo(int paramCount, GeneratorTypes generatorType, Type returnType) {
}
