using System.Net;

namespace Kevsoft.WLED.Tests;

public class MockHttpMessageHandler : HttpMessageHandler
{
    private readonly Dictionary<string, (HttpStatusCode statusCode, string? body)> _mockedResponses = new(StringComparer.InvariantCultureIgnoreCase);

    private readonly Dictionary<string, string?> _capturedRequests = new(StringComparer.InvariantCultureIgnoreCase);

    private readonly List<(string Uri, string? Body)> _capturedRequestList = new();

    public Dictionary<string, string?> CapturedRequests => _capturedRequests;

    public IReadOnlyList<(string Uri, string? Body)> CapturedRequestList => _capturedRequestList;

    public void AppendResponse(string uri, string body)
    {
        _mockedResponses.Add(uri, (HttpStatusCode.OK, body));
    }
    
    public void AppendResponse(string uri)
    {
        _mockedResponses.Add(uri, (HttpStatusCode.OK, null));
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var body = await (request.Content?.ReadAsStringAsync(cancellationToken) ?? Task.FromResult(""));
        _capturedRequests[request.RequestUri!.AbsoluteUri] = body;
        _capturedRequestList.Add((request.RequestUri!.AbsoluteUri, body));
        
        if (_mockedResponses.TryGetValue(request.RequestUri!.AbsoluteUri, out var value))
        {
            return new HttpResponseMessage(value.statusCode)
            {
                Content = new StringContent(value.body ?? string.Empty, Encoding.UTF8, "application/json")
            };
        }

        return new HttpResponseMessage(HttpStatusCode.NotFound);
    }
}