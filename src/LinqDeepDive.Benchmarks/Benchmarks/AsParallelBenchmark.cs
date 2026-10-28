using BenchmarkDotNet.Attributes;
using LinqDeepDive.Domain;
using LinqDeepDive.Domain.Models;

namespace LinqDeepDive.Benchmarks.Benchmarks;

[MemoryDiagnoser]
public class AsParallelBenchmark
{
    [Params("S", "M", "L", "XL")]
    public string Scale { get; set; } = "L";

    private FleetData _data = null!;

    [GlobalSetup]
    public void Setup()
    {
        _data = DataGenerator.Generate(Scales.Get(Scale));
    }

    [Benchmark(Baseline = true)]
    public List<(int VehicleId, int DeliveryId)> SequentialNesting()
    {
        var pendingByZone = _data.Deliveries
            .Where(d => d.Status == DeliveryStatus.Pending)
            .ToLookup(d => d.DestinationZoneId);

        var restrictionsByZoneType = _data.Restrictions
            .ToLookup(r => (r.ZoneId, r.VehicleType));

        return _data.Vehicles
            .Where(v => v.IsAvailable)
            .SelectMany(v =>
            {
                var zoneDeliveries = pendingByZone[v.CurrentZoneId];
                var zoneRestrictions = restrictionsByZoneType[(v.CurrentZoneId, v.Type)];

                return zoneDeliveries
                    .Where(d => !zoneRestrictions.Any(r => r.BlockedSlot.Overlaps(d.PreferredSlot)))
                    .Select(d => (VehicleId: v.Id, DeliveryId: d.Id));
            })
            .ToList();
    }

    [Benchmark]
    public List<(int VehicleId, int DeliveryId)> ParallelNesting()
    {
        var pendingByZone = _data.Deliveries
            .Where(d => d.Status == DeliveryStatus.Pending)
            .ToLookup(d => d.DestinationZoneId);

        var restrictionsByZoneType = _data.Restrictions
            .ToLookup(r => (r.ZoneId, r.VehicleType));

        return _data.Vehicles
            .Where(v => v.IsAvailable)
            .AsParallel()
            .SelectMany(v =>
            {
                var zoneDeliveries = pendingByZone[v.CurrentZoneId];
                var zoneRestrictions = restrictionsByZoneType[(v.CurrentZoneId, v.Type)];

                return zoneDeliveries
                    .Where(d => !zoneRestrictions.Any(r => r.BlockedSlot.Overlaps(d.PreferredSlot)))
                    .Select(d => (VehicleId: v.Id, DeliveryId: d.Id));
            })
            .ToList();
    }
}
