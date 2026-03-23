using BenchmarkDotNet.Running;
using UrGuide.PerformanceTests.Benchmarks;

// Run all benchmarks in this assembly
BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
