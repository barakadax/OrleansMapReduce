using Extensions;
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
        try
        {
            var taskList = new List<(Task<Dictionary<ulong, ulong>>, string)>();
            var client = await GetConnection();
            var mobyDick = GetMobyDick();
            taskList.Add(RunGrain(client, mobyDick.FileName, mobyDick.FileContent));

            var AIW = GetAIW();
            taskList.Add(RunGrain(client, AIW.FileName, AIW.FileContent));

            var results = await Task.WhenAll(taskList.Select(async x => (await x.Item1, x.Item2)));

            foreach (var result in results)
            {
                if (result.Item1.NotNullNorEmpty())
                {
                    ReadResult(result.Item1, result.Item2);
                }
                else
                {
                    Console.WriteLine("Text wasn\'t processed.");
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    private static async Task<IClusterClient> GetConnection()
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
        Console.WriteLine("Connected to Silo!");
        Console.WriteLine();

        return host.Services.GetRequiredService<IClusterClient>();
    }

    private static InitRecord GetAIW()
    {
        var fileContent = File.ReadAllText("AIW.txt");
        return new InitRecord() { FileName = "Alice's Adventures in Wonderland", FileContent = fileContent };
    }

    private static InitRecord GetMobyDick()
    {
        var fileContent = File.ReadAllText("MobyDick.txt");
        return new InitRecord() { FileName = "Moby Dick", FileContent = fileContent };
    }

    private static (Task<Dictionary<ulong, ulong>>, string) RunGrain(IClusterClient client, string fileName, string fileContent)
    {
        var fileGrain = client.GetGrain<IFileGrain>(fileName);
        return (fileGrain.ProcessHistogram(fileContent, fileName), fileName);
    }

    private static void ReadResult(Dictionary<ulong, ulong> result, string origin)
    {
        Console.WriteLine($"{origin}:");
        foreach (var item in result!)
        {
            Console.WriteLine($"Word Length: {item.Key} | encountered: {item.Value}");
        }
        Console.WriteLine();
    }
}
