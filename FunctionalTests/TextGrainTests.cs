using GrainInterfaces;
using NUnit.Framework;

namespace FunctionalTests;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class TextGrainTests
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
    public async Task ProcessHistogram_InputWithNoneAlphabeticalWords_ShouldReturnExpected()
    {
        // Arrange
        var name = Guid.NewGuid().ToString("N");
        var textGrain = _host.Cluster.GrainFactory.GetGrain<ITextGrain>(name);
        var text = "a1 2b three four c5d six, e7!f. eight g9h! ?";

        // Act
        var result = await textGrain.ProcessHistogram(text, name);

        // Assert
        Assert.AreEqual(4, result.Count);
        // Need to fix regex to continue this test
    }

    [Test]
    public async Task ProcessHistogram_GoodInput_ShouldReturnExpected()
    {
        // Arrange
        var name = Guid.NewGuid().ToString("N");
        var textGrain = _host.Cluster.GrainFactory.GetGrain<ITextGrain>(name);
        var text = "hey, how are you this day, I ate a banana\nמילים";

        // Act
        var result = await textGrain.ProcessHistogram(text, name);

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
        var name = Guid.NewGuid().ToString("N");
        var textGrain = _host.Cluster.GrainFactory.GetGrain<ITextGrain>(name);
        var text = "hey, how are you this day, I ate a banana\nמילים";

        // Act
        _ = await textGrain.ProcessHistogram(text, name);
        var result = await textGrain.GetResultWithoutProcessing();

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
        var textGrain = builder.Cluster.GrainFactory.GetGrain<ITextGrain>(text);

        // Act + Assert
        _ = Assert.ThrowsAsync<Exception>(async () => await textGrain.ProcessHistogram(text, text));
    }
}
