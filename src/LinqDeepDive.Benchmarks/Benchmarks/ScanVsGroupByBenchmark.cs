using BenchmarkDotNet.Attributes;
using LinqDeepDive.Domain;
using LinqDeepDive.Domain.Models;

namespace LinqDeepDive.Benchmarks.Benchmarks;

/// <summary>
/// Scansione ripetuta (una per zona) contro GroupBy. Costo del baseline: Zone x Consegne.
/// Zone e consegne sono parametri espliciti, cosi' si vede la crescita quadratica.
/// </summary>
[MemoryDiagnoser]
public class ScanVsGroupByBenchmark
{
    [Params(20, 200, 2_000)]
    public int ZoneCount { get; set; }

    [Params(2_000, 20_000, 100_000)]
    public int DeliveryCount { get; set; }

    private FleetData _data = null!;

    [GlobalSetup]
    public void Setup() =>
        _data = DataGenerator.Generate(new FleetOptions(200, DeliveryCount, 100, ZoneCount));

    [Benchmark(Baseline = true)]
    public List<(string Zone, int Count)> RepeatedScan() =>
        _data.Zones.Select(z => (
            Zone: z.Name,
            Count: _data.Deliveries.Count(d => d.DestinationZoneId == z.Id && d.Status == DeliveryStatus.Pending)
        )).ToList();

#if !NETFRAMEWORK // .NET Framework 4.8 non ha l'API: su 4.8 si misura solo il codice «prima»
    [Benchmark]
    public List<(string Zone, int Count)> GroupByDictionary()
    {
        var pendingByZone = _data.Deliveries
            .Where(d => d.Status == DeliveryStatus.Pending)
            .GroupBy(d => d.DestinationZoneId)
            .ToDictionary(g => g.Key, g => g.Count());

        return _data.Zones.Select(z => (
            Zone: z.Name,
            Count: pendingByZone.GetValueOrDefault(z.Id, 0)
        )).ToList();
    }
#endif
}
