using LinqDeepDive.Domain;

namespace LinqDeepDive.Api;

/// <summary>
/// Copia dei nomi di scala dei benchmark (src/LinqDeepDive.Benchmarks/Scales.cs).
/// Stessi numeri, cosi' i conteggi del carico coincidono con quelli misurati in memoria.
/// </summary>
public static class Scales
{
    public static readonly IReadOnlyDictionary<string, FleetOptions> Map = new Dictionary<string, FleetOptions>
    {
        ["S"]   = new(50, 200, 100),
        ["M"]   = new(200, 2_000, 1_000),
        ["L"]   = new(500, 5_000, 2_000),
        ["XL"]  = new(1_000, 10_000, 4_000),
        ["XXL"] = new(2_000, 20_000, 8_000),
        ["L15"]  = new(750, 7_500, 3_000),
        ["L17"]  = new(850, 8_500, 3_400),
        ["XL12"] = new(1_200, 12_000, 4_800),
        ["XL15"] = new(1_500, 15_000, 6_000),
    };

    /// <summary>Come Map, con suffisso facoltativo "@<percentuale>" (es. "XL12@70") per il profilo con assegnamenti.</summary>
    public static FleetOptions Get(string name)
    {
        var at = name.IndexOf('@');
        return at < 0 ? Map[name] : Map[name[..at]] with { BlockedShare = int.Parse(name[(at + 1)..]) / 100.0 };
    }
}
