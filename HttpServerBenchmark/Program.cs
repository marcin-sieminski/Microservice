using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using BenchmarkDotNet.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace HttpServerBenchmark;

public static class Program
{
    static Program()
    {
        ServicePointManager.ReusePort = true;
        ServicePointManager.DefaultConnectionLimit = 12 * Environment.ProcessorCount;
        ServicePointManager.UseNagleAlgorithm = false;
    }
    public static async Task Main(string[] args)
    {
        await new Benchmark().Run();
        System.Environment.Exit(0);
    }
}

public sealed class MyController : Controller
{
    [Route("/")]
    public async Task<int> PostAsync()
    {
        using var sr = new StreamReader(Request.Body);
        var bodyText = await sr.ReadToEndAsync().ConfigureAwait(false);
        var payload = JsonSerializer.Deserialize<Payload>(bodyText);
        return payload.Value;
    }
}

public struct Payload
{
    [JsonPropertyName("value")]
    public int Value { get; set; }
}

public sealed class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMvcCore().AddApplicationPart(Assembly.GetExecutingAssembly());
    }

    public void Configure(
        IApplicationBuilder app,
        IWebHostEnvironment env)
    {
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}

public class Benchmark
{
    private static readonly HttpClient s_client = new HttpClient() { Timeout = TimeSpan.FromSeconds(1) };

    [Benchmark]
    public async Task Run()
    {
        const int samplesCount = 500;

        var port = 30000 + new Random().Next(10000);
        var server = CreateWebHostBuilder(port).Build();
        using var cts = new CancellationTokenSource();
        _ = Task.Factory.StartNew(() => server.Start(), TaskCreationOptions.LongRunning);
        var sum = 0;
        var api = $"http://localhost:{port}/";
        var tasks = new List<Task<int>>(samplesCount);
        for (var i = 1; i <= samplesCount; i++)
        {
            tasks.Add(SendAsync(api, i));
        }
        foreach (var task in tasks)
        {
            sum += await task.ConfigureAwait(false);
        }

        Console.WriteLine(sum);
        Environment.ExitCode(0);
    }

    private static IHostBuilder CreateWebHostBuilder(int port) =>
        Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.AddJsonFile("appsettings.json",
                    optional: true,
                    reloadOnChange: false);
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.SuppressStatusMessages(true).ConfigureLogging((context, logging) =>
                    {
                        logging.ClearProviders();
                    })
                    .UseKestrel(options =>
                    {
                        options.Limits.MaxRequestBodySize = null;
                        options.ListenLocalhost(port);
                    })
                    .UseStartup<Startup>();
            });

    private static async Task<int> SendAsync(string api, int value)
    {
        var payload = JsonSerializer.Serialize(new Payload { Value = value });
        while (true)
        {
            try
            {
                var content = new StringContent(payload, Encoding.UTF8);
                var response = await s_client.PostAsync(api, content).ConfigureAwait(false);
                return int.Parse(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
            }
            catch
            {
                
            }
        }
    }
}