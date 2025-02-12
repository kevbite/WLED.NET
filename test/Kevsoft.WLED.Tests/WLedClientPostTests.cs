namespace Kevsoft.WLED.Tests;

public class WLedClientPostTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public async Task PostEmptyWLedRootRequestData()
    {
        var mockHttpMessageHandler = new MockHttpMessageHandler();
        var baseUri = $"http://{Guid.NewGuid():N}.com";
        mockHttpMessageHandler.AppendResponse($"{baseUri}/json");
        var client = new WLedClient(mockHttpMessageHandler, baseUri);

        var request = new WLedRootRequest();
        await client.Post(request);

        var (uri, body) = mockHttpMessageHandler.CapturedRequests.Single();
        uri.Should().Be($"{baseUri}/json");
        var json = JsonDocument.Parse(body!);
        json.RootElement.EnumerateObject().Should().HaveCount(0);
    }

    [Fact]
    public async Task PostEmptyStateRequestData()
    {
        var mockHttpMessageHandler = new MockHttpMessageHandler();
        var baseUri = $"http://{Guid.NewGuid():N}.com";
        mockHttpMessageHandler.AppendResponse($"{baseUri}/json/state");
        var client = new WLedClient(mockHttpMessageHandler, baseUri);

        var request = new StateRequest();
        await client.Post(request);

        var (uri, body) = mockHttpMessageHandler.CapturedRequests.Single();
        uri.Should().Be($"{baseUri}/json/state");
        var json = JsonDocument.Parse(body!);
        json.RootElement.EnumerateObject().Should().HaveCount(0);
    }

    [Fact]
    public async Task PostEmptySingleLedRequestData()
    {
        var mockHttpMessageHandler = new MockHttpMessageHandler();
        var baseUri = $"http://{Guid.NewGuid():N}.com";
        mockHttpMessageHandler.AppendResponse($"{baseUri}/json/state");
        var client = new WLedClient(mockHttpMessageHandler, baseUri);

        List<SingleLedRequest> request = [];
        await client.Post(request);

        var (uri, body) = mockHttpMessageHandler.CapturedRequests.Single();
        uri.Should().Be($"{baseUri}/json/state");
        var json = JsonDocument.Parse(body!);
        // Expected Request:
        // {"on":true,"seg":[{"id":0,"i":[]}]}

        json.RootElement.EnumerateObject().Should().HaveCount(2);
        json.RootElement.GetProperty("seg").EnumerateArray().Should().HaveCount(1);
        json.RootElement.GetProperty("seg")[0].GetProperty("id").GetInt32().Should().Be(0);
        json.RootElement.GetProperty("seg")[0].GetProperty("i").EnumerateArray().Should().HaveCount(0);
    }

    [Fact]
    public async Task PostFullWLedRootResponse()
    {
        var mockHttpMessageHandler = new MockHttpMessageHandler();
        var baseUri = $"http://{Guid.NewGuid():N}.com";
        mockHttpMessageHandler.AppendResponse($"{baseUri}/json");
        var client = new WLedClient(mockHttpMessageHandler, baseUri);

        var response = _fixture.Create<WLedRootResponse>();
        await client.Post(response);

        var (uri, body) = mockHttpMessageHandler.CapturedRequests.Single();
        uri.Should().Be($"{baseUri}/json");
        var json = JsonDocument.Parse(body!);
        var expected = JsonDocument.Parse(JsonBuilder.CreateRootResponse(response));

        AssertBeEquivalentTo(json.RootElement.GetProperty("state"), expected.RootElement.GetProperty("state"));
    }

    [Fact]
    public async Task PostFullStateResponse()
    {
        var mockHttpMessageHandler = new MockHttpMessageHandler();
        var baseUri = $"http://{Guid.NewGuid():N}.com";
        mockHttpMessageHandler.AppendResponse($"{baseUri}/json/state");
        var client = new WLedClient(mockHttpMessageHandler, baseUri);

        var response = _fixture.Create<StateResponse>();
        await client.Post(response);

        var (uri, body) = mockHttpMessageHandler.CapturedRequests.Single();
        uri.Should().Be($"{baseUri}/json/state");
        var json = JsonDocument.Parse(body!);
        var expected = JsonDocument.Parse(JsonBuilder.CreateStateJson(response));

        AssertBeEquivalentTo(json.RootElement, expected.RootElement);
    }

    private static void AssertBeEquivalentTo(JsonElement actualJsonElement, JsonElement expectedJsonElement)
    {
        if (expectedJsonElement.ValueKind == JsonValueKind.Object)
        {
            foreach (var jsonProperty in expectedJsonElement.EnumerateObject())
            {
                var actualValue = actualJsonElement.GetProperty(jsonProperty.Name);
                AssertBeEquivalentTo(actualValue, jsonProperty.Value);
            }
        }
        else if (expectedJsonElement.ValueKind == JsonValueKind.Array)
        {
            actualJsonElement.GetArrayLength().Should().Be(expectedJsonElement.GetArrayLength());
            foreach (var (actual, expected) in actualJsonElement.EnumerateArray()
                         .Zip(expectedJsonElement.EnumerateArray()))
            {
                AssertBeEquivalentTo(actual, expected);
            }
        }
        else
        {
            actualJsonElement.ValueKind.Should().Be(expectedJsonElement.ValueKind);
            actualJsonElement.GetRawText().Should().Be(expectedJsonElement.GetRawText());
        }
    }
}