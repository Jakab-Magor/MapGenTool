[Flags]
enum Verbosity : int {
    None = 0,
    Finished = 1,
    Generators = 2 | Finished,
    ParseTree = 4 | 2 | Finished,
    Benchmarking = 8 | 2,
    GeneratorDebug = 16 | 2,

    All = int.MaxValue,
}
