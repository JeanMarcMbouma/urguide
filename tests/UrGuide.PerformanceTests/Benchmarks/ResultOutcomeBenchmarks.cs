using BenchmarkDotNet.Attributes;
using BbQ.Outcome;
using UrGuide.Model.Results;

namespace UrGuide.PerformanceTests.Benchmarks;

[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 10)]
public class ResultOutcomeBenchmarks
{
    [Benchmark(Baseline = true)]
    public Outcome<string> CreateSuccessResult() => Result.Of("success");

    [Benchmark]
    public Outcome<Result.EmptyStruct> CreateEmptyResult() => Result.Empty;

    [Benchmark]
    public Outcome<string> CreateAndAddErrors()
    {
        var result = Result.Of("data");
        return result.WithErrors("error1", "error2");
    }

    [Benchmark]
    public Outcome<string> CombineResults()
    {
        var result1 = Result.Of("data1");
        var result2 = Result.Of("data2");
        return result1.Combine(result2);
    }
}
