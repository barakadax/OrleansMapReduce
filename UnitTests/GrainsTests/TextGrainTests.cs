using Extensions;
using Grains;
using NUnit.Framework;

namespace UnitTests.GrainsTests;

[TestFixture]
public class TextGrainTests
{
    [Test]
    public async Task GetResultWithoutProcessing_NeverCalculated_ShouldReturnEmpty()
    {
        // Assert
        var textGrain = new TextGrain();

        // Act
        var result = await textGrain.GetResultWithoutProcessing();

        // Assert
        Assert.That(result.IsNullOrEmpty(), Is.True);
    }

    [Test]
    public async Task ProcessHistogram_TextIsNull_ShouldReturnNull()
    {
        // Assert
        var textGrain = new TextGrain();

        // Act
        var result = await textGrain.ProcessHistogram(null, "name");

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task ProcessHistogram_NameIsNull_ShouldReturnNull()
    {
        // Assert
        var textGrain = new TextGrain();

        // Act
        var result = await textGrain.ProcessHistogram("Text", null);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task ProcessHistogram_NameAndTextAreNull_ShouldReturnNull()
    {
        // Assert
        var textGrain = new TextGrain();

        // Act
        var result = await textGrain.ProcessHistogram(null, null);

        // Assert
        Assert.That(result, Is.Null);
    }

    [TestCase("", "")]
    [TestCase("text", "")]
    [TestCase("", "name")]
    public async Task ProcessHistogram_InputIsEmpty_ShouldReturnNull(string text, string name)
    {
        // Assert
        var textGrain = new TextGrain();

        // Act
        var result = await textGrain.ProcessHistogram(text, name);

        // Assert
        Assert.That(result, Is.Null);
    }
}
