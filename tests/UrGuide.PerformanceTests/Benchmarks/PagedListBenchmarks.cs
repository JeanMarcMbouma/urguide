using BenchmarkDotNet.Attributes;
using UrGuide.Core;

namespace UrGuide.PerformanceTests.Benchmarks;

/// <summary>
/// Benchmarks for PagedList pagination using synchronous IEnumerable overloads.
/// The async IQueryable overloads require an EF Core provider and cannot be benchmarked in isolation.
/// </summary>
[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 10)]
public class PagedListBenchmarks
{
    private List<int> _smallDataSet = null!;
    private List<int> _largeDataSet = null!;

    [GlobalSetup]
    public void Setup()
    {
        _smallDataSet = Enumerable.Range(1, 100).ToList();
        _largeDataSet = Enumerable.Range(1, 10000).ToList();
    }

    [Benchmark(Baseline = true)]
    public PagedList<int> Paginate_SmallDataSet()
        => PagedList.Of(_smallDataSet, 1);

    [Benchmark]
    public PagedList<int> Paginate_LargeDataSet()
        => PagedList.Of(_largeDataSet, 1);

    [Benchmark]
    public PagedList<int> Paginate_LargeDataSet_Page50()
        => PagedList.Of(_largeDataSet, 50);

    [Benchmark]
    public PagedList<string> Paginate_WithMapping()
        => PagedList.Of(_largeDataSet, 1, x => x.ToString());
}
