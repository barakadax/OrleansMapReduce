using Extensions;
using Extensions.Interfaces;
using Grains;
using GrainInterfaces;
using Translators.Interfaces;
using NUnit.Framework;
using NSubstitute;
using System.Collections.Concurrent;

namespace UnitTests.GrainsTests;

[TestFixture]
public class WordGrainTests
{
    private IMicrosoftTranslator _translator;
    private ITranslatedWordsDictionary _translatedDictionary;
    private IGrainFactory _grainFactory;

    [SetUp]
    public void SetUp()
    {
        _translator = Substitute.For<IMicrosoftTranslator>();
        _grainFactory = Substitute.For<IGrainFactory>();
        _translatedDictionary = new TranslatedWordsDictionary
        {
            TranslatedWords = new ConcurrentDictionary<string, string>()
        };
    }

    [TestCase(null, "job1")]
    [TestCase("", "job1")]
    [TestCase("word", null)]
    [TestCase("word", "")]
    public async Task ProcessWord_InvalidInput_ShouldReturnZero(string word, string resultIdentifier)
    {
        // Arrange
        var wordGrain = new WordGrain(_translator, _translatedDictionary, _grainFactory);

        // Act
        var result = await wordGrain.ProcessWord(word, resultIdentifier);

        // Assert
        Assert.That(result, Is.EqualTo(0));
    }

    [Test]
    public async Task ProcessWord_NoTranslationNeeded_ShouldIncrementCounterAndReturnLength()
    {
        // Arrange
        var wordGrain = new WordGrain(_translator, _translatedDictionary, _grainFactory);
        var word = "hello";
        var resultIdentifier = "job1";

        var numberGrain = Substitute.For<INumberGrain>();
        _grainFactory.GetGrain<INumberGrain>("job15").Returns(numberGrain);

        // Act
        var result = await wordGrain.ProcessWord(word, resultIdentifier);

        // Assert
        Assert.That(result, Is.EqualTo(5));
        _ = numberGrain.Received(1).Increment();
    }

    [Test]
    public async Task ProcessWord_WithCachedTranslation_ShouldUseCacheAndReturnTranslatedLength()
    {
        // Arrange
        var wordGrain = new WordGrain(_translator, _translatedDictionary, _grainFactory);
        var word = "cat";
        var resultIdentifier = "job1";
        _ = _translatedDictionary.TranslatedWords.TryAdd(word, "gato");

        var numberGrain = Substitute.For<INumberGrain>();
        _grainFactory.GetGrain<INumberGrain>("job14").Returns(numberGrain);

        // Act
        var result = await wordGrain.ProcessWord(word, resultIdentifier);

        // Assert
        Assert.That(result, Is.EqualTo(4)); // length of "gato"
        _ = numberGrain.Received(1).Increment();
        _ = _translator.DidNotReceive().GetWordTranslation(Arg.Any<string>());
    }

    [Test]
    public async Task ProcessWord_WithTranslatorCall_ShouldTranslateAndCache()
    {
        // Arrange
        var wordGrain = new WordGrain(_translator, _translatedDictionary, _grainFactory);
        var word = "dog";
        var resultIdentifier = "job1";

        _translator.CanTranslate().Returns(true);
        _translator.GetWordTranslation(word).Returns("perro");

        var numberGrain = Substitute.For<INumberGrain>();
        _grainFactory.GetGrain<INumberGrain>("job15").Returns(numberGrain);

        // Act
        var result = await wordGrain.ProcessWord(word, resultIdentifier);

        // Assert
        Assert.That(result, Is.EqualTo(5)); // length of "perro"
        Assert.That(_translatedDictionary.TranslatedWords[word], Is.EqualTo("perro"));
        _ = numberGrain.Received(1).Increment();
    }
}
