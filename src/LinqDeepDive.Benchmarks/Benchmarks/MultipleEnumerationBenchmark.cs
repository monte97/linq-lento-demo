using BenchmarkDotNet.Attributes;
using LinqDeepDive.Domain;
using LinqDeepDive.Domain.Models;

namespace LinqDeepDive.Benchmarks.Benchmarks;

/// <summary>
/// Una query tenuta come IEnumerable e usata tre volte (Count, Max, First) contro una
/// ToList() fatta una volta e letta tre volte. Pattern 3 del talk (R5 di docs/05-revisione.md).
/// </summary>
[MemoryDiagnoser]
public class MultipleEnumerationBenchmark
{
    [Params("S", "M", "L", "XL")]
    public string Scale { get; set; } = "L";

    private FleetData _data = null!;

    [GlobalSetup]
    public void Setup() => _data = DataGenerator.Generate(Scales.Get(Scale));

    [Benchmark(Baseline = true)]
    public (int, double, int) ThreeEnumerations()
    {
        IEnumerable<Delivery> pending = _data.Deliveries
            .Where(d => d.Status == DeliveryStatus.Pending && d.Requirements.Any(r => r.Type == "Fragile"));
        var n = pending.Count();
        var max = pending.Max(d => d.WeightKg);
        var first = pending.First().Id;
        return (n, max, first);
    }

    [Benchmark]
    public (int, double, int) MaterializedOnce()
    {
        var pending = _data.Deliveries
            .Where(d => d.Status == DeliveryStatus.Pending && d.Requirements.Any(r => r.Type == "Fragile"))
            .ToList();
        var n = pending.Count;
        var max = pending.Max(d => d.WeightKg);
        var first = pending.First().Id;
        return (n, max, first);
    }
}
