using LinqDeepDive.Domain.Models;

namespace LinqDeepDive.Domain.Scenarios;

/// <summary>Una riga del report di copertura: consegne in attesa e veicoli liberi nella stessa zona.</summary>
public record ZoneCoverageRow(string ZoneId, int PendingDeliveries, int AvailableVehicles);

/// <summary>
/// Report di copertura delle zone: per ogni zona che ha consegne in attesa o veicoli liberi,
/// quante consegne aspettano e quanti veicoli ci sono. Servono entrambi i lati, anche quando
/// uno manca (zone scoperte: consegne senza veicoli; zone ferme: veicoli senza consegne),
/// quindi e' un full outer join fra i due conteggi per zona.
///
/// Classic: come si scriveva prima di .NET 9, con GroupBy per contare e il full join composto
/// a mano (left join con GroupJoin e DefaultIfEmpty, piu' le righe del solo lato destro).
/// Modern: CountBy (.NET 9) e FullJoin (.NET 11). Sintetico, non e' il codice di un cliente.
/// </summary>
public static class ZoneCoverage
{
    public static List<ZoneCoverageRow> Classic(FleetData data)
    {
        var pending = data.Deliveries
            .Where(d => d.Status == DeliveryStatus.Pending)
            .GroupBy(d => d.DestinationZoneId)
            .Select(g => new { ZoneId = g.Key, Count = g.Count() })
            .ToList();
        var available = data.Vehicles
            .Where(v => v.IsAvailable)
            .GroupBy(v => v.CurrentZoneId)
            .Select(g => new { ZoneId = g.Key, Count = g.Count() })
            .ToList();

        // Lato sinistro con eventuale destro (left join)
        var left = pending
            .GroupJoin(available, p => p.ZoneId, a => a.ZoneId, (p, matches) => new { p, matches })
            .SelectMany(x => x.matches.DefaultIfEmpty(),
                (x, a) => new ZoneCoverageRow(x.p.ZoneId, x.p.Count, a?.Count ?? 0));
        // Solo lato destro: zone con veicoli liberi e nessuna consegna in attesa
        var rightOnly = available
            .Where(a => !pending.Any(p => p.ZoneId == a.ZoneId))
            .Select(a => new ZoneCoverageRow(a.ZoneId, 0, a.Count));

        return left.Concat(rightOnly).OrderBy(r => r.ZoneId, StringComparer.Ordinal).ToList();
    }

    /// <summary>
    /// Come Classic, ma le zone del solo lato destro si trovano con un HashSet delle chiavi
    /// invece di riscorrere la lista con Any: separa il merito di FullJoin da quello di non
    /// essere quadratici nel numero di zone.
    /// </summary>
    public static List<ZoneCoverageRow> ClassicHashSet(FleetData data)
    {
        var pending = data.Deliveries
            .Where(d => d.Status == DeliveryStatus.Pending)
            .GroupBy(d => d.DestinationZoneId)
            .Select(g => new { ZoneId = g.Key, Count = g.Count() })
            .ToList();
        var available = data.Vehicles
            .Where(v => v.IsAvailable)
            .GroupBy(v => v.CurrentZoneId)
            .Select(g => new { ZoneId = g.Key, Count = g.Count() })
            .ToList();

        var left = pending
            .GroupJoin(available, p => p.ZoneId, a => a.ZoneId, (p, matches) => new { p, matches })
            .SelectMany(x => x.matches.DefaultIfEmpty(),
                (x, a) => new ZoneCoverageRow(x.p.ZoneId, x.p.Count, a?.Count ?? 0));
        var pendingZones = new HashSet<string>(pending.Select(p => p.ZoneId));
        var rightOnly = available
            .Where(a => !pendingZones.Contains(a.ZoneId))
            .Select(a => new ZoneCoverageRow(a.ZoneId, 0, a.Count));

        return left.Concat(rightOnly).OrderBy(r => r.ZoneId, StringComparer.Ordinal).ToList();
    }

#if NET11_0_OR_GREATER
    public static List<ZoneCoverageRow> Modern(FleetData data)
    {
        var pending = data.Deliveries
            .Where(d => d.Status == DeliveryStatus.Pending)
            .CountBy(d => d.DestinationZoneId);
        var available = data.Vehicles
            .Where(v => v.IsAvailable)
            .CountBy(v => v.CurrentZoneId);

        return pending
            .FullJoin(available, p => p.Key, a => a.Key,
                (p, a) => new ZoneCoverageRow(p.Key ?? a.Key, p.Value, a.Value))
            .OrderBy(r => r.ZoneId, StringComparer.Ordinal)
            .ToList();
    }
#endif
}
