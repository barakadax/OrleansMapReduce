using NUnit.Framework;
using Silo;
using System.Net;

namespace UnitTests.Factories;

[TestFixture]
public class HttpListenerFactoryTests
{
    [Test]
    public void Create_Returns_NonNull_HttpListener()
    {
        var factory = new DefaultHttpListenerFactory();

        var listener = factory.Create();

        Assert.That(listener, Is.Not.Null);
        Assert.That(listener, Is.InstanceOf<HttpListener>());
    }

    [Test]
    public void Create_Returns_New_Instance_On_Each_Call()
    {
        var factory = new DefaultHttpListenerFactory();

        var a = factory.Create();
        var b = factory.Create();

        Assert.That(a, Is.Not.SameAs(b));
    }

    [Test]
    public void Created_HttpListener_Is_NotListening_And_Has_No_Prefixes()
    {
        var factory = new DefaultHttpListenerFactory();
        var listener = factory.Create();

        Assert.That(listener.IsListening, Is.False);
        Assert.That(listener.Prefixes, Is.Not.Null);
        Assert.That(listener.Prefixes.Count, Is.EqualTo(0));
    }
}
