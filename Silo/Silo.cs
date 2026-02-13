using Extensions.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans.Configuration;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Silo;

[ExcludeFromCodeCoverage]
public static class Silo
{
    public static async Task RunSilo()
    {
        IHost host = null;
        try
        {
            host = await GetSilo();
            var logger = host.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Silo");
            logger.LogInformation("Silo has started running, press enter to terminate.");
            _ = Console.ReadLine();
            await host.StopAsync();
        }
        catch (Exception e)
        {
            if (host != null)
            {
                var logger = host.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Silo");
                logger.LogError(e, "An error occurred while running the silo.");
            }
            else
            {
                // Fallback if host failed to start
                Console.WriteLine($"Critical failure during Silo startup: {e.Message}");
            }
        }
    }

    private static async Task<IHost> GetSilo()
    {
        var wordsTranslateDict = new TranslatedWordsDictionary()
        {
            TranslatedWords = new ConcurrentDictionary<string, string>()
        };

        var host = new HostBuilder()
            .UseOrleans(silo =>
            {
                _ = silo.UseLocalhostClustering()
                    .ConfigureLogging(logging => logging.AddConsole())
                    .Configure<ClusterOptions>(options =>
                    {
                        options.ClusterId = "MapReduce";
                        options.ServiceId = "Barakadax";
                    })
                    .ConfigureServices(services =>
                    {
                        _ = services.AddSingleton<ITranslatedWordsDictionary>(wordsTranslateDict);

                        foreach (var binding in DIBinding.Bindings)
                        {
                            _ = services.AddSingleton(binding.Interface, binding.Class);
                        }
                    });
            }).Build();

        await host.StartAsync();

        return host;
    }
}
