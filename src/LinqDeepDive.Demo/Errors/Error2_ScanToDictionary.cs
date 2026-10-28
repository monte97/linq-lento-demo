using System.Diagnostics;
using LinqDeepDive.Domain;
using LinqDeepDive.Domain.Models;

namespace LinqDeepDive.Demo.Errors;

/// <summary>
/// Errore 2: Scansione ripetuta vs GroupBy/ToDictionary
/// Scenario: per ogni zona, contare le consegne pending. Approccio naif: filtra tutte le consegne per ogni zona.
/// </summary>
public static class Error2_ScanToDictionary
{
    public static void RunBefore(FleetData data)
    {
        Console.WriteLine("=== BEFORE: Scansione ripetuta per ogni zona ===\n");

        var sw = Stopwatch.StartNew();

        // O(zone * consegne) — riscansiona tutta la lista per ogni zona
        var pendingPerZone = data.Zones.Select(z => new
        {
            Zone = z.Name,
            PendingCount = data.Deliveries
                .Count(d => d.DestinationZoneId == z.Id && d.Status == DeliveryStatus.Pending)
        }).ToList();

        sw.Stop();

        Console.WriteLine($"Zone analizzate: {pendingPerZone.Count}");
        Console.WriteLine($"Totale pending: {pendingPerZone.Sum(x => x.PendingCount)}");
        Console.WriteLine($"Tempo: {sw.Elapsed.TotalMilliseconds:F3} ms\n");
    }

    public static void RunAfter(FleetData data)
    {
        Console.WriteLine("=== AFTER: GroupBy + ToDictionary (singola scansione) ===\n");

        var sw = Stopwatch.StartNew();

        // O(consegne) — una sola scansione, poi lookup O(1) per zona
        var pendingByZone = data.Deliveries
            .Where(d => d.Status == DeliveryStatus.Pending)
            .GroupBy(d => d.DestinationZoneId)
            .ToDictionary(g => g.Key, g => g.Count());

        var pendingPerZone = data.Zones.Select(z => new
        {
            Zone = z.Name,
            PendingCount = pendingByZone.GetValueOrDefault(z.Id, 0)
        }).ToList();

        sw.Stop();

        Console.WriteLine($"Zone analizzate: {pendingPerZone.Count}");
        Console.WriteLine($"Totale pending: {pendingPerZone.Sum(x => x.PendingCount)}");
        Console.WriteLine($"Tempo: {sw.Elapsed.TotalMilliseconds:F3} ms\n");
    }

    public static void RunComparison(FleetData data)
    {
        Console.WriteLine(">>> Errore 2: Scansione ripetuta vs GroupBy/ToDictionary <<<\n");
        RunBefore(data);
        RunAfter(data);
        Console.WriteLine(new string('-', 50));
    }
}
