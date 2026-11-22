using LinqDeepDive.Api.Data;
using LinqDeepDive.Domain;
using LinqDeepDive.Domain.Models;
using LinqDeepDive.Domain.Scenarios;
using Microsoft.EntityFrameworkCore;

namespace LinqDeepDive.Api.Coverage;

/// <summary>
/// Le tre varianti del report di copertura lette da PostgreSQL:
/// Classic e Modern caricano le righe con EF e girano in memoria (il codice del Domain),
/// Db lascia il lavoro al database con GroupBy + Count e FullJoin tradotti in SQL.
/// </summary>
public static class ZoneCoverageQueries
{
    public static async Task<List<ZoneCoverageRow>> ClassicAsync(
        CaricoDbContext db,
        CancellationToken cancellationToken = default)
    {
        var data = await LoadAsync(db, cancellationToken);
        return ZoneCoverage.Classic(data);
    }

    public static async Task<List<ZoneCoverageRow>> ModernAsync(
        CaricoDbContext db,
        CancellationToken cancellationToken = default)
    {
        var data = await LoadAsync(db, cancellationToken);
        return ZoneCoverage.Modern(data);
    }

    public static IQueryable<ZoneCoverageRow> DbQuery(CaricoDbContext db)
    {
        // EF Core 11 non traduce CountBy: GroupBy + Count produce la stessa SQL.
        var pending = db.Deliveries
            .Where(d => d.Status == DeliveryStatus.Pending)
            .GroupBy(d => d.DestinationZoneId)
            .Select(g => new { ZoneId = g.Key, Count = g.Count() });
        var available = db.Vehicles
            .Where(v => v.IsAvailable)
            .GroupBy(v => v.CurrentZoneId)
            .Select(g => new { ZoneId = g.Key, Count = g.Count() });

        return pending
            .FullJoin(available, p => p.ZoneId, a => a.ZoneId,
                (p, a) => new { ZoneId = p!.ZoneId ?? a!.ZoneId, Pending = p!.Count, Available = a!.Count })
            .OrderBy(x => x.ZoneId)
            .Select(x => new ZoneCoverageRow(x.ZoneId, x.Pending, x.Available));
    }

    public static Task<List<ZoneCoverageRow>> DbAsync(
        CaricoDbContext db,
        CancellationToken cancellationToken = default) =>
        DbQuery(db).ToListAsync(cancellationToken);

    /// <summary>
    /// Il report usa solo consegne e veicoli: le altre tre liste restano vuote,
    /// cosi' l'endpoint non paga il caricamento di tabelle che non legge.
    /// </summary>
    private static async Task<FleetData> LoadAsync(CaricoDbContext db, CancellationToken cancellationToken)
    {
        var vehicles = await db.Vehicles.AsNoTracking().ToListAsync(cancellationToken);
        var deliveries = await db.Deliveries.AsNoTracking().ToListAsync(cancellationToken);

        return new FleetData(vehicles, [], deliveries, [], []);
    }
}
