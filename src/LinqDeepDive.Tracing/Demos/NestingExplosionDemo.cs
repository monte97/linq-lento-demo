using System.Diagnostics;
using LinqDeepDive.Domain;
using LinqDeepDive.Domain.Models;
using LinqDeepDive.Tracing.Extensions;

namespace LinqDeepDive.Tracing.Demos;

/// <summary>
/// Visualizza l'esplosione combinatoria del triplo nesting.
/// Su Grafana Tempo: il trace "Before" avrà molti più span child del trace "After".
/// </summary>
public static class NestingExplosionDemo
{
    public static void Run(FleetData data)
    {
        using var activity = LinqTracingExtensions.Source.StartActivity("NestingExplosionDemo");

        Console.WriteLine("=== Nesting Explosion Demo ===\n");

        // Before: nesting diretto — si vedono gli span per ogni combinazione
        using (var beforeSpan = LinqTracingExtensions.Source.StartActivity("Before-TripleNesting"))
        {
            var count = 0;
            foreach (var v in data.Vehicles.Where(v => v.IsAvailable).Take(5)) // limita per non esplodere
            {
                using var vehicleSpan = LinqTracingExtensions.Source.StartActivity($"Vehicle-{v.Id}");

                var zoneDeliveries = data.Deliveries
                    .Where(d => d.Status == DeliveryStatus.Pending && d.DestinationZoneId == v.CurrentZoneId)
                    .MonitorElements($"V{v.Id}-Deliveries");

                foreach (var d in zoneDeliveries)
                {
                    var blocked = data.Restrictions
                        .Where(r => r.ZoneId == d.DestinationZoneId && r.VehicleType == v.Type)
                        .MonitorElements($"V{v.Id}-D{d.Id}-Restrictions")
                        .Any(r => r.BlockedSlot.Overlaps(d.PreferredSlot));

                    if (!blocked) count++;
                }
            }
            beforeSpan?.SetTag("demo.assignments", count);
            Console.WriteLine($"Before (5 veicoli): {count} assegnamenti");
        }

        // After: pre-indicizzato — meno span, più efficienti
        using (var afterSpan = LinqTracingExtensions.Source.StartActivity("After-PreIndexed"))
        {
            var pendingByZone = data.Deliveries
                .Where(d => d.Status == DeliveryStatus.Pending)
                .Monitor("Index-PendingByZone")
                .ToLookup(d => d.DestinationZoneId);

            var restrictionIndex = data.Restrictions
                .Monitor("Index-Restrictions")
                .ToLookup(r => (r.ZoneId, r.VehicleType));

            var count = data.Vehicles
                .Where(v => v.IsAvailable)
                .Take(5)
                .SelectMany(v =>
                {
                    var zoneDeliveries = pendingByZone[v.CurrentZoneId];
                    var zoneRestrictions = restrictionIndex[(v.CurrentZoneId, v.Type)];
                    return zoneDeliveries
                        .Where(d => !zoneRestrictions.Any(r => r.BlockedSlot.Overlaps(d.PreferredSlot)))
                        .Select(d => (v.Id, d.Id));
                })
                .Monitor("Assignments")
                .Count();

            afterSpan?.SetTag("demo.assignments", count);
            Console.WriteLine($"After (5 veicoli): {count} assegnamenti");
        }
    }
}
