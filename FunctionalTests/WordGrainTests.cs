using NUnit.Framework;
using GrainInterfaces;

namespace FunctionalTests;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class WordGrainTests
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

    [TestCase("a")]
    [TestCase("זר")]
    [TestCase("pizza")]
    [TestCase("supercalafragilisticexpialidocious")]
    public async Task CalculateWordLength_OneWord_ShouldReturnCorrectLength(string word)
    {
        // Arrange
        var name = Guid.NewGuid().ToString("N");

        // Act
        var wordGrain = _host.Cluster.GrainFactory.GetGrain<IWordGrain>(name);
        var resultFromWordGrain = await wordGrain.WordCalculate(word, name);
        var resultFromNumberGrain = await _host.Cluster.GrainFactory.GetGrain<INumberGrain>($"{name}{word.Length}").GetCounter();

        // Assert
        Assert.That(resultFromWordGrain, Is.EqualTo(word.Length));
        Assert.That(resultFromNumberGrain, Is.EqualTo(1));
    }

    [TestCase("a", "I")]
    [TestCase("זר", "外国")]
    [TestCase("pizza", "Barak")]
    public async Task CalculateWordLength_TwoWordWithSameLength_ShouldReturnCorrectLength(string word1, string word2)
    {
        // Arrange
        var name = Guid.NewGuid().ToString("N");

        // Act
        var wordGrain = _host.Cluster.GrainFactory.GetGrain<IWordGrain>(name);
        var result1 = await wordGrain.WordCalculate(word1, name);
        var result2 = await wordGrain.WordCalculate(word2, name);
        var resultFromNumberGrain = await _host.Cluster.GrainFactory.GetGrain<INumberGrain>($"{name}{word1.Length}").GetCounter();

        // Assert
        Assert.That(word2.Length, Is.EqualTo(word1.Length));
        Assert.That(result1, Is.EqualTo(word1.Length));
        Assert.That(result2, Is.EqualTo(word1.Length));
        Assert.That(resultFromNumberGrain, Is.EqualTo(2));
    }

    [Test]
    public async Task CalculateWordLength_Null_ShouldReturnZero()
    {
        // Arrange
        var name = Guid.NewGuid().ToString("N");

        // Act
        var wordGrain = _host.Cluster.GrainFactory.GetGrain<IWordGrain>(name);
        var result = await wordGrain.WordCalculate(null, name);

        // Assert
        Assert.That(result, Is.EqualTo(0));
    }

    [Test]
    public async Task CalculateWordLength_StringEmpty_ShouldReturnZero()
    {
        // Arrange
        var name = Guid.NewGuid().ToString("N");

        // Act
        var wordGrain = _host.Cluster.GrainFactory.GetGrain<IWordGrain>(name);
        var result = await wordGrain.WordCalculate(string.Empty, name);

        // Assert
        Assert.That(result, Is.EqualTo(0));
    }
}
