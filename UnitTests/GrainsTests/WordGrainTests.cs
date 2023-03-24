using Grains;
using NUnit.Framework;

namespace UnitTests.GrainsTests;

[TestFixture]
public class WordGrainTests
{
    [Test]
    public void CalculateWordLength_Throws_ShouldGetAnException()
    {
        // Assert
        var wordGrain = new WordGrain(null, null);

        // Act + Assert
        _ = Assert.ThrowsAsync<NullReferenceException>(async () => await wordGrain.WordCalculate(string.Empty, "name"));
    }
}
