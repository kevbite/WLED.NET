using System.Net;

namespace Kevsoft.WLED.Tests;

public class ExceptionTests
{
    [Fact]
    public async Task NonSuccessResponseThrowsWledResponseExceptionWithStatusAndBody()
    {
        var mockHttpMessageHandler = new MockHttpMessageHandler();
        var baseUri = $"http://{Guid.NewGuid():N}.com";
        mockHttpMessageHandler.AppendResponse($"{baseUri}/json/state", HttpStatusCode.InternalServerError, "boom");
        var client = new WLedClient(mockHttpMessageHandler, baseUri);

        var act = () => client.GetState();

        var exception = (await act.Should().ThrowAsync<WledResponseException>()).Which;
        exception.StatusCode.Should().Be(500);
        exception.Body.Should().Be("boom");
    }

    [Fact]
    public async Task NotFoundOnGetThrowsWledResponseException()
    {
        var mockHttpMessageHandler = new MockHttpMessageHandler();
        var baseUri = $"http://{Guid.NewGuid():N}.com";
        var client = new WLedClient(mockHttpMessageHandler, baseUri);

        var act = () => client.GetState();

        (await act.Should().ThrowAsync<WledResponseException>()).Which.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task TransportFailureThrowsWledConnectionException()
    {
        var mockHttpMessageHandler = new MockHttpMessageHandler
        {
            ThrowOnSend = new HttpRequestException("no route to host")
        };
        var baseUri = $"http://{Guid.NewGuid():N}.com";
        var client = new WLedClient(mockHttpMessageHandler, baseUri);

        var act = () => client.GetState();

        var exception = (await act.Should().ThrowAsync<WledConnectionException>()).Which;
        exception.InnerException.Should().BeOfType<HttpRequestException>();
    }

    [Fact]
    public async Task TransportFailureOnPostThrowsWledConnectionException()
    {
        var mockHttpMessageHandler = new MockHttpMessageHandler
        {
            ThrowOnSend = new HttpRequestException("connection refused")
        };
        var baseUri = $"http://{Guid.NewGuid():N}.com";
        var client = new WLedClient(mockHttpMessageHandler, baseUri);

        var act = () => client.TurnOn();

        await act.Should().ThrowAsync<WledConnectionException>();
    }

    [Fact]
    public async Task CancelledTokenThrowsOperationCanceledException()
    {
        var mockHttpMessageHandler = new MockHttpMessageHandler();
        var baseUri = $"http://{Guid.NewGuid():N}.com";
        mockHttpMessageHandler.AppendResponse($"{baseUri}/json/state", "{}");
        var client = new WLedClient(mockHttpMessageHandler, baseUri);

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var act = () => client.GetState(cts.Token);

        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}
