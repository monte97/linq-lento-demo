using System.Collections.Frozen;
using BenchmarkDotNet.Attributes;

namespace LinqDeepDive.Benchmarks.Benchmarks;

/// <summary>
/// La lunghezza della lista di lookup come parametro esplicito, indipendente dalle zone del
/// generatore. Chiavi sintetiche ("ZONE-n"), circa un terzo delle chiavi dei veicoli e' nella lista.
/// Misura con Count (senza materializzare). Risponde a R12 di docs/05-revisione.md.
/// </summary>
[MemoryDiagnoser]
public class ListLengthBenchmark
{
    [Params(500, 5_000)]
    public int Vehicles { get; set; }

    [Params(7, 47, 150, 224, 700)]
    public int ListLength { get; set; }

    private string[] _keys = null!;
    private List<string> _list = null!;
    private string[] _array = null!;
    private HashSet<string> _set = null!;
    private FrozenSet<string> _frozen = null!;

    [GlobalSetup]
    public void Setup()
    {
        var rnd = new Random(42);
        var universe = ListLength * 3;
        _keys = Enumerable.Range(0, Vehicles).Select(_ => "ZONE-" + rnd.Next(universe)).ToArray();
        _list = Enumerable.Range(0, ListLength).Select(i => "ZONE-" + i).ToList();
        _array = _list.ToArray();
        _set = _list.ToHashSet();
        _frozen = _list.ToFrozenSet();
        Console.WriteLine($"// ListLength: veicoli={Vehicles} lunghezza lista={_list.Count} match={_keys.Count(k => _set.Contains(k))}");
    }

    [Benchmark(Baseline = true)]
    public int ListContains() => _keys.Count(k => _list.Contains(k));

    [Benchmark]
    public int ArrayContains() => _keys.Count(k => _array.Contains(k));

    [Benchmark]
    public int HashSetContains() => _keys.Count(k => _set.Contains(k));

    [Benchmark]
    public int FrozenSetContains() => _keys.Count(k => _frozen.Contains(k));
}
