using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using LinqDeepDive.Api;
using LinqDeepDive.Api.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LinqDeepDive.Api.Tests;

[Collection("carico-db")]
public class SeedRoundTripTests
{
    private static readonly string ConnectionString =
        Environment.GetEnvironmentVariable("CARICO_TEST_CONNECTION") ?? CaricoConnection.Default;

    private static readonly JsonSerializerOptions ComparisonOptions = new()
    {
        Converters = { new MicrosecondDateTimeConverter() }
    };

    [Fact]
    public async Task Seed_L_20_zone_matches_in_memory_lists()
    {
        var options = Scales.Get("L") with { Zones = 20 };
        var dbOptions = new DbContextOptionsBuilder<CaricoDbContext>().UseNpgsql(ConnectionString).Options;

        await using var db = new CaricoDbContext(dbOptions);
        var generated = await CaricoSeeder.RecreateAndSeedAsync(db, options);

        Assert.Equal(generated.Vehicles.Count, await CaricoSeeder.CountAsync(db, "vehicles"));
        Assert.Equal(generated.Drivers.Count, await CaricoSeeder.CountAsync(db, "drivers"));
        Assert.Equal(generated.Deliveries.Count, await CaricoSeeder.CountAsync(db, "deliveries"));
        Assert.Equal(generated.Zones.Count, await CaricoSeeder.CountAsync(db, "zones"));
        Assert.Equal(generated.Restrictions.Count, await CaricoSeeder.CountAsync(db, "restrictions"));

        var vehicles = await db.Vehicles.AsNoTracking().OrderBy(v => v.Id).ToListAsync();
        var drivers = await db.Drivers.AsNoTracking().OrderBy(d => d.Id).ToListAsync();
        var deliveries = await db.Deliveries.AsNoTracking().OrderBy(d => d.Id).ToListAsync();
        var zones = await db.Zones.AsNoTracking().OrderBy(z => z.Id).ToListAsync();
        var restrictions = await db.Restrictions.AsNoTracking().OrderBy(r => r.Id).ToListAsync();

        Assert.Equal(Serialize(generated.Vehicles.OrderBy(v => v.Id)), Serialize(vehicles));
        Assert.Equal(Serialize(generated.Drivers.OrderBy(d => d.Id)), Serialize(drivers));
        Assert.Equal(Serialize(generated.Deliveries.OrderBy(d => d.Id)), Serialize(deliveries));
        Assert.Equal(Serialize(generated.Zones.OrderBy(z => z.Id)), Serialize(zones));
        Assert.Equal(Serialize(generated.Restrictions.OrderBy(r => r.Id)), Serialize(restrictions));
    }

    private static string Serialize<T>(IEnumerable<T> items) =>
        JsonSerializer.Serialize(items.ToList(), ComparisonOptions);

    /// <summary>
    /// PostgreSQL conserva i timestamp al microsecondo, DateTime arriva al tick (100 ns):
    /// il confronto tronca entrambi i lati al microsecondo.
    /// </summary>
    private sealed class MicrosecondDateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
            DateTime.Parse(reader.GetString()!, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            // Il timestamp senza fuso conserva l'orologio, non il Kind: si confronta quello.
            var truncated = new DateTime(value.Ticks - (value.Ticks % 10), DateTimeKind.Unspecified);
            writer.WriteStringValue(truncated.ToString("O", CultureInfo.InvariantCulture));
        }
    }
}
