using System.Diagnostics;
using LinqDeepDive.Domain;
using LinqDeepDive.Domain.Models;
using LinqDeepDive.Tracing.Extensions;

namespace LinqDeepDive.Tracing.Demos;

/// <summary>
/// Mostra il problema della multiple enumeration:
/// la stessa query IEnumerable viene valutata più volte.
/// Su Grafana Tempo si vedono span duplicati per la stessa pipeline.
/// </summary>
public static class MultipleEnumerationDemo
{
    public static void Run(FleetData data)
    {
        using var activity = LinqTracingExtensions.Source.StartActivity("MultipleEnumerationDemo");

        Console.WriteLine("=== Multiple Enumeration Demo ===\n");

        // Query non materializzata — ogni uso la riesegue
        var pendingDeliveries = data.Deliveries
            .Where(d => d.Status == DeliveryStatus.Pending)
            .Monitor("Pending-Query");

        // Prima enumerazione
        Console.WriteLine($"Count: {pendingDeliveries.Count()}");     // enumera tutto
        // Seconda enumerazione
        Console.WriteLine($"Max peso: {pendingDeliveries.Max(d => d.WeightKg)} kg"); // enumera tutto di nuovo
        // Terza enumerazione
        Console.WriteLine($"Prima consegna: ID {pendingDeliveries.First().Id}"); // enumera (almeno 1)

        Console.WriteLine("\n--- Versione corretta (materializzata) ---\n");

        var materializedPending = data.Deliveries
            .Where(d => d.Status == DeliveryStatus.Pending)
            .Monitor("Pending-Materialized")
            .ToList();  // una sola enumerazione

        Console.WriteLine($"Count: {materializedPending.Count}");
        Console.WriteLine($"Max peso: {materializedPending.Max(d => d.WeightKg)} kg");
        Console.WriteLine($"Prima consegna: ID {materializedPending.First().Id}");

        activity?.SetTag("demo.multiple_enum_count", 3);
    }
}
