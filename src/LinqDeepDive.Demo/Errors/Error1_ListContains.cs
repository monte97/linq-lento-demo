using System.Diagnostics;
using LinqDeepDive.Domain;
using LinqDeepDive.Domain.Models;

namespace LinqDeepDive.Demo.Errors;

/// <summary>
/// Errore 1: List.Contains() in un ciclo — O(N*M) → O(N) con HashSet
/// Scenario: trovare tutti i veicoli la cui zona corrente è in una lista di zone ristrette.
/// </summary>
public static class Error1_ListContains
{
    public static void RunBefore(FleetData data)
    {
        Console.WriteLine("=== BEFORE: List.Contains() in Where ===\n");

        // Zona ristrette come List<string>
        var restrictedZoneIds = data.Zones
            .Where(z => z.IsRestricted)
            .Select(z => z.Id)
            .ToList();

        var sw = Stopwatch.StartNew();

        // O(veicoli * zoneRistrette) — scansione lineare per ogni Contains
        var vehiclesInRestrictedZones = data.Vehicles
            .Where(v => restrictedZoneIds.Contains(v.CurrentZoneId))
            .ToList();

        sw.Stop();

        Console.WriteLine($"Zone ristrette: {restrictedZoneIds.Count}");
        Console.WriteLine($"Veicoli trovati: {vehiclesInRestrictedZones.Count}");
        Console.WriteLine($"Tempo: {sw.Elapsed.TotalMilliseconds:F3} ms\n");
    }

    public static void RunAfter(FleetData data)
    {
        Console.WriteLine("=== AFTER: HashSet.Contains() in Where ===\n");

        // Stesse zone, ma come HashSet — lookup O(1)
        var restrictedZoneIds = data.Zones
            .Where(z => z.IsRestricted)
            .Select(z => z.Id)
            .ToHashSet();

        var sw = Stopwatch.StartNew();

        // O(veicoli) — ogni Contains è O(1)
        var vehiclesInRestrictedZones = data.Vehicles
            .Where(v => restrictedZoneIds.Contains(v.CurrentZoneId))
            .ToList();

        sw.Stop();

        Console.WriteLine($"Zone ristrette: {restrictedZoneIds.Count}");
        Console.WriteLine($"Veicoli trovati: {vehiclesInRestrictedZones.Count}");
        Console.WriteLine($"Tempo: {sw.Elapsed.TotalMilliseconds:F3} ms\n");
    }

    public static void RunComparison(FleetData data)
    {
        Console.WriteLine(">>> Errore 1: List.Contains vs HashSet.Contains <<<\n");
        RunBefore(data);
        RunAfter(data);
        Console.WriteLine(new string('-', 50));
    }
}
