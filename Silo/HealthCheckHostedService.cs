using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Silo;

public class HealthCheckHostedService : IHostedService
{
    private readonly ILogger<HealthCheckHostedService> _logger;
    private readonly HttpListener _listener;
    private CancellationTokenSource _cts;

    public HealthCheckHostedService(ILogger<HealthCheckHostedService> logger)
    {
        _logger = logger;
        _listener = new HttpListener();
        _listener.Prefixes.Add("http://+:8080/health/");
        _listener.Prefixes.Add("http://+:8080/liveness/");
        _listener.Prefixes.Add("http://+:8080/readiness/");
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting HealthCheckHostedService on port 8080");
        _listener.Start();
        _cts = new CancellationTokenSource();
        Task.Run(() => HandleRequestsAsync(_cts.Token), cancellationToken);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping HealthCheckHostedService");
        _cts?.Cancel();
        _listener.Stop();
        _listener.Close();
        return Task.CompletedTask;
    }

    private async Task HandleRequestsAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                var context = await _listener.GetContextAsync();
                context.Response.StatusCode = 204;
                context.Response.ContentLength64 = 0;
                context.Response.OutputStream.Close();
            }
            catch (Exception ex) when (ex is HttpListenerException || ex is ObjectDisposedException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling health check request");
            }
        }
    }
}
