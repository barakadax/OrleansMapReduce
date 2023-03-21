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
    public async Task ProcessHistogram_GoodInput_ShouldReturnExpected()
    {
        // Arrange
        var fileName = Guid.NewGuid().ToString("N");
        var fileGrain = _host.Cluster.GrainFactory.GetGrain<IFileGrain>(fileName);
        var text = "hey, how are you this day, I ate a banana\nמילים";

        // Act
        var result = await fileGrain.ProcessHistogram(text, fileName);

        // Assert
        Assert.AreEqual(5, result.Count);
        Assert.AreEqual(2, result[1]);
        Assert.AreEqual(6, result[3]);
        Assert.AreEqual(1, result[4]);
        Assert.AreEqual(1, result[5]);
        Assert.AreEqual(1, result[6]);
    }

    [Test]
    public async Task GetResultWithoutProcessing_GoodInput_ShouldReturnExpected()
    {
        // Arrange
        var fileName = Guid.NewGuid().ToString("N");
        var fileGrain = _host.Cluster.GrainFactory.GetGrain<IFileGrain>(fileName);
        var text = "hey, how are you this day, I ate a banana\nמילים";

        // Act
        _ = await fileGrain.ProcessHistogram(text, fileName);
        var result = await fileGrain.GetResultWithoutProcessing();

        // Assert
        Assert.AreEqual(5, result.Count);
        Assert.AreEqual(2, result[1]);
        Assert.AreEqual(6, result[3]);
        Assert.AreEqual(1, result[4]);
        Assert.AreEqual(1, result[5]);
        Assert.AreEqual(1, result[6]);
    }

    [Test]
    [NonParallelizable]
    public void ProcessHistogram_Throws_ShouldGetAnException()
    {
        // Arrange
        var text = Guid.NewGuid().ToString("N");
        var builder = new TestHost<TestSiloConfigurationsThrows>();
        var fileGrain = builder.Cluster.GrainFactory.GetGrain<IFileGrain>(text);

        // Act + Assert
        _ = Assert.ThrowsAsync<Exception>(async () => await fileGrain.ProcessHistogram(text, text));
    }
}
