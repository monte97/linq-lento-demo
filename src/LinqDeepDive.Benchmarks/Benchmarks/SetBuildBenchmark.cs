using BenchmarkDotNet.Attributes;

namespace LinqDeepDive.Benchmarks.Benchmarks;

/// <summary>
/// L'obiezione "costruire l'HashSet costa" (R14 di docs/05-revisione.md): cerca N chiavi in una
/// lista di lunghezza L, contro costruire un HashSet dalla lista a ogni chiamata e poi cercare.
/// Il punto di pareggio sta fra i valori di Lookups per ogni ListLength.
/// </summary>
[MemoryDiagnoser]
public class SetBuildBenchmark
{
    [Params(7, 47, 224)]
    public int ListLength { get; set; }

    [Params(10, 100, 1_000)]
    public int Lookups { get; set; }

    private string[] _keys = null!;
    private List<string> _list = null!;

    [GlobalSetup]
    public void Setup()
    {
        var rnd = new Random(42);
        var universe = ListLength * 3;
        _keys = Enumerable.Range(0, Lookups).Select(_ => "ZONE-" + rnd.Next(universe)).ToArray();
        _list = Enumerable.Range(0, ListLength).Select(i => "ZONE-" + i).ToList();
    }

    [Benchmark(Baseline = true)]
    public int ListLookups() => _keys.Count(k => _list.Contains(k));

    [Benchmark]
    public int BuildSetThenLookups()
    {
        var set = _list.ToHashSet();
        return _keys.Count(k => set.Contains(k));
    }
}
