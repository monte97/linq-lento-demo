using BenchmarkDotNet.Attributes;
using LinqDeepDive.Domain;
using LinqDeepDive.Domain.Models;

namespace LinqDeepDive.Benchmarks.Benchmarks;

/// <summary>
/// Controllo su AsParallelPostBenchmark: nel pattern del post il lavoro sta nell'elementSelector
/// del ToDictionary. Qui lo stesso lavoro sta in un Select parallelo e il dizionario si costruisce
/// dopo. Serve a capire se il risultato "parallelo uguale a sequenziale" viene dal pattern o dal lavoro.
/// </summary>
[MemoryDiagnoser]
public class AsParallelSelectBenchmark
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
    public Dictionary<int, bool> SequentialSelectToDictionary()
        => _data.Vehicles.Select(v => (v.Id, Valid: IsAssignmentValid(v))).ToDictionary(x => x.Id, x => x.Valid);

    [Benchmark]
    public Dictionary<int, bool> ParallelSelectThenDictionary()
        => _data.Vehicles.AsParallel().Select(v => (v.Id, Valid: IsAssignmentValid(v))).ToDictionary(x => x.Id, x => x.Valid);
}
