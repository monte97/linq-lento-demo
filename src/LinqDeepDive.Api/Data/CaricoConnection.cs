namespace LinqDeepDive.Api.Data;

public static class CaricoConnection
{
    public const string Default =
        "Host=localhost;Port=5433;Database=linq_carico;Username=linq;Password=linq";

    public static string Resolve(string? fromConfiguration = null) =>
        fromConfiguration
        ?? Environment.GetEnvironmentVariable("CARICO_CONNECTION")
        ?? Default;
}
