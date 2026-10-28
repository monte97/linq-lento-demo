using BenchmarkDotNet.Attributes;
using LinqDeepDive.Domain;
using LinqDeepDive.Domain.Models;

namespace LinqDeepDive.Benchmarks.Benchmarks;

[MemoryDiagnoser]
[GroupBenchmarksBy(BenchmarkDotNet.Configs.BenchmarkLogicalGroupRule.ByCategory, BenchmarkDotNet.Configs.BenchmarkLogicalGroupRule.ByParams)]
[CategoriesColumn]
public class AllocationsVsForeachBenchmark
{
    [Params("S", "M", "L", "XL")]
    public string Scale { get; set; } = "L";

    private FleetData _data = null!;

    [GlobalSetup]
    public void Setup()
    {
        _data = DataGenerator.Generate(Scales.Get(Scale));
    }

    [BenchmarkCategory("Pipeline"), Benchmark(Baseline = true)]
    public List<Delivery> IntermediateToList()
    {
        return _data.Deliveries
            .Where(d => d.WeightKg > 100)
            .ToList()
            .Where(d => d.Status == DeliveryStatus.Pending)
            .ToList()
            .OrderByDescending(d => d.WeightKg)
            .ToList();
    }

    [BenchmarkCategory("Pipeline"), Benchmark]
    public List<Delivery> LazyPipeline()
    {
        return _data.Deliveries
            .Where(d => d.WeightKg > 100)
            .Where(d => d.Status == DeliveryStatus.Pending)
            .OrderByDescending(d => d.WeightKg)
            .ToList();
    }

    [BenchmarkCategory("Exists"), Benchmark(Baseline = true)]
    public bool CountGreaterZero()
    {
        return _data.Deliveries
            .Where(d => d.Requirements.Any(r => r.Type == "HazMat"))
            .Count() > 0;
    }

    [BenchmarkCategory("Exists"), Benchmark]
    public bool AnyEarlyExit()
    {
        return _data.Deliveries
            .Any(d => d.Requirements.Any(r => r.Type == "HazMat"));
    }

    [BenchmarkCategory("Max"), Benchmark(Baseline = true)]
    public Delivery OrderByFirst()
    {
        return _data.Deliveries
            .Where(d => d.Status == DeliveryStatus.Pending)
            .OrderByDescending(d => d.WeightKg)
            .First();
    }

    [BenchmarkCategory("Max"), Benchmark]
    public Delivery MaxByLinear()
    {
        return _data.Deliveries
            .Where(d => d.Status == DeliveryStatus.Pending)
            .MaxBy(d => d.WeightKg)!;
    }
}
