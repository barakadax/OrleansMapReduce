using Extensions;
using Grains;
using GrainInterfaces;
using NUnit.Framework;
using NSubstitute;

namespace UnitTests.GrainsTests;

[TestFixture]
public class TextGrainTests
{
    private IGrainFactory _grainFactory;

    [SetUp]
    public void SetUp()
    {
        _grainFactory = Substitute.For<IGrainFactory>();
    }

    [Test]
    public async Task GetResults_NeverCalculated_ShouldReturnEmpty()
    {
        // Arrange
        var textGrain = new TextGrain(_grainFactory);

        // Act
        var result = await textGrain.GetResults();

        // Assert
        Assert.That(result.IsNullOrEmpty(), Is.True);
    }

    [TestCase(null, "job1")]
    [TestCase("text", null)]
    [TestCase("", "job1")]
    [TestCase("text", "")]
    public async Task ProcessText_InvalidInput_ShouldReturnNull(string text, string resultIdentifier)
    {
        // Arrange
        var textGrain = new TextGrain(_grainFactory);

        // Act
        var result = await textGrain.ProcessText(text, resultIdentifier);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task ProcessText_ValidInput_ShouldExecuteMapReduceFlow()
    {
        // Arrange
        var textGrain = new TextGrain(_grainFactory);
        var text = "hello world";
        var resultIdentifier = "job1";

        // Mock WordGrains
        var helloWordGrain = Substitute.For<IWordGrain>();
        helloWordGrain.ProcessWord("HELLO", resultIdentifier).Returns((ulong)5);
        _grainFactory.GetGrain<IWordGrain>("HELLO").Returns(helloWordGrain);

        var worldWordGrain = Substitute.For<IWordGrain>();
        worldWordGrain.ProcessWord("WORLD", resultIdentifier).Returns((ulong)5);
        _grainFactory.GetGrain<IWordGrain>("WORLD").Returns(worldWordGrain);

        // Mock NumberGrain (Reducer)
        var length5Counter = Substitute.For<INumberGrain>();
        length5Counter.GetCount().Returns((ulong)2);
        _grainFactory.GetGrain<INumberGrain>("job15").Returns(length5Counter);

        // Act
        var result = await textGrain.ProcessText(text, resultIdentifier);

        // Assert
        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[5], Is.EqualTo(2));
        
        // Verify Interactions
        _ = _grainFactory.Received(1).GetGrain<IWordGrain>("HELLO");
        _ = _grainFactory.Received(1).GetGrain<IWordGrain>("WORLD");
        _ = helloWordGrain.Received(1).ProcessWord("HELLO", resultIdentifier);
        _ = worldWordGrain.Received(1).ProcessWord("WORLD", resultIdentifier);
        _ = length5Counter.Received(1).GetCount();
    }

    [Test]
    public async Task ProcessText_ExistingResult_ShouldReturnImmediately()
    {
        // Arrange
        var textGrain = new TextGrain(_grainFactory);
        var text = "test";
        var resultIdentifier = "job1";

        // Mock a single word flow
        var testWordGrain = Substitute.For<IWordGrain>();
        testWordGrain.ProcessWord("TEST", resultIdentifier).Returns((ulong)4);
        _grainFactory.GetGrain<IWordGrain>("TEST").Returns(testWordGrain);

        var length4Counter = Substitute.For<INumberGrain>();
        length4Counter.GetCount().Returns((ulong)1);
        _grainFactory.GetGrain<INumberGrain>("job14").Returns(length4Counter);

        // First call
        await textGrain.ProcessText(text, resultIdentifier);

        // Act - Second call
        var result = await textGrain.ProcessText("different text", resultIdentifier);

        // Assert
        Assert.That(result[4], Is.EqualTo(1));
        // Verify that WordGrain was only called once (from the first call)
        _ = testWordGrain.Received(1).ProcessWord(Arg.Any<string>(), Arg.Any<string>());
    }
}
