using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using Silo;
using System.Net;

namespace FunctionalTests;

[TestFixture]
[NonParallelizable]
public class HealthCheckHostedServiceTests
{
    private ILogger<HealthCheckHostedService> _mockLogger;
    private IHttpListenerFactory _mockListenerFactory;
    private HealthCheckHostedService _service;

    [SetUp]
    public void SetUp()
    {
        _mockLogger = Substitute.For<ILogger<HealthCheckHostedService>>();
        _mockListenerFactory = Substitute.For<IHttpListenerFactory>();
        _mockListenerFactory.Create().Returns(new HttpListener());
        _service = new HealthCheckHostedService(_mockLogger, _mockListenerFactory);
    }

    [TearDown]
    public async Task TearDown()
    {
        if (_service != null)
        {
            try
            {
                await _service.StopAsync(CancellationToken.None);
            }
            catch
            {
                // Suppress errors during cleanup
            }
        }
    }

    [Test]
    public async Task StartAsync_ShouldStartListenerSuccessfully()
    {
        // Act & Assert
        var startTask = _service.StartAsync(CancellationToken.None);
        Assert.That(startTask.IsCompletedSuccessfully, Is.True, "StartAsync should complete successfully");

        // Cleanup
        await _service.StopAsync(CancellationToken.None);
    }

    [Test]
    public async Task StopAsync_ShouldStopListenerSuccessfully()
    {
        // Arrange
        await _service.StartAsync(CancellationToken.None);
        await Task.Delay(50);

        // Act & Assert
        var stopTask = _service.StopAsync(CancellationToken.None);
        Assert.That(stopTask.IsCompletedSuccessfully, Is.True, "StopAsync should complete successfully");
    }

    [TestCase("http://localhost:8080/health/")]
    [TestCase("http://localhost:8080/liveness/")]
    [TestCase("http://localhost:8080/readiness/")]
    public async Task K8sEndpoint_ShouldReturnNoContent(string url)
    {
        // Arrange
        await _service.StartAsync(CancellationToken.None);
        await Task.Delay(200);

        // Act
        var response = await MakeHealthCheckRequest(url);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent),
            $"Expected NoContent, got {response.StatusCode}");

        // Cleanup
        await _service.StopAsync(CancellationToken.None);
    }

    private static async Task<HttpResponseMessage> MakeHealthCheckRequest(string url)
    {
        using var handler = new HttpClientHandler();
        using var client = new HttpClient(handler);
        client.Timeout = TimeSpan.FromSeconds(5);

        try
        {
            return await client.GetAsync(url);
        }
        catch (HttpRequestException ex)
        {
            return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
            {
                Content = new StringContent($"Request failed: {ex.Message}")
            };
        }
        catch (TaskCanceledException)
        {
            return new HttpResponseMessage(HttpStatusCode.RequestTimeout);
        }
    }
}
