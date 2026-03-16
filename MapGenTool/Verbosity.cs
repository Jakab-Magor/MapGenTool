[Flags]
enum Verbosity:int {
    None = 0,
    Finished = 1,
    // 2 Generators
    Generators = 3,
    // 4 Treeview
    ParseTree = 7,
    // 8 Benchmarking
    Benchmarking = 10,

    All = 2147483647,
}

/// -----------------------------------
/// TODO:
/// -----------------------------------
/// - Diffusion limited aggregation
/// - Dijkstra map
/// - Fused room placer
/// - Wave function collapse
/// - Refactor to not use polymorphism
/// 