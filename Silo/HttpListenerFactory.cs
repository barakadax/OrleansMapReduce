using System.Net;

namespace Silo;

public interface IHttpListenerFactory
{
    public HttpListener Create();
}

public class DefaultHttpListenerFactory : IHttpListenerFactory
{
    public HttpListener Create()
    {
        return new HttpListener();
    }
}
