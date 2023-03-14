using NUnit.Framework;
using GrainInterfaces;
using Extensions.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Translators.Interfaces;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace FunctionalTests;

[TestFixture]
public class WordGrainTests
{
    [TestCase("a")]
    [TestCase("זר")]
    [TestCase("pizza")]
    [TestCase("supercalafragilisticexpialidocious")]
    public async Task CalculateWordLength_OneWord_ShouldReturnCorrectLength(string word)
    {
        // Arrange
        var builder = new TestHost();

        // Act
        var wordGrain = builder.Cluster.GrainFactory.GetGrain<IWordGrain>("fileName");
        var resultFromWordGrain = await wordGrain.WordCalculate(word, "fileName");
        var resultFromNumberGrain = await builder.Cluster.GrainFactory.GetGrain<INumberGrain>($"fileName{word.Length}").GetCounter();

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
        var builder = new TestHost();

        // Act
        var wordGrain = builder.Cluster.GrainFactory.GetGrain<IWordGrain>("fileName");
        var result1 = await wordGrain.WordCalculate(word1, "fileName");
        var result2 = await wordGrain.WordCalculate(word2, "fileName");
        var resultFromNumberGrain = await builder.Cluster.GrainFactory.GetGrain<INumberGrain>($"fileName{word1.Length}").GetCounter();

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
        var builder = new TestHost();

        // Act
        var wordGrain = builder.Cluster.GrainFactory.GetGrain<IWordGrain>("fileName");
        var result = await wordGrain.WordCalculate(null, "fileName");

        // Assert
        Assert.AreEqual(0, result);
    }

    [Test]
    public async Task CalculateWordLength_StringEmpty_ShouldReturnZero()
    {
        // Arrange
        var builder = new TestHost();

        // Act
        var wordGrain = builder.Cluster.GrainFactory.GetGrain<IWordGrain>("fileName");
        var result = await wordGrain.WordCalculate(string.Empty, "fileName");

        // Assert
        Assert.AreEqual(0, result);
    }

    [Test]
    public void CalculateWordLength_Throws_ShouldGetAnException()
    {
        // Arrange
        var builder = new TestHost();
        var serviceProvider = builder.Cluster.ServiceProvider;
        var serviceCollection = serviceProvider.GetService<IServiceCollection>();
        var wordGrain = builder.Cluster.GrainFactory.GetGrain<IWordGrain>("fileName");

        // Remove binding
        var existingRegistration = serviceProvider.GetService(typeof(IMicrosoftTranslator));
        if (existingRegistration != null)
        {
            serviceCollection!.RemoveAll<IMicrosoftTranslator>();
        }

        // Add binding
        var translateMock = Substitute.For<IMicrosoftTranslator>();
        translateMock.CanTranslate().Throws(new Exception());
        serviceCollection!.AddSingleton(translateMock);

        // Act + Assert
        Assert.ThrowsAsync<Exception>(async () => await wordGrain.WordCalculate(string.Empty, "fileName"));
    }
}
