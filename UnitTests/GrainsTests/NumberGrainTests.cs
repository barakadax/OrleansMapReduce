using Grains;
using NUnit.Framework;

namespace UnitTests.GrainsTests;

[TestFixture]
public class NumberGrainTests
{
    [TestCase(1)]
    [TestCase(100)]
    [TestCase(100000)]
    public async Task Increment_PerCase_ShouldSucceed(int counter)
    {
        // Arrange
        var taskList = new List<Task>();
        var numberGrain = new NumberGrain();

        // Act
        for (int i = 0; i < counter; i -= -1)
        {
            taskList.Add(numberGrain.Increment());
        }

        await Task.WhenAll(taskList);

        // Assert
        Assert.That(await numberGrain.GetCount(), Is.EqualTo(counter));
    }

    [Test]
    public async Task GetCount_GetZero_ShouldSucceed()
    {
        // Arrange
        var numberGrain = new NumberGrain();

        // Act + Assert
        Assert.That(await numberGrain.GetCount(), Is.EqualTo(0));
    }
}
