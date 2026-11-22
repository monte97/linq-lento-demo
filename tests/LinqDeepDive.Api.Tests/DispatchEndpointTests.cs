using LinqDeepDive.Api;
using LinqDeepDive.Api.Data;
using LinqDeepDive.Api.Dispatch;
using LinqDeepDive.Domain.Scenarios;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LinqDeepDive.Api.Tests;

[Collection("carico-db")]
public class DispatchEndpointTests
{
    private static readonly string ConnectionString =
        Environment.GetEnvironmentVariable("CARICO_TEST_CONNECTION") ?? CaricoConnection.Default;

    private static CaricoDbContext CreateDb() =>
        new(new DbContextOptionsBuilder<CaricoDbContext>().UseNpgsql(ConnectionString).Options);

    [Fact]
    public async Task Before_and_after_on_db_match_in_memory_and_make_assignments()
    {
        var options = Scales.Get("M@70") with { Zones = 20 };

        await using var db = CreateDb();
        var generated = await CaricoSeeder.RecreateAndSeedAsync(db, options);

        var expected = Dispatcher.Before(generated);
        var before = await DispatchQueries.BeforeAsync(db);
        var after = await DispatchQueries.AfterAsync(db);

        Assert.True(expected.Assignments > 0, "il profilo M@70 deve produrre assegnamenti");
        Assert.Equal(expected, before);
        Assert.Equal(before, after);
    }
}
