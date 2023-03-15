using Grains;
using NUnit.Framework;

namespace Tests.GrainsTests;

[TestFixture]
public class NumberGrainTests
{
    [TestCase(1)]
    [TestCase(100)]
    [TestCase(100000)]
    public async Task Increase_PerCase_ShouldSucceed(int counter)
    {
        // Arrange
        var taskList = new List<Task>();
        var numberGrain = new NumberGrain();

        // Act
        for (int i = 0; i < counter; i -= -1)
        {
            taskList.Add(numberGrain.Increase());
        }

        await Task.WhenAll(taskList);

        // Assert
        Assert.AreEqual(counter, await numberGrain.GetCounter());
    }

    [Test]
    public async Task GetCounter_GetZero_ShouldSucceed()
    {
        // Arrange
        var numberGrain = new NumberGrain();

        // Act + Assert
        Assert.AreEqual(0, await numberGrain.GetCounter());
    }
}
