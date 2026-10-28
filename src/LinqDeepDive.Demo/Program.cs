using LinqDeepDive.Demo;
using LinqDeepDive.Domain;
using LinqDeepDive.Demo.Errors;
using LinqDeepDive.Domain.Scenarios;
using Spectre.Console;

// Modalita' non interattiva: dotnet run --project src/LinqDeepDive.Demo -- --all S|M|L|XL|XXL
// oppure --custom <veicoli> <consegne> <vincoli> <zone>. Tempi a colpo singolo: solo orientamento.
// Modalita' per il palco (vedi LiveDemo.cs): --dispatch <scala> [--prima|--dopo] | --enumeration <scala>
if (args.Length >= 2 && args[0] == "--dispatch") { LiveDemo.Dispatch(LiveDemo.ScaleFor(args[1]), soloBefore: args.Contains("--prima"), soloAfter: args.Contains("--dopo")); return; }
if (args.Length >= 2 && args[0] == "--enumeration") { LiveDemo.Enumeration(LiveDemo.ScaleFor(args[1])); return; }

if (args.Length >= 2 && args[0] is "--all" or "--custom")
{
    var opts = args[0] == "--all"
        ? args[1] switch
        {
            "S" => new FleetOptions(50, 200, 100),
            "M" => new FleetOptions(200, 2_000, 1_000),
            "L" => new FleetOptions(500, 5_000, 2_000),
            "XL" => new FleetOptions(1_000, 10_000, 4_000),
            "XXL" => new FleetOptions(2_000, 20_000, 8_000),
            _ => throw new ArgumentException("scala sconosciuta")
        }
        : new FleetOptions(int.Parse(args[1]), int.Parse(args[2]), int.Parse(args[3]), int.Parse(args[4]));
    var d = DataGenerator.Generate(opts);
    Console.WriteLine($"Dataset {opts}: runtime {System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription}");
    Error1_ListContains.RunComparison(d);
    Error2_ScanToDictionary.RunComparison(d);
    Error3_TripleNesting.RunComparison(d);
    Error4_AllocationsEarlyExit.RunComparison(d);
    Console.WriteLine(">>> Scenario end-to-end: Dispatcher.Before vs After <<<");
    for (var pass = 0; pass < 2; pass++)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var rb = Dispatcher.Before(d);
        var tb = sw.Elapsed.TotalMilliseconds;
        sw.Restart();
        var ra = Dispatcher.After(d);
        var ta = sw.Elapsed.TotalMilliseconds;
        Console.WriteLine($"giro {pass + 1}: Before {tb:F3} ms, After {ta:F3} ms, risultati uguali: {rb == ra}");
    }
    return;
}

AnsiConsole.Write(new FigletText("LINQ Deep Dive").Color(Color.Cyan1));
AnsiConsole.MarkupLine("[grey]Demo interattiva — errori comuni e soluzioni[/]\n");

var size = AnsiConsole.Prompt(
    new SelectionPrompt<FleetSize>()
        .Title("Seleziona la [green]dimensione del dataset[/]:")
        .AddChoices(FleetSize.Small, FleetSize.Medium, FleetSize.Large));

AnsiConsole.MarkupLine($"\n[yellow]Generazione dati ({size})...[/]");
var data = DataGenerator.Generate(size);
AnsiConsole.MarkupLine($"[green]Veicoli: {data.Vehicles.Count}, Consegne: {data.Deliveries.Count}, " +
                       $"Vincoli: {data.Restrictions.Count}[/]\n");

while (true)
{
    var choice = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("Quale [green]demo[/] vuoi eseguire?")
            .AddChoices(
                "1. List.Contains vs HashSet",
                "2. Scansione ripetuta vs GroupBy/ToDictionary",
                "3. Triplo nesting vs Pre-indicizzazione",
                "4. Allocazioni inutili vs Lazy + Early exit",
                "5. Scenario end-to-end (dispatcher)",
                "Tutti i confronti",
                "Esci"));

    Console.WriteLine();

    switch (choice[0])
    {
        case '1':
            Error1_ListContains.RunComparison(data);
            break;
        case '2':
            Error2_ScanToDictionary.RunComparison(data);
            break;
        case '3':
            Error3_TripleNesting.RunComparison(data);
            break;
        case '4':
            Error4_AllocationsEarlyExit.RunComparison(data);
            break;
        case '5':
            RunDispatch(data);
            break;
        case 'T':
            Error1_ListContains.RunComparison(data);
            Error2_ScanToDictionary.RunComparison(data);
            Error3_TripleNesting.RunComparison(data);
            Error4_AllocationsEarlyExit.RunComparison(data);
            break;
        case 'E':
            return;
    }

    Console.WriteLine();
}

static void RunDispatch(FleetData d)
{
    var sw = System.Diagnostics.Stopwatch.StartNew();
    var before = Dispatcher.Before(d);
    Console.WriteLine($"Before: {sw.Elapsed.TotalMilliseconds:F3} ms");
    sw.Restart();
    var after = Dispatcher.After(d);
    Console.WriteLine($"After:  {sw.Elapsed.TotalMilliseconds:F3} ms (risultati uguali: {before == after})");
}
