using Extensions.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Orleans.TestingHost;
using Silo;
using System.Collections.Concurrent;
using Translators.Interfaces;

namespace FunctionalTests;

public class TestSiloConfigurations : ISiloConfigurator
{
    public void Configure(ISiloBuilder siloBuilder)
    {
        _ = siloBuilder.ConfigureServices(services =>
        {
            _ = services.AddSingleton<ITranslatedWordsDictionary>(new TranslatedWordsDictionary()
            {
                TranslatedWords = new ConcurrentDictionary<string, string>()
            });

            foreach (var binding in DIBinding.Bindings)
            {
                _ = services.AddSingleton(binding.Interface, binding.Class);
            }
        });
    }
}

public class TestSiloConfigurationsThrows : ISiloConfigurator
{
    public void Configure(ISiloBuilder siloBuilder)
    {
        _ = siloBuilder.ConfigureServices(services =>
        {
            _ = services.AddSingleton<ITranslatedWordsDictionary>(new TranslatedWordsDictionary()
            {
                TranslatedWords = new ConcurrentDictionary<string, string>()
            });

            var mock = Substitute.For<IMicrosoftTranslator>();
            _ = mock.CanTranslate().Throws(new Exception());
            _ = services.AddSingleton<IMicrosoftTranslator>(mock);
        });
    }
}

public class TestHost<T> : IDisposable where T : class, new()
{
    public TestCluster Cluster { get; }

    public TestHost()
    {
        var builder = new TestClusterBuilder();
        _ = builder.AddSiloBuilderConfigurator<T>();
        Cluster = builder.Build();
        Cluster.Deploy();
    }

    public void Dispose() => Cluster.StopAllSilos();
}
