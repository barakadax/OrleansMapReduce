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
        var fileName = Guid.NewGuid().ToString("N");

        // Act
        var wordGrain = _host.Cluster.GrainFactory.GetGrain<IWordGrain>(fileName);
        var resultFromWordGrain = await wordGrain.WordCalculate(word, fileName);
        var resultFromNumberGrain = await _host.Cluster.GrainFactory.GetGrain<INumberGrain>($"{fileName}{word.Length}").GetCounter();

        // Assert
        Assert.AreEqual(word.Length, resultFromWordGrain);
        Assert.AreEqual(1, resultFromNumberGrain);
    }

    [TestCase("a", "I")]
    [TestCase("זר", "外国")]
    [TestCase("pizza", "Barak")]
    public async Task CalculateWordLength_TwoWordWithSameLength_ShouldReturnCorrectLength(string word1, string word2)
    {
        // Arrange
        var fileName = Guid.NewGuid().ToString("N");

        // Act
        var wordGrain = _host.Cluster.GrainFactory.GetGrain<IWordGrain>(fileName);
        var result1 = await wordGrain.WordCalculate(word1, fileName);
        var result2 = await wordGrain.WordCalculate(word2, fileName);
        var resultFromNumberGrain = await _host.Cluster.GrainFactory.GetGrain<INumberGrain>($"{fileName}{word1.Length}").GetCounter();

        // Assert
        Assert.AreEqual(word1.Length, word2.Length);
        Assert.AreEqual(word1.Length, result1);
        Assert.AreEqual(word1.Length, result2);
        Assert.AreEqual(2, resultFromNumberGrain);
    }

    [Test]
    public async Task CalculateWordLength_Null_ShouldReturnZero()
    {
        // Arrange
        var fileName = Guid.NewGuid().ToString("N");

        // Act
        var wordGrain = _host.Cluster.GrainFactory.GetGrain<IWordGrain>(fileName);
        var result = await wordGrain.WordCalculate(null, fileName);

        // Assert
        Assert.AreEqual(0, result);
    }

    [Test]
    public async Task CalculateWordLength_StringEmpty_ShouldReturnZero()
    {
        // Arrange
        var fileName = Guid.NewGuid().ToString("N");

        // Act
        var wordGrain = _host.Cluster.GrainFactory.GetGrain<IWordGrain>(fileName);
        var result = await wordGrain.WordCalculate(string.Empty, fileName);

        // Assert
        Assert.AreEqual(0, result);
    }

    [Test]
    [NonParallelizable]
    public void CalculateWordLength_Throws_ShouldGetAnException()
    {
        // Arrange
        var fileName = Guid.NewGuid().ToString("N");
        var builder = new TestHost<TestSiloConfigurationsThrows>();
        var wordGrain = builder.Cluster.GrainFactory.GetGrain<IWordGrain>(fileName);

        // Act + Assert
        Assert.ThrowsAsync<Exception>(async () => await wordGrain.WordCalculate(string.Empty, fileName));
    }
}
