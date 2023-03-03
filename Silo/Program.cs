using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ninject;
using Orleans.Configuration;

namespace Silo;

public class Program
{
    public static async Task Main()
    {
        try
        {
            var silo = await GetSilo();
            Console.WriteLine("Silo has started running, press enter to terminate.");
            Console.ReadLine();
            await silo.StopAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    private static async Task<IHost> GetSilo()
    {
        var host = new HostBuilder()
            .UseOrleans(silo =>
            {
                silo.UseLocalhostClustering()
                    .ConfigureLogging(logging => logging.AddConsole())
                    .Configure<ClusterOptions>(options =>
                    {
                        options.ClusterId = "MapReduce";
                        options.ServiceId = "Barakadax";
                    });
            }).Build();

        await host.StartAsync();

        return host;
    }
}
