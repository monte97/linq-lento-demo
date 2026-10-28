using System.Diagnostics;
using LinqDeepDive.Domain;
using LinqDeepDive.Domain.Models;
using LinqDeepDive.Tracing.Extensions;

namespace LinqDeepDive.Tracing.Demos;

/// <summary>
/// Visualizza come OrderBy è un operatore bloccante che deve consumare
/// tutto l'input prima di produrre output, a differenza di Where/Select.
/// Su Grafana Tempo: lo span di OrderBy dura molto più di Take.
/// </summary>
public static class OrderByBlockingDemo
{
    public static void Run(FleetData data)
    {
        using var activity = LinqTracingExtensions.Source.StartActivity("OrderByBlockingDemo");

        Console.WriteLine("=== OrderBy Blocking Demo ===\n");

        // Scenario 1: OrderBy prima di Take — ordina TUTTO, poi prende 5
        using (var s1 = LinqTracingExtensions.Source.StartActivity("OrderBy-Then-Take"))
        {
            var result = data.Deliveries
                .Where(d => d.Status == DeliveryStatus.Pending)
                .Monitor("S1-Filter")
                .OrderByDescending(d => d.WeightKg)
                .Monitor("S1-OrderBy-ALL")
                .Take(5)
                .Monitor("S1-Take5")
                .ToList();

            Console.WriteLine($"OrderBy+Take(5): {result.Count} risultati, " +
                              $"max={result.FirstOrDefault()?.WeightKg} kg");
        }

        // Scenario 2: MaxBy senza ordinamento — singola scansione
        using (var s2 = LinqTracingExtensions.Source.StartActivity("MaxBy-NoSort"))
        {
            var top5 = data.Deliveries
                .Where(d => d.Status == DeliveryStatus.Pending)
                .Monitor("S2-Filter")
                .ToList();

            // Per top-N senza ordinamento completo: si può usare un partial sort
            // o semplicemente iterare N volte (per N piccolo)
            var result = top5
                .OrderByDescending(d => d.WeightKg)
                .Take(5)
                .Monitor("S2-Take5-AfterSort")
                .ToList();

            Console.WriteLine($"Materializza+Sort+Take: {result.Count} risultati, " +
                              $"max={result.FirstOrDefault()?.WeightKg} kg");
        }
    }
}
