#if NET11_0_OR_GREATER
using BenchmarkDotNet.Attributes;
using LinqDeepDive.Domain;
using LinqDeepDive.Domain.Scenarios;

namespace LinqDeepDive.Benchmarks.Benchmarks;

/// <summary>
/// Report di copertura delle zone: full join scritto a mano (Classic) contro CountBy e FullJoin
/// (Modern, solo .NET 11). Zones fa crescere il numero di zone, cioe' le righe del join:
/// con 20 zone il join e' piccolo e conta soprattutto il conteggio per zona.
/// Il setup verifica che le due versioni diano le stesse righe.
/// </summary>
[MemoryDiagnoser]
public class ZoneCoverageBenchmark
{
    [Params(20, 2_000)]
    public int Zones { get; set; } = 20;

    [Params("L", "XXL")]
    public string Scale { get; set; } = "L";

    private FleetData _data = null!;

    [GlobalSetup]
    public void Setup()
    {
        _data = DataGenerator.Generate(Scales.Get(Scale) with { Zones = Zones });
        var classic = ZoneCoverage.Classic(_data);
        var modern = ZoneCoverage.Modern(_data);
        if (!classic.SequenceEqual(modern))
            throw new InvalidOperationException($"Classic e Modern divergono: {classic.Count} righe contro {modern.Count}");
        Console.WriteLine($"// ZoneCoverage {Scale} z{Zones}: {classic.Count} righe");
    }

    [Benchmark(Baseline = true)]
    public List<ZoneCoverageRow> Classic() => ZoneCoverage.Classic(_data);

    [Benchmark]
    public List<ZoneCoverageRow> Modern() => ZoneCoverage.Modern(_data);
}
#endif
