﻿using Extensions.Interfaces;
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
        try
        {
            var silo = await GetSilo();
            Console.WriteLine("Silo has started running, press enter to terminate.");
            _ = Console.ReadLine();
            await silo.StopAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
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
