using BenchmarkDotNet.Attributes;
using LinqDeepDive.Domain;
using LinqDeepDive.Domain.Scenarios;

namespace LinqDeepDive.Benchmarks.Benchmarks;

/// <summary>
/// Come DispatchEndToEndBenchmark, ma sul dataset con assegnamenti non vuoti
/// (FleetOptions.BlockedShare = 0.7: il 70% delle combinazioni zona/tipo/fascia ha vincoli).
/// Le scale sono le stesse del benchmark storico, con suffisso "@70".
/// </summary>
[MemoryDiagnoser]
public class DispatchOpenSlotsBenchmark
{
    [Params("L@70", "XL@70", "XL12@70", "XL15@70")]
    public string Scale { get; set; } = "L@70";

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
