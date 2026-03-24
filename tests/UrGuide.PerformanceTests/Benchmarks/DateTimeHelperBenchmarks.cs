using BenchmarkDotNet.Attributes;
using UrGuide.Services.Helpers;

namespace UrGuide.PerformanceTests.Benchmarks;

[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 10)]
public class DateTimeHelperBenchmarks
{
    private DateTime? _validDate;
    private DateTime? _nullDate;

    [GlobalSetup]
    public void Setup()
    {
        _validDate = new DateTime(2024, 6, 15, 14, 30, 45);
        _nullDate = null;
    }

    [Benchmark(Baseline = true)]
    public string GetDate_WithValue() => DateTimeHelper.GetDate(_validDate);

    [Benchmark]
    public string GetDate_WithNull() => DateTimeHelper.GetDate(_nullDate);

    [Benchmark]
    public string GetTime_WithValue() => DateTimeHelper.GetTime(_validDate);

    [Benchmark]
    public string GetTime_WithNull() => DateTimeHelper.GetTime(_nullDate);

    [Benchmark]
    public string GetDateTime_WithValue() => DateTimeHelper.GetDateTime(_validDate);

    [Benchmark]
    public string GetDateTime_WithNull() => DateTimeHelper.GetDateTime(_nullDate);
}
