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
            var client = await GetConnection();
            var mobyDick = GetMobyDick();
            await RunGrain(client, mobyDick.FileName, mobyDick.FileContent);

            var AIW = GetAIW();
            await RunGrain(client, AIW.FileName, AIW.FileContent);
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
            client.UseLocalhostClustering()
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

    private static async Task RunGrain(IClusterClient client, string fileName, string fileContent)
    {
        var fileGrain = client.GetGrain<IFileGrain>(fileName);
        var result = await fileGrain.ProcessHistogram(fileContent, fileName);

        if (result!.IsNullOrEmpty())
        {
            Console.WriteLine("Got empty result, check your text file or if an error occurred on silo side.");
            return;
        }

        Console.WriteLine($"{fileName}:");
        foreach (var item in result!)
        {
            Console.WriteLine($"Word Length: {item.Key} | encountered: {item.Value}");
        }
        Console.WriteLine();
    }
}
