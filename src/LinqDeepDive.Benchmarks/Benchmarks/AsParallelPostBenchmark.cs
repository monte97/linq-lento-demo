using BenchmarkDotNet.Attributes;
using LinqDeepDive.Domain;
using LinqDeepDive.Domain.Models;

namespace LinqDeepDive.Benchmarks.Benchmarks;

/// <summary>
/// Il pattern del post 01 (BLOG/01, "La Trappola di AsParallel"): ToDictionary con AsParallel
/// sui veicoli, contro ToDictionary sequenziale e contro il foreach con Dictionary pre-allocato.
/// Due pesi del lavoro per elemento: Light (early exit alla prima restrizione utile) e Heavy
/// (scansione di tutte le restrizioni). Risponde a R4 di docs/05-revisione.md.
/// </summary>
[MemoryDiagnoser]
public class AsParallelPostBenchmark
{
    [Params(50, 200, 500, 2_000, 5_000)]
    public int Vehicles { get; set; }

    [Params("Light", "Heavy")]
    public string Work { get; set; } = "Light";

    private FleetData _data = null!;

    [GlobalSetup]
    public void Setup()
    {
        _data = DataGenerator.Generate(new FleetOptions(Vehicles, Vehicles * 10, Vehicles * 4));
    }

    private bool IsAssignmentValid(Vehicle v) => Work == "Light"
        ? !_data.Restrictions.Any(r => r.ZoneId == v.CurrentZoneId && r.VehicleType == v.Type)
        : _data.Restrictions.Count(r => r.ZoneId == v.CurrentZoneId && r.VehicleType == v.Type) == 0;

    [Benchmark(Baseline = true)]
    public Dictionary<int, bool> ForeachPreallocated()
    {
        var results = new Dictionary<int, bool>(capacity: _data.Vehicles.Count);
        foreach (var v in _data.Vehicles) results[v.Id] = IsAssignmentValid(v);
        return results;
    }

    [Benchmark]
    public Dictionary<int, bool> SequentialToDictionary()
        => _data.Vehicles.ToDictionary(v => v.Id, v => IsAssignmentValid(v));

    [Benchmark]
    public Dictionary<int, bool> ParallelToDictionary()
        => _data.Vehicles.AsParallel().ToDictionary(v => v.Id, v => IsAssignmentValid(v));
}
