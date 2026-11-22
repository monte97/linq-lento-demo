using LinqDeepDive.Api;
using LinqDeepDive.Api.Coverage;
using LinqDeepDive.Api.Data;
using LinqDeepDive.Api.Dispatch;
using LinqDeepDive.Domain;
using Microsoft.EntityFrameworkCore;

if (args.Length > 0 && args[0] == "--seed")
{
    var options = ParseSeed(args);

    var dbOptions = new DbContextOptionsBuilder<CaricoDbContext>()
        .UseNpgsql(CaricoConnection.Resolve())
        .Options;

    await using var db = new CaricoDbContext(dbOptions);
    var data = await CaricoSeeder.RecreateAndSeedAsync(db, options);

    Console.WriteLine($"seed {options}");
    Console.WriteLine($"  vehicles:     {data.Vehicles.Count}");
    Console.WriteLine($"  drivers:      {data.Drivers.Count}");
    Console.WriteLine($"  deliveries:   {data.Deliveries.Count}");
    Console.WriteLine($"  zones:        {data.Zones.Count}");
    Console.WriteLine($"  restrictions: {data.Restrictions.Count}");
    return;
}

var builder = WebApplication.CreateBuilder(args);

// Sotto carico i log a livello Information (una riga per richiesta e per comando SQL) finiscono
// nella misura: il 22/11 un giro a scala L ne ha scritti 475 MB. Restano avvisi, errori e avvio.
builder.Logging.SetMinimumLevel(LogLevel.Warning);
builder.Logging.AddFilter("Microsoft.Hosting.Lifetime", LogLevel.Information);

builder.Services.AddDbContext<CaricoDbContext>(options =>
    options.UseNpgsql(CaricoConnection.Resolve(builder.Configuration.GetConnectionString("Carico"))));

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapGet("/coverage/classic", async (CaricoDbContext db, CancellationToken cancellationToken) =>
    Results.Ok(await ZoneCoverageQueries.ClassicAsync(db, cancellationToken)));

app.MapGet("/coverage/modern", async (CaricoDbContext db, CancellationToken cancellationToken) =>
    Results.Ok(await ZoneCoverageQueries.ModernAsync(db, cancellationToken)));

app.MapGet("/coverage/db", async (CaricoDbContext db, CancellationToken cancellationToken) =>
    Results.Ok(await ZoneCoverageQueries.DbAsync(db, cancellationToken)));

app.MapGet("/dispatch/before", async (CaricoDbContext db, CancellationToken cancellationToken) =>
    Results.Ok(await DispatchQueries.BeforeAsync(db, cancellationToken)));

app.MapGet("/dispatch/after", async (CaricoDbContext db, CancellationToken cancellationToken) =>
    Results.Ok(await DispatchQueries.AfterAsync(db, cancellationToken)));

app.Run();

static FleetOptions ParseSeed(string[] args)
{
    if (args.Length < 3)
    {
        throw new ArgumentException("Uso: --seed <scala> <zone> (es. --seed L 20)");
    }

    var scale = args[1];
    var baseName = scale.Split('@')[0];
    if (!Scales.Map.ContainsKey(baseName))
    {
        throw new ArgumentException($"Scala sconosciuta '{baseName}'. Disponibili: {string.Join(", ", Scales.Map.Keys)}");
    }

    if (!int.TryParse(args[2], out var zones) || zones < 1)
    {
        throw new ArgumentException($"Numero di zone non valido: '{args[2]}'");
    }

    return Scales.Get(scale) with { Zones = zones };
}
