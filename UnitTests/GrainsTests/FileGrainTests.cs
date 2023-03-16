using Extensions;
using Grains;
using NUnit.Framework;

namespace UnitTests.GrainsTests;

[TestFixture]
public class FileGrainTests
{
    [Test]
    public async Task GetResultWithoutProcessing_NeverCalculated_ShouldReturnEmpty()
    {
        // Assert
        var fileGrain = new FileGrain();

        // Act
        var result = await fileGrain.GetResultWithoutProcessing();

        // Assert
        Assert.IsTrue(result.IsNullOrEmpty());
    }

    [Test]
    public async Task ProcessHistogram_TextIsNull_ShouldReturnNull()
    {
        // Assert
        var fileGrain = new FileGrain();

        // Act
        var result = await fileGrain.ProcessHistogram(null, "fileName");

        // Assert
        Assert.IsNull(result);
    }

    [Test]
    public async Task ProcessHistogram_FileNameIsNull_ShouldReturnNull()
    {
        // Assert
        var fileGrain = new FileGrain();

        // Act
        var result = await fileGrain.ProcessHistogram("Text", null);

        // Assert
        Assert.IsNull(result);
    }

    [Test]
    public async Task ProcessHistogram_FileNameAndTextAreNull_ShouldReturnNull()
    {
        // Assert
        var fileGrain = new FileGrain();

        // Act
        var result = await fileGrain.ProcessHistogram(null, null);

        // Assert
        Assert.IsNull(result);
    }

    [TestCase("", "")]
    [TestCase("text", "")]
    [TestCase("", "fileName")]
    public async Task ProcessHistogram_InputIsEmpty_ShouldReturnNull(string text, string fileName)
    {
        // Assert
        var fileGrain = new FileGrain();

        // Act
        var result = await fileGrain.ProcessHistogram(text, fileName);

        // Assert
        Assert.IsNull(result);
    }
}
