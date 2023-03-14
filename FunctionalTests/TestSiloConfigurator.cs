using Microsoft.Extensions.DependencyInjection;
using Orleans.TestingHost;
using Silo;

namespace FunctionalTests;

public class TestSiloConfigurations : ISiloConfigurator
{
    public void Configure(ISiloBuilder siloBuilder)
    {
        siloBuilder.ConfigureServices(services => {
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
