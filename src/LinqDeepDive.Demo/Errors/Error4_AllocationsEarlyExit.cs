using System.Diagnostics;
using LinqDeepDive.Domain;
using LinqDeepDive.Domain.Models;

namespace LinqDeepDive.Demo.Errors;

/// <summary>
/// Errore 4: Allocazioni inutili e mancato early exit
/// Scenario a: ToList() intermedi non necessari → lazy evaluation
/// Scenario b: Any() vs Count() > 0 per early exit
/// </summary>
public static class Error4_AllocationsEarlyExit
{
    public static void RunBefore(FleetData data)
    {
        Console.WriteLine("=== BEFORE: ToList() intermedi + Count() > 0 ===\n");

        var sw = Stopwatch.StartNew();

        // Materializza tutto con ToList() anche se servono solo pochi elementi
        var heavyDeliveries = data.Deliveries
            .Where(d => d.WeightKg > 100)
            .ToList()  // allocazione inutile!
            .Where(d => d.Status == DeliveryStatus.Pending)
            .ToList()  // altra allocazione!
            .OrderByDescending(d => d.WeightKg)
            .ToList(); // terza allocazione!

        // Count() scorre tutta la collezione anche se basta sapere se esiste almeno 1
        var hasHazMat = data.Deliveries
            .Where(d => d.Requirements.Any(r => r.Type == "HazMat"))
            .Count() > 0;

        // First() dopo OrderBy: ordina tutto per prendere solo il primo
        var heaviestPending = data.Deliveries
            .Where(d => d.Status == DeliveryStatus.Pending)
            .OrderByDescending(d => d.WeightKg)
            .ToList()
            .First();

        sw.Stop();

        Console.WriteLine($"Consegne pesanti pending: {heavyDeliveries.Count}");
        Console.WriteLine($"Esistono HazMat: {hasHazMat}");
        Console.WriteLine($"Piu' pesante pending: {heaviestPending.WeightKg} kg (ID: {heaviestPending.Id})");
        Console.WriteLine($"Tempo: {sw.Elapsed.TotalMilliseconds:F3} ms\n");
    }

    public static void RunAfter(FleetData data)
    {
        Console.WriteLine("=== AFTER: Lazy evaluation + Any() + MaxBy() ===\n");

        var sw = Stopwatch.StartNew();

        // Pipeline lazy — nessuna allocazione intermedia
        var heavyDeliveries = data.Deliveries
            .Where(d => d.WeightKg > 100)
            .Where(d => d.Status == DeliveryStatus.Pending)
            .OrderByDescending(d => d.WeightKg)
            .ToList(); // materializza solo alla fine

        // Any() esce al primo match
        var hasHazMat = data.Deliveries
            .Any(d => d.Requirements.Any(r => r.Type == "HazMat"));

        // MaxBy() — singola scansione senza ordinamento completo O(n log n)
        var heaviestPending = data.Deliveries
            .Where(d => d.Status == DeliveryStatus.Pending)
            .MaxBy(d => d.WeightKg)!;

        sw.Stop();

        Console.WriteLine($"Consegne pesanti pending: {heavyDeliveries.Count}");
        Console.WriteLine($"Esistono HazMat: {hasHazMat}");
        Console.WriteLine($"Piu' pesante pending: {heaviestPending.WeightKg} kg (ID: {heaviestPending.Id})");
        Console.WriteLine($"Tempo: {sw.Elapsed.TotalMilliseconds:F3} ms\n");
    }

    public static void RunComparison(FleetData data)
    {
        Console.WriteLine(">>> Errore 4: Allocazioni inutili vs Lazy + Early exit <<<\n");
        RunBefore(data);
        RunAfter(data);
        Console.WriteLine(new string('-', 50));
    }
}
