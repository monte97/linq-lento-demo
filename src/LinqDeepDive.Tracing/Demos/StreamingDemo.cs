using System.Diagnostics;
using LinqDeepDive.Domain;
using LinqDeepDive.Domain.Models;
using LinqDeepDive.Tracing.Extensions;

namespace LinqDeepDive.Tracing.Demos;

/// <summary>
/// Mostra come una pipeline LINQ streaming viene valutata lazily.
/// Su Grafana Tempo si vede che ogni stage processa un elemento alla volta.
/// </summary>
public static class StreamingDemo
{
    public static void Run(FleetData data)
    {
        using var activity = LinqTracingExtensions.Source.StartActivity("StreamingDemo");

        Console.WriteLine("=== Streaming Demo ===\n");

        var result = data.Deliveries
            .Where(d => d.Status == DeliveryStatus.Pending)
            .Monitor("Filter-Pending")
            .Where(d => d.WeightKg > 50)
            .Monitor("Filter-Heavy")
            .OrderByDescending(d => d.WeightKg)
            .Monitor("OrderBy-Weight")
            .Take(10)
            .Monitor("Take-10")
            .ToList();

        Console.WriteLine($"Pipeline streaming completata: {result.Count} risultati");
        activity?.SetTag("demo.result_count", result.Count);
    }
}
