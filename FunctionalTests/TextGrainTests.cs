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
        Assert.That(result.Count, Is.EqualTo(4));
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
        Assert.That(result.Count, Is.EqualTo(5));
        Assert.That(result[1], Is.EqualTo(2));
        Assert.That(result[3], Is.EqualTo(6));
        Assert.That(result[4], Is.EqualTo(1));
        Assert.That(result[5], Is.EqualTo(1));
        Assert.That(result[6], Is.EqualTo(1));
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
        Assert.That(result.Count, Is.EqualTo(5));
        Assert.That(result[1], Is.EqualTo(2));
        Assert.That(result[3], Is.EqualTo(6));
        Assert.That(result[4], Is.EqualTo(1));
        Assert.That(result[5], Is.EqualTo(1));
        Assert.That(result[6], Is.EqualTo(1));
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
