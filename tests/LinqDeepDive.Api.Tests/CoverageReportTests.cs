using LinqDeepDive.Api;
using LinqDeepDive.Api.Coverage;
using LinqDeepDive.Api.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace LinqDeepDive.Api.Tests;

[Collection("carico-db")]
public class CoverageReportTests
{
    private readonly ITestOutputHelper _output;

    public CoverageReportTests(ITestOutputHelper output) => _output = output;

    private static readonly string ConnectionString =
        Environment.GetEnvironmentVariable("CARICO_TEST_CONNECTION") ?? CaricoConnection.Default;

    private static CaricoDbContext CreateDb() =>
        new(new DbContextOptionsBuilder<CaricoDbContext>().UseNpgsql(ConnectionString).Options);

    [Fact]
    public async Task Classic_modern_and_db_agree_row_by_row()
    {
        await using var db = CreateDb();
        await CaricoSeeder.RecreateAndSeedAsync(db, Scales.Get("S") with { Zones = 5 });

        var classic = await ZoneCoverageQueries.ClassicAsync(db);
        var modern = await ZoneCoverageQueries.ModernAsync(db);
        var fromDb = await ZoneCoverageQueries.DbAsync(db);

        Assert.NotEmpty(classic);
        Assert.Equal(classic, modern);
        Assert.Equal(classic, fromDb);
    }

    [Fact]
    public async Task Db_query_is_translated_to_a_full_join()
    {
        await using var db = CreateDb();
        await CaricoSeeder.RecreateAndSeedAsync(db, Scales.Get("S") with { Zones = 5 });

        var sql = ZoneCoverageQueries.DbQuery(db).ToQueryString();
        _output.WriteLine(sql);

        Assert.Contains("FULL JOIN", sql, StringComparison.OrdinalIgnoreCase);
    }
}
