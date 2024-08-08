using BenchmarkDotNet.Attributes;

namespace Benchmarking;

[MemoryDiagnoser]
[ThreadingDiagnoser]
public class WebApiBenchmark
{
    private static readonly HttpClient _httpClient = new()
    {
        BaseAddress = new Uri("http://localhost:5230"),
    };

    [Benchmark]
    public async Task GetFullBenchmark()
    {
        using HttpResponseMessage response = await _httpClient.GetAsync("items");
        response.EnsureSuccessStatusCode();
        var jsonResponse = await response.Content.ReadAsStringAsync();
    }
}