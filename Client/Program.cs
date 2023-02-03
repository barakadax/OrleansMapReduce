using Extensions;
using GrainInterfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans.Configuration;

namespace Client;

public class Program
{
    private static async Task Main()
    {
        try
        {
            var client = await GetConnection();
            var record = GetNameAndData();
            await RunGrain(client, record.FileName, record.FileContent);
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

        return host.Services.GetRequiredService<IClusterClient>();
    }

    private static InitRecord GetNameAndData()
    {
        var fileContent = File.ReadAllText("AIW.txt");
        return new InitRecord() { FileName = "Alice's Adventures in Wonderland", FileContent = fileContent };
    }

    private static async Task RunGrain(IClusterClient client, string dataName, string dataInput)
    {
        var fileGrain = client.GetGrain<IFileGrain>(dataName);
        var result = await fileGrain.ProcessHistogram(dataInput);

        if (result!.IsNullOrEmpty())
        {
            Console.WriteLine("Got empty result, check your text file or if an error occurred on silo side.");
            return;
        }

        foreach (var item in result!)
        {
            Console.WriteLine($"Word Length: {item.Key} | encountered: {item.Value}");
        }
    }
}
