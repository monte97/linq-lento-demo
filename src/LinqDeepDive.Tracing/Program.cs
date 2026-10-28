using System.Diagnostics;
using LinqDeepDive.Domain;
using LinqDeepDive.Tracing.Demos;
using LinqDeepDive.Tracing.Extensions;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

// Setup OpenTelemetry con OTLP exporter verso Grafana LGTM stack (Tempo)
using var tracerProvider = Sdk.CreateTracerProviderBuilder()
    .SetResourceBuilder(ResourceBuilder.CreateDefault()
        .AddService("LinqDeepDive.Tracing"))
    .AddSource(LinqTracingExtensions.Source.Name)
    .AddOtlpExporter(opts =>
    {
        opts.Endpoint = new Uri("http://localhost:4317");
    })
    .Build();

Console.WriteLine("LINQ Deep Dive — Tracing Demo");
Console.WriteLine("Assicurati che lo stack LGTM sia avviato: docker compose up -d");
Console.WriteLine($"Grafana UI: http://localhost:3000 (Explore > Tempo per le trace)\n");

var data = DataGenerator.Generate(FleetSize.Medium);
Console.WriteLine($"Dati generati: {data.Vehicles.Count} veicoli, " +
                  $"{data.Deliveries.Count} consegne\n");

var demos = new Dictionary<string, Action>
{
    ["1"] = () => StreamingDemo.Run(data),
    ["2"] = () => MultipleEnumerationDemo.Run(data),
    ["3"] = () => NestingExplosionDemo.Run(data),
    ["4"] = () => OrderByBlockingDemo.Run(data),
};

while (true)
{
    Console.WriteLine("\nSeleziona demo:");
    Console.WriteLine("  1. Streaming Pipeline");
    Console.WriteLine("  2. Multiple Enumeration");
    Console.WriteLine("  3. Nesting Explosion");
    Console.WriteLine("  4. OrderBy Blocking");
    Console.WriteLine("  A. Esegui tutte");
    Console.WriteLine("  Q. Esci");
    Console.Write("\n> ");

    var input = Console.ReadLine()?.Trim().ToUpper();

    if (input == "Q") break;

    if (input == "A")
    {
        foreach (var demo in demos.Values)
        {
            demo();
            Console.WriteLine();
        }
    }
    else if (input != null && demos.TryGetValue(input, out var action))
    {
        action();
    }
    else
    {
        Console.WriteLine("Scelta non valida.");
    }

    // Forza il flush delle trace
    tracerProvider?.ForceFlush();
    Console.WriteLine("[Trace inviate a Grafana Tempo]");
}
