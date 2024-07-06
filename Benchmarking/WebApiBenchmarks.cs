using BenchmarkDotNet.Attributes;

namespace Benchmarking;

[MemoryDiagnoser]
[ThreadingDiagnoser]
public class WebApiBenchmarks
{
    private static HttpClient httpClient = new()
    {
        BaseAddress = new Uri("http://localhost:5198"),
    };

    [Benchmark]
    public async Task GetBenchmark()
    {
        using HttpResponseMessage response = await httpClient.GetAsync("item");
        response.EnsureSuccessStatusCode();
        var jsonResponse = await response.Content.ReadAsStringAsync();
    }
}