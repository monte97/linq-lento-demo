using System.Collections.Frozen;
using BenchmarkDotNet.Attributes;

namespace LinqDeepDive.Benchmarks.Benchmarks;

/// <summary>
/// Solo il costo di costruire il set dalla lista (N5 di docs/06-verifica.md): HashSet e FrozenSet
/// da una lista di lunghezza L. Il punto di pareggio si calcola dai tempi di ListLengthBenchmark.
/// </summary>
[MemoryDiagnoser]
public class SetBuildCostBenchmark
{
    [Params(7, 47, 224, 700)]
    public int ListLength { get; set; }

    private List<string> _list = null!;

    [GlobalSetup]
    public void Setup() => _list = Enumerable.Range(0, ListLength).Select(i => "ZONE-" + i).ToList();

    [Benchmark(Baseline = true)]
    public HashSet<string> BuildHashSet() => _list.ToHashSet();

    [Benchmark]
    public FrozenSet<string> BuildFrozenSet() => _list.ToFrozenSet();
}
