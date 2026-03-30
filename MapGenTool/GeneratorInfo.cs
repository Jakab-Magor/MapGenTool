namespace MapGenTool;
public enum GeneratorTypes : byte {
    First,
    Follower,
    Binary
}
public record class GeneratorInfo(GeneratorTypes generatorType, Type[] inputTypes, Type? returnType, string shortDescription, params string[] parameters) {
}
