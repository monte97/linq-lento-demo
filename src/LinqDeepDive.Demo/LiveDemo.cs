using System.Diagnostics;
using LinqDeepDive.Domain;
using LinqDeepDive.Domain.Models;
using LinqDeepDive.Domain.Scenarios;

namespace LinqDeepDive.Demo;

/// <summary>
/// Le due modalita' pensate per il palco: niente BenchmarkDotNet dal vivo.
/// --dispatch <scala> [--prima]: giro di dispatch prima e dopo (con --prima solo il prima, con --dopo solo il dopo), con scaldamento e N passate, tempi con Stopwatch
///   (minimo, mediana, massimo). Dice l'ordine di grandezza, non un numero da citare.
/// --enumeration: quante volte gira la lambda con una query IEnumerable usata tre volte
///   contro una ToList(). Sono conteggi: non dipendono dalla macchina.
/// </summary>
public static class LiveDemo
{
    public static FleetOptions ScaleFor(string name) => name switch
    {
        "S" => new FleetOptions(50, 200, 100),
        "M" => new FleetOptions(200, 2_000, 1_000),
        "L" => new FleetOptions(500, 5_000, 2_000),
        "L17" => new FleetOptions(850, 8_500, 3_400),
        "XL" => new FleetOptions(1_000, 10_000, 4_000),
        "XL12" => new FleetOptions(1_200, 12_000, 4_800),
        "XL15" => new FleetOptions(1_500, 15_000, 6_000),
        "XXL" => new FleetOptions(2_000, 20_000, 8_000),
        // Oltre la XXL, per cercare dove il "dopo" incrocia il budget (usare con --dopo)
        "X4" => new FleetOptions(4_000, 40_000, 16_000),
        "X6" => new FleetOptions(6_000, 60_000, 24_000),
        "X8" => new FleetOptions(8_000, 80_000, 32_000),
        _ => throw new ArgumentException($"scala sconosciuta: {name}")
    };

    public static void Dispatch(FleetOptions opts, bool soloBefore = false, bool soloAfter = false, int passes = 15, int warmup = 3)
    {
        var d = DataGenerator.Generate(opts);
        Console.WriteLine($"Dataset {opts}, runtime {System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription}");
        Console.WriteLine($"Budget di riferimento: 100 ms. Scaldamento {warmup} giri, poi {passes} passate misurate con Stopwatch.");
        for (var i = 0; i < warmup; i++) { if (!soloAfter) Dispatcher.Before(d); if (!soloBefore) Dispatcher.After(d); }

        var before = new double[passes];
        var after = new double[passes];
        DispatchResult? rb = null, ra = null;
        for (var i = 0; i < passes; i++)
        {
            var sw = Stopwatch.StartNew();
            if (!soloAfter)
            {
                rb = Dispatcher.Before(d);
                before[i] = sw.Elapsed.TotalMilliseconds;
            }
            if (soloBefore) continue;
            sw.Restart();
            ra = Dispatcher.After(d);
            after[i] = sw.Elapsed.TotalMilliseconds;
        }
        if (soloAfter) { Report("After ", after); Console.WriteLine("Solo After"); return; }
        Report("Before", before);
        if (soloBefore) { Console.WriteLine($"Solo Before (assegnamenti: {rb!.Assignments})"); return; }
        Report("After ", after);
        Console.WriteLine($"Risultati uguali: {rb == ra} (assegnamenti: {rb!.Assignments})");
    }

    private static void Report(string name, double[] ms)
    {
        var s = ms.OrderBy(x => x).ToArray();
        Console.WriteLine($"{name}: min {s[0]:F2} ms, mediana {s[s.Length / 2]:F2} ms, max {s[^1]:F2} ms");
    }

    public static void Enumeration(FleetOptions opts)
    {
        var d = DataGenerator.Generate(opts);
        Console.WriteLine($"Dataset {opts}: {d.Deliveries.Count} consegne");

        var calls = 0;
        bool IsPendingFragile(Delivery x)
        {
            calls++;
            return x.Status == DeliveryStatus.Pending && x.Requirements.Any(r => r.Type == "Fragile");
        }

        IEnumerable<Delivery> pending = d.Deliveries.Where(IsPendingFragile);
        var n = pending.Count();
        var afterCount = calls;
        var max = pending.Max(x => x.WeightKg);
        var afterMax = calls;
        var first = pending.First().Id;
        Console.WriteLine($"IEnumerable usato tre volte: Count={n} ({afterCount} chiamate), Max={max:F1} ({afterMax - afterCount} chiamate), First={first} ({calls - afterMax} chiamate). Totale {calls} chiamate.");

        calls = 0;
        var list = d.Deliveries.Where(IsPendingFragile).ToList();
        var n2 = list.Count;
        var max2 = list.Max(x => x.WeightKg);
        var first2 = list.First().Id;
        Console.WriteLine($"ToList() una volta, tre letture: Count={n2}, Max={max2:F1}, First={first2}. Totale {calls} chiamate.");
    }
}
