using Grains;
using NUnit.Framework;

namespace UnitTests.GrainsTests;

[TestFixture]
public class WordGrainTests
{
    [Test]
    public async Task ProcessWord_EmptyInput_ShouldReturnZero()
    {
        // Assert
        var wordGrain = new WordGrain(null, null);

        // Act
        var result = await wordGrain.ProcessWord(string.Empty, "name");

        // Assert
        Assert.That(result, Is.EqualTo(0));
    }
}
