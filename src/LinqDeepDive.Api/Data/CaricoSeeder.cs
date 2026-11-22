using LinqDeepDive.Domain;
using LinqDeepDive.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace LinqDeepDive.Api.Data;

public static class CaricoSeeder
{
    /// <summary>Genera i dati con DataGenerator, ricrea lo schema e li ricarica. Ritorna i dati generati.</summary>
    public static async Task<FleetData> RecreateAndSeedAsync(
        CaricoDbContext db,
        FleetOptions options,
        CancellationToken cancellationToken = default)
    {
        var data = DataGenerator.Generate(options);

        await db.Database.EnsureDeletedAsync(cancellationToken);
        await db.Database.EnsureCreatedAsync(cancellationToken);

        db.Vehicles.AddRange(data.Vehicles);
        db.Drivers.AddRange(data.Drivers);
        db.Deliveries.AddRange(data.Deliveries);
        db.Zones.AddRange(data.Zones);
        db.Restrictions.AddRange(data.Restrictions);

        await db.SaveChangesAsync(cancellationToken);
        db.ChangeTracker.Clear();

        return data;
    }

    public static async Task<long> CountAsync(
        CaricoDbContext db,
        string table,
        CancellationToken cancellationToken = default)
    {
        var connection = db.Database.GetDbConnection();
        var wasOpen = connection.State == System.Data.ConnectionState.Open;
        if (!wasOpen) await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = $"SELECT count(*) FROM {table}";
        var value = await command.ExecuteScalarAsync(cancellationToken);

        if (!wasOpen) await connection.CloseAsync();
        return Convert.ToInt64(value);
    }
}
