using System.Collections.Frozen;
using BenchmarkDotNet.Attributes;
using LinqDeepDive.Domain;
using LinqDeepDive.Domain.Models;

namespace LinqDeepDive.Benchmarks.Benchmarks;

/// <summary>
/// List.Contains contro HashSet.Contains al variare di due parametri espliciti:
/// i veicoli da filtrare e le zone del dataset. La lista di lookup contiene le zone
/// con IsRestricted = true (circa il 30% di ZoneCount): la sua lunghezza e' stampata nel setup.
/// Due famiglie: ToList (come il benchmark originale). Array e Count sulle altre varianti sono stati tolti per tenere la suite sotto un tempo ragionevole e Count (senza materializzare).
/// </summary>
[MemoryDiagnoser]
[GroupBenchmarksBy(BenchmarkDotNet.Configs.BenchmarkLogicalGroupRule.ByCategory, BenchmarkDotNet.Configs.BenchmarkLogicalGroupRule.ByParams)]
[CategoriesColumn]
public class ListVsHashSetBenchmark
{
    [Params(500, 5_000)]
    public int VehicleCount { get; set; }

    [Params(20, 150, 700)]
    public int ZoneCount { get; set; }

    private FleetData _data = null!;
    private List<string> _list = null!;
    private HashSet<string> _set = null!;
    private FrozenSet<string> _frozen = null!;

    /// <summary>Lunghezza reale della lista di lookup, per le tabelle.</summary>
    public int ListLength => _list.Count;

    [GlobalSetup]
    public void Setup()
    {
        _data = DataGenerator.Generate(new FleetOptions(VehicleCount, 200, 100, ZoneCount));
        _list = _data.Zones.Where(z => z.IsRestricted).Select(z => z.Id).ToList();
        _set = _list.ToHashSet();
        _frozen = _list.ToFrozenSet();
        Console.WriteLine($"// ListVsHashSet: veicoli={VehicleCount} zone={ZoneCount} lunghezza lista di lookup={_list.Count}");
    }

    [BenchmarkCategory("ToList"), Benchmark(Baseline = true)]
    public List<Vehicle> ListContains() => _data.Vehicles.Where(v => _list.Contains(v.CurrentZoneId)).ToList();

    [BenchmarkCategory("ToList"), Benchmark]
    public List<Vehicle> HashSetContains() => _data.Vehicles.Where(v => _set.Contains(v.CurrentZoneId)).ToList();

    [BenchmarkCategory("ToList"), Benchmark]
    public List<Vehicle> FrozenSetContains() => _data.Vehicles.Where(v => _frozen.Contains(v.CurrentZoneId)).ToList();

    [BenchmarkCategory("Count"), Benchmark(Baseline = true)]
    public int ListCount() => _data.Vehicles.Count(v => _list.Contains(v.CurrentZoneId));

    [BenchmarkCategory("Count"), Benchmark]
    public int HashSetCount() => _data.Vehicles.Count(v => _set.Contains(v.CurrentZoneId));
}
