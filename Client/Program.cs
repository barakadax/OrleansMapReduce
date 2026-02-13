using Extensions;
using System.Text;
using Extensions.Interfaces;
using GrainInterfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace Client;

[ExcludeFromCodeCoverage]
public class Program
{
    public static async Task Main()
    {
        ILogger logger = null;
        IClusterClient client = null;
        IHost host = null;
        try
        {
            (host, client, logger) = await GetConnection();

            List<(Task<Dictionary<ulong, ulong>>, string)> taskList = [];

            var mobyDick = GetMobyDick();
            taskList.Add(RunGrain(client, mobyDick.FileName, mobyDick.FileContent));

            var AIW = GetAIW();
            taskList.Add(RunGrain(client, AIW.FileName, AIW.FileContent));

            var results = await Task.WhenAll(taskList.Select(async x => (await x.Item1, x.Item2)));

            foreach (var item in results.Where(x => x.Item1.NotNullNorEmpty()))
            {
                ReadResult(item.Item1, item.Item2, logger);
            }

            foreach (var _ in results.Where(x => !x.Item1.NotNullNorEmpty()))
            {
                logger.LogWarning("Text wasn't processed.");
            }
        }
        catch (Exception e)
        {
            if (logger != null)
            {
                logger.LogError(e, "An error occurred in the client.");
            }
            else
            {
                Console.WriteLine($"Critical failure during Client startup: {e.Message}");
            }
        }
        finally
        {
            if (host != null)
            {
                await host.StopAsync();
                host.Dispose();
            }
        }
    }

    private static async Task<(IHost, IClusterClient, ILogger)> GetConnection()
    {
        var host = new HostBuilder()
        .UseOrleansClient(client =>
        {
            _ = client.UseLocalhostClustering()
            .Configure<ClusterOptions>(options =>
            {
                options.ClusterId = "MapReduce";
                options.ServiceId = "Barakadax";
            });
        })
        .ConfigureLogging(logging => logging.AddConsole())
        .Build();

        await host.StartAsync();
        var logger = host.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Client");
        logger.LogInformation("Connected to Silo!");

        return (host, host.Services.GetRequiredService<IClusterClient>(), logger);
    }

    private static InitRecord GetAIW()
    {
        var fileContent = File.ReadAllText("./Client/AIW.txt");
        return new InitRecord() { FileName = "Alice's Adventures in Wonderland", FileContent = fileContent };
    }

    private static InitRecord GetMobyDick()
    {
        var fileContent = File.ReadAllText("./Client/MobyDick.txt");
        return new InitRecord() { FileName = "Moby Dick", FileContent = fileContent };
    }

    private static (Task<Dictionary<ulong, ulong>>, string) RunGrain(IClusterClient client, string fileName, string fileContent)
    {
        var textGrain = client.GetGrain<ITextGrain>(fileName);
        return (textGrain.ProcessText(fileContent, fileName), fileName);
    }

    private static void ReadResult(Dictionary<ulong, ulong> result, string origin, ILogger logger)
    {
        var myStringBuilder = new StringBuilder($"{{Origin}}: {origin}\n");
        var lines = result!.Select(item => $"Word Length: {item.Key} | encountered: {item.Value}");
        myStringBuilder.Append(string.Join("\n", lines));
        myStringBuilder.Append("\n");
        logger.LogInformation(myStringBuilder.ToString());
    }
}
