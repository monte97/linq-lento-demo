using System.Diagnostics;
using LinqDeepDive.Domain;
using LinqDeepDive.Domain.Models;

namespace LinqDeepDive.Demo.Errors;

/// <summary>
/// Errore 3: Triplo nesting SelectMany/Where — O(V*D*R)
/// Scenario: per ogni veicolo disponibile, per ogni consegna pending nella sua zona,
/// verifica che non ci siano restrizioni che bloccano il tipo di veicolo nello slot richiesto.
/// </summary>
public static class Error3_TripleNesting
{
    public static void RunBefore(FleetData data)
    {
        Console.WriteLine("=== BEFORE: Triplo nesting (V * D * R) ===\n");

        var sw = Stopwatch.StartNew();

        // O(V * D * R) — tre livelli di iterazione annidati
        var assignments = data.Vehicles
            .Where(v => v.IsAvailable)
            .SelectMany(v => data.Deliveries
                .Where(d => d.Status == DeliveryStatus.Pending
                         && d.DestinationZoneId == v.CurrentZoneId)
                .Where(d => !data.Restrictions
                    .Any(r => r.ZoneId == d.DestinationZoneId
                           && r.VehicleType == v.Type
                           && r.BlockedSlot.Overlaps(d.PreferredSlot)))
                .Select(d => new { VehicleId = v.Id, DeliveryId = d.Id }))
            .ToList();

        sw.Stop();

        Console.WriteLine($"Assegnamenti validi: {assignments.Count}");
        Console.WriteLine($"Tempo: {sw.Elapsed.TotalMilliseconds:F3} ms\n");
    }

    public static void RunAfter(FleetData data)
    {
        Console.WriteLine("=== AFTER: Pre-indicizzazione con Dictionary + Lookup ===\n");

        var sw = Stopwatch.StartNew();

        // Pre-indicizzazione: O(D) + O(R)
        var pendingByZone = data.Deliveries
            .Where(d => d.Status == DeliveryStatus.Pending)
            .ToLookup(d => d.DestinationZoneId);

        var restrictionsByZoneAndType = data.Restrictions
            .ToLookup(r => (r.ZoneId, r.VehicleType));

        // O(V * D_zona) — dove D_zona << D totali
        var assignments = data.Vehicles
            .Where(v => v.IsAvailable)
            .SelectMany(v =>
            {
                var zoneDeliveries = pendingByZone[v.CurrentZoneId];
                var zoneRestrictions = restrictionsByZoneAndType[(v.CurrentZoneId, v.Type)];

                return zoneDeliveries
                    .Where(d => !zoneRestrictions.Any(r => r.BlockedSlot.Overlaps(d.PreferredSlot)))
                    .Select(d => new { VehicleId = v.Id, DeliveryId = d.Id });
            })
            .ToList();

        sw.Stop();

        Console.WriteLine($"Assegnamenti validi: {assignments.Count}");
        Console.WriteLine($"Tempo: {sw.Elapsed.TotalMilliseconds:F3} ms\n");
    }

    public static void RunComparison(FleetData data)
    {
        Console.WriteLine(">>> Errore 3: Triplo nesting vs Pre-indicizzazione <<<\n");
        RunBefore(data);
        RunAfter(data);
        Console.WriteLine(new string('-', 50));
    }
}
