using Extensions;
using NUnit.Framework;

namespace Tests.ExtensionsTests;

[TestFixture]
public class ExtensionsMethodsTests
{
    [TestCase("", true)]
    [TestCase(null, true)]
    [TestCase("str", false)]
    public void String_IsNullOrEmpty_ShouldSucceed(string value, bool expected)
    {
        // Arrange Act & Assert
        Assert.That(value.IsNullOrEmpty(), Is.EqualTo(expected));
    }

    [TestCase("", false)]
    [TestCase(null, false)]
    [TestCase("str", true)]
    public void String_NotNullNorEmpty_ShouldSucceed(string value, bool expected)
    {
        // Arrange Act & Assert
        Assert.That(value.NotNullNorEmpty(), Is.EqualTo(expected));
    }

    [Test]
    public void UlongUlongDictionary_IsNullOrEmpty_Null_ShouldReturnTrue()
    {
        // Arrange
        Dictionary<ulong, ulong> value = null;

        // Act + Arrange
        Assert.That(value!.IsNullOrEmpty(), Is.True);
    }

    [Test]
    public void UlongUlongDictionary_IsNullOrEmpty_Empty_ShouldReturnTrue()
    {
        // Arrange
        var value = new Dictionary<ulong, ulong>();

        // Act + Arrange
        Assert.That(value.IsNullOrEmpty(), Is.True);
    }

    [Test]
    public void UlongUlongDictionary_IsNullOrEmpty_Empty_ShouldReturnFalse()
    {
        // Arrange
        var value = new Dictionary<ulong, ulong>() { [1] = 1 };

        // Act + Arrange
        Assert.That(value.IsNullOrEmpty(), Is.False);
    }

    [Test]
    public void UlongUlongDictionary_NotNullNorEmpty_Null_ShouldReturnFalse()
    {
        // Arrange
        Dictionary<ulong, ulong> value = null;

        // Act + Arrange
        Assert.That(value!.NotNullNorEmpty(), Is.False);
    }

    [Test]
    public void UlongUlongDictionary_NotNullNorEmpty_Empty_ShouldReturnFalse()
    {
        // Arrange
        var value = new Dictionary<ulong, ulong>();

        // Act + Arrange
        Assert.That(value.NotNullNorEmpty(), Is.False);
    }

    [Test]
    public void UlongUlongDictionary_NotNullNorEmpty_Empty_ShouldReturnTrue()
    {
        // Arrange
        var value = new Dictionary<ulong, ulong>() { [1] = 1 };

        // Act + Arrange
        Assert.That(value.NotNullNorEmpty(), Is.True);
    }
}
