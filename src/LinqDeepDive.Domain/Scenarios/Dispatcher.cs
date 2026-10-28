using LinqDeepDive.Domain.Models;

namespace LinqDeepDive.Domain.Scenarios;

/// <summary>Esito di un giro di dispatch. Serve a verificare che prima e dopo diano lo stesso risultato.</summary>
public record DispatchResult(
    int VehiclesInRestrictedZones,
    int Assignments,
    int PendingTotal,
    bool HasHazMat,
    int HeaviestPendingId);

/// <summary>
/// Scenario end-to-end della demo: un giro di dispatch che mette insieme i quattro pattern
/// (List.Contains, scansione ripetuta per zona, triplo nesting, ToList intermedi e Count() > 0).
/// Sintetico: non e' il servizio di un cliente. Le due versioni fanno lo stesso lavoro.
/// </summary>
public static class Dispatcher
{
    public static DispatchResult Before(FleetData data)
    {
        // Pattern 1: List.Contains in Where
        var restrictedZoneIds = data.Zones.Where(z => z.IsRestricted).Select(z => z.Id).ToList();
        var inRestricted = data.Vehicles
            .Where(v => restrictedZoneIds.Contains(v.CurrentZoneId))
            .ToList();

        // Pattern 2: scansione ripetuta per ogni zona
        var pendingPerZone = data.Zones.Select(z => (
            Zone: z.Id,
            Count: data.Deliveries.Count(d => d.DestinationZoneId == z.Id && d.Status == DeliveryStatus.Pending)
        )).ToList();

        // Pattern 3: triplo nesting V x D x R
        var assignments = data.Vehicles
            .Where(v => v.IsAvailable)
            .SelectMany(v => data.Deliveries
                .Where(d => d.Status == DeliveryStatus.Pending && d.DestinationZoneId == v.CurrentZoneId)
                .Where(d => !data.Restrictions
                    .Any(r => r.ZoneId == d.DestinationZoneId
                           && r.VehicleType == v.Type
                           && r.BlockedSlot.Overlaps(d.PreferredSlot)))
                .Select(d => (VehicleId: v.Id, DeliveryId: d.Id)))
            .ToList();

        // Pattern 4: ToList intermedi, Count() > 0, OrderBy + First
        var pending = data.Deliveries
            .Where(d => d.WeightKg > 0)
            .ToList()
            .Where(d => d.Status == DeliveryStatus.Pending)
            .ToList();
        var hasHazMat = data.Deliveries
            .Where(d => d.Requirements.Any(r => r.Type == "HazMat"))
            .Count() > 0;
        var heaviest = pending.OrderByDescending(d => d.WeightKg).ThenBy(d => d.Id).First();

        return new DispatchResult(inRestricted.Count, assignments.Count, pendingPerZone.Sum(p => p.Count), hasHazMat, heaviest.Id);
    }

    public static DispatchResult After(FleetData data)
    {
        var restrictedZoneIds = data.Zones.Where(z => z.IsRestricted).Select(z => z.Id).ToHashSet();
        var inRestricted = data.Vehicles.Count(v => restrictedZoneIds.Contains(v.CurrentZoneId));

        var pendingByZone = data.Deliveries
            .Where(d => d.Status == DeliveryStatus.Pending)
            .ToLookup(d => d.DestinationZoneId);
        var pendingPerZone = data.Zones.Select(z => (Zone: z.Id, Count: pendingByZone[z.Id].Count())).ToList();

        var restrictionsByZoneType = data.Restrictions.ToLookup(r => (r.ZoneId, r.VehicleType));
        var assignments = data.Vehicles
            .Where(v => v.IsAvailable)
            .SelectMany(v =>
            {
                var zoneRestrictions = restrictionsByZoneType[(v.CurrentZoneId, v.Type)];
                return pendingByZone[v.CurrentZoneId]
                    .Where(d => !zoneRestrictions.Any(r => r.BlockedSlot.Overlaps(d.PreferredSlot)))
                    .Select(d => (VehicleId: v.Id, DeliveryId: d.Id));
            })
            .Count();

        var hasHazMat = data.Deliveries.Any(d => d.Requirements.Any(r => r.Type == "HazMat"));
        Delivery? heaviest = null;
        foreach (var d in pendingByZone.SelectMany(g => g))
        {
            if (heaviest is null || d.WeightKg > heaviest.WeightKg
                || (d.WeightKg == heaviest.WeightKg && d.Id < heaviest.Id))
                heaviest = d;
        }

        return new DispatchResult(inRestricted, assignments, pendingPerZone.Sum(p => p.Count), hasHazMat, heaviest!.Id);
    }
}
