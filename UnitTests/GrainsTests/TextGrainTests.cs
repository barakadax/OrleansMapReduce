using Extensions;
using Grains;
using NUnit.Framework;

namespace UnitTests.GrainsTests;

[TestFixture]
public class TextGrainTests
{
    [Test]
    public async Task GetResults_NeverCalculated_ShouldReturnEmpty()
    {
        // Assert
        var textGrain = new TextGrain();

        // Act
        var result = await textGrain.GetResults();

        // Assert
        Assert.That(result.IsNullOrEmpty(), Is.True);
    }

    [Test]
    public async Task ProcessText_TextIsNull_ShouldReturnNull()
    {
        // Assert
        var textGrain = new TextGrain();

        // Act
        var result = await textGrain.ProcessText(null, "name");

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task ProcessText_NameIsNull_ShouldReturnNull()
    {
        // Assert
        var textGrain = new TextGrain();

        // Act
        var result = await textGrain.ProcessText("Text", null);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task ProcessText_NameAndTextAreNull_ShouldReturnNull()
    {
        // Assert
        var textGrain = new TextGrain();

        // Act
        var result = await textGrain.ProcessText(null, null);

        // Assert
        Assert.That(result, Is.Null);
    }

    [TestCase("", "")]
    [TestCase("text", "")]
    [TestCase("", "name")]
    public async Task ProcessText_InputIsEmpty_ShouldReturnNull(string text, string name)
    {
        // Assert
        var textGrain = new TextGrain();

        // Act
        var result = await textGrain.ProcessText(text, name);

        // Assert
        Assert.That(result, Is.Null);
    }
}
