using BenchmarkDotNet.Running;
using Benchmarking;

BenchmarkRunner.Run<HttpServerBenchmark.Benchmark>();
BenchmarkRunner.Run<WebApiBenchmark>();
