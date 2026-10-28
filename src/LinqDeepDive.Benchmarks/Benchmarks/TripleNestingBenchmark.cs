using BenchmarkDotNet.Attributes;
using LinqDeepDive.Domain;
using LinqDeepDive.Domain.Models;

namespace LinqDeepDive.Benchmarks.Benchmarks;

/// <summary>
/// Errore 3 della demo: triplo nesting V x D x R, la versione originale (quella mai
/// benchmarkata nei report precedenti), contro la pre-indicizzazione con ToLookup.
/// Le taglie sono quelle di Scales (zone fisse a 20).
/// </summary>
[MemoryDiagnoser]
public class TripleNestingBenchmark
{
    [Params("S", "M", "L", "XL", "XXL")]
    public string Scale { get; set; } = "L";

    private FleetData _data = null!;

    [GlobalSetup]
    public void Setup() => _data = DataGenerator.Generate(Scales.Get(Scale));

    [Benchmark(Baseline = true)]
    public int Nested() =>
        _data.Vehicles
            .Where(v => v.IsAvailable)
            .SelectMany(v => _data.Deliveries
                .Where(d => d.Status == DeliveryStatus.Pending && d.DestinationZoneId == v.CurrentZoneId)
                .Where(d => !_data.Restrictions
                    .Any(r => r.ZoneId == d.DestinationZoneId
                           && r.VehicleType == v.Type
                           && r.BlockedSlot.Overlaps(d.PreferredSlot)))
                .Select(d => (VehicleId: v.Id, DeliveryId: d.Id)))
            .Count();

    [Benchmark]
    public int Lookup()
    {
        var pendingByZone = _data.Deliveries.Where(d => d.Status == DeliveryStatus.Pending).ToLookup(d => d.DestinationZoneId);
        var restrictionsByZoneType = _data.Restrictions.ToLookup(r => (r.ZoneId, r.VehicleType));
        return _data.Vehicles
            .Where(v => v.IsAvailable)
            .SelectMany(v =>
            {
                var zoneRestrictions = restrictionsByZoneType[(v.CurrentZoneId, v.Type)];
                return pendingByZone[v.CurrentZoneId]
                    .Where(d => !zoneRestrictions.Any(r => r.BlockedSlot.Overlaps(d.PreferredSlot)))
                    .Select(d => (VehicleId: v.Id, DeliveryId: d.Id));
            })
            .Count();
    }
}
