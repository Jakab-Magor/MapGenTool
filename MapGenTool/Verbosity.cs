[Flags]
enum Verbosity : int {
    None = 0,
    Finished = 1,
    Generators = 1 + 2,
    ParseTree = 4 + 2 + 1,
    Benchmarking = 8 + 2,
    GeneratorDebug = 16 + 2,

    All = int.MaxValue,
}
