using BenchmarkDotNet.Attributes;
using LinqDeepDive.Domain;
using LinqDeepDive.Domain.Models;

namespace LinqDeepDive.Benchmarks.Benchmarks;

/// <summary>
/// Any contro Count() > 0 con l'elemento cercato in testa, a meta' o assente.
/// L'unico elemento che soddisfa la condizione e' scelto per posizione (R5 e 6e del talk).
/// </summary>
[MemoryDiagnoser]
public class AnyPositionBenchmark
{
    [Params("L", "XL")]
    public string Scale { get; set; } = "L";

    [Params("Head", "Middle", "Absent")]
    public string Position { get; set; } = "Head";

    private List<Delivery> _deliveries = null!;
    private int _target;

    [GlobalSetup]
    public void Setup()
    {
        _deliveries = DataGenerator.Generate(Scales.Get(Scale)).Deliveries;
        _target = Position switch
        {
            "Head" => _deliveries[0].Id,
            "Middle" => _deliveries[_deliveries.Count / 2].Id,
            _ => -1
        };
    }

    [Benchmark(Baseline = true)]
    public bool CountGreaterZero() => _deliveries.Where(d => d.Id == _target).Count() > 0;

    [Benchmark]
    public bool Any() => _deliveries.Any(d => d.Id == _target);
}
