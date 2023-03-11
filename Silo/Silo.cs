using Extensions;
using Extensions.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans.Configuration;

namespace Silo;

public static class Silo
{
    public static async Task RunSilo()
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
        var kernel = Injection.Host.BindInjection();

        var host = new HostBuilder()
            .UseOrleans(silo =>
            {
                silo.UseLocalhostClustering()
                    .ConfigureLogging(logging => logging.AddConsole())
                    .Configure<ClusterOptions>(options =>
                    {
                        options.ClusterId = "MapReduce";
                        options.ServiceId = "Barakadax";
                    })
                    .ConfigureServices(services =>
                    {
                        services.AddSingleton<IMicrosoftTranslator, MicrosoftTranslator>();  // change to use kernel
                    });
            }).Build();

        await host.StartAsync();

        return host;
    }
}
