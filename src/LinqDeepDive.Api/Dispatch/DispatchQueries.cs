using LinqDeepDive.Api.Data;
using LinqDeepDive.Domain;
using LinqDeepDive.Domain.Scenarios;
using Microsoft.EntityFrameworkCore;

namespace LinqDeepDive.Api.Dispatch;

/// <summary>
/// Un giro di dispatch sui dati letti da PostgreSQL: le due varianti del Domain
/// (Before e After) lavorano in memoria sulla stessa <see cref="FleetData"/>.
/// Il dispatcher legge tutte e cinque le liste, quindi qui si caricano tutte.
/// </summary>
public static class DispatchQueries
{
    public static async Task<DispatchResult> BeforeAsync(
        CaricoDbContext db,
        CancellationToken cancellationToken = default) =>
        Dispatcher.Before(await LoadAsync(db, cancellationToken));

    public static async Task<DispatchResult> AfterAsync(
        CaricoDbContext db,
        CancellationToken cancellationToken = default) =>
        Dispatcher.After(await LoadAsync(db, cancellationToken));

    private static async Task<FleetData> LoadAsync(CaricoDbContext db, CancellationToken cancellationToken)
    {
        var vehicles = await db.Vehicles.AsNoTracking().ToListAsync(cancellationToken);
        var drivers = await db.Drivers.AsNoTracking().ToListAsync(cancellationToken);
        var deliveries = await db.Deliveries.AsNoTracking().ToListAsync(cancellationToken);
        var zones = await db.Zones.AsNoTracking().ToListAsync(cancellationToken);
        var restrictions = await db.Restrictions.AsNoTracking().ToListAsync(cancellationToken);

        return new FleetData(vehicles, drivers, deliveries, zones, restrictions);
    }
}
