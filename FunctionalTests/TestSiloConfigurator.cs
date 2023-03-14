using Extensions.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Orleans.TestingHost;
using Silo;
using System.Collections.Concurrent;

namespace FunctionalTests;

public class TestSiloConfigurations : ISiloConfigurator
{
    public void Configure(ISiloBuilder siloBuilder)
    {
        siloBuilder.ConfigureServices(services => {
            services.AddSingleton<ITranslatedWordsDictionary>(new TranslatedWordsDictionary()
            {
                TranslatedWords = new ConcurrentDictionary<string, string>()
            });

            foreach (var binding in DIBinding.Bindings)
            {
                services.AddSingleton(binding.Interface, binding.Class);
            }
        });
    }
}

public class TestHost : IDisposable
{
    public TestCluster Cluster { get; }

    public TestHost()
    {
        var builder = new TestClusterBuilder();
        builder.AddSiloBuilderConfigurator<TestSiloConfigurations>();
        Cluster = builder.Build();
        Cluster.Deploy();
    }

    public void Dispose() => Cluster.StopAllSilos();
}
