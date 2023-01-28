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
            var (name, data) = GetNameAndData();
            await RunGrain(client, name, data);
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

    private static (string, string) GetNameAndData()
    {
        var fileContent = File.ReadAllText("AIW.txt");
        return ("Alice's Adventures in Wonderland", fileContent);
    }

    private static async Task RunGrain(IClusterClient client, string dataName, string dataInput)
    {
        var fileGrain = client.GetGrain<IFileGrain>(dataName);
        var result = await fileGrain.ProcessHistogram(dataInput);
        foreach (var item in result)
        {
            Console.WriteLine($"Word Length: {item.Key} | encountered: {item.Value}");
        }
    }
}
