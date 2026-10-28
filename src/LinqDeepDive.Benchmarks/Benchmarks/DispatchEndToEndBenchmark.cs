using BenchmarkDotNet.Attributes;
using LinqDeepDive.Domain;
using LinqDeepDive.Domain.Scenarios;

namespace LinqDeepDive.Benchmarks.Benchmarks;

/// <summary>
/// Scenario end-to-end: un giro di dispatch con i quattro pattern dentro (Dispatcher.Before)
/// contro la versione corretta (Dispatcher.After). Budget di riferimento del talk: 100 ms.
/// Il setup verifica che le due versioni diano lo stesso risultato.
/// </summary>
[MemoryDiagnoser]
public class DispatchEndToEndBenchmark
{
    [Params("S", "M", "L", "XL", "XXL")]
    public string Scale { get; set; } = "L";

    private FleetData _data = null!;

    [GlobalSetup]
    public void Setup()
    {
        _data = DataGenerator.Generate(Scales.Get(Scale));
        var before = Dispatcher.Before(_data);
        var after = Dispatcher.After(_data);
        if (before != after)
            throw new InvalidOperationException($"Before e After divergono: {before} contro {after}");
        Console.WriteLine($"// Dispatch {Scale}: {before}");
    }

    [Benchmark(Baseline = true)]
    public DispatchResult Before() => Dispatcher.Before(_data);

    [Benchmark]
    public DispatchResult After() => Dispatcher.After(_data);
}
