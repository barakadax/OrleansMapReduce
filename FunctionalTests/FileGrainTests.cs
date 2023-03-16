using GrainInterfaces;
using NUnit.Framework;

namespace FunctionalTests;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class FileGrainTests
{
    private TestHost<TestSiloConfigurations> _host;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _host = new TestHost<TestSiloConfigurations>();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _host.Dispose();
    }

    [Test]
    [NonParallelizable]
    public void RunProcessHistogram_Throws_ShouldGetAnException()
    {
        // Arrange
        var text = Guid.NewGuid().ToString("N");
        var builder = new TestHost<TestSiloConfigurationsThrows>();
        var fileGrain = builder.Cluster.GrainFactory.GetGrain<IFileGrain>(text);

        // Act + Assert
        Assert.ThrowsAsync<Exception>(async () => await fileGrain.ProcessHistogram(text, text));
    }
}
