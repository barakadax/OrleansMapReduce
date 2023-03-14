using NUnit.Framework;
using GrainInterfaces;

namespace FunctionalTests;

[TestFixture]
public class WordGrainTests
{
    [Test]
    public async Task SaysHelloCorrectly()
    {
        var builder = new TestHost();

        var hello = builder.Cluster.GrainFactory.GetGrain<IWordGrain>("asd");
        await hello.WordCalculate("pizza", "food");
    }
}
