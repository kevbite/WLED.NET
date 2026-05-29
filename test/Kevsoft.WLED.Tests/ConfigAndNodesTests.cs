namespace Kevsoft.WLED.Tests;

public class ConfigAndNodesTests
{
    [Fact]
    public async Task GetNodesParsesNodeList()
    {
        var json = @"{""nodes"":[
            {""name"":""Desk"",""type"":32,""ip"":""192.168.1.5"",""age"":0,""vid"":2310130},
            {""name"":""Shelf"",""type"":32,""ip"":""192.168.1.6"",""age"":3,""vid"":2310130}
        ]}";

        var mockHttpMessageHandler = new MockHttpMessageHandler();
        var baseUri = $"http://{Guid.NewGuid():N}.com";
        mockHttpMessageHandler.AppendResponse($"{baseUri}/json/nodes", json);
        var client = new WLedClient(mockHttpMessageHandler, baseUri);

        var nodes = await client.GetNodes();

        nodes.Should().HaveCount(2);
        nodes[0].Name.Should().Be("Desk");
        nodes[0].IpAddress.Should().Be("192.168.1.5");
        nodes[0].BoardType.Should().Be(32);
        nodes[0].BuildId.Should().Be(2310130u);
        nodes[1].Age.Should().Be(3);
    }

    [Fact]
    public async Task GetNodesReturnsEmptyForEmptyPayload()
    {
        var mockHttpMessageHandler = new MockHttpMessageHandler();
        var baseUri = $"http://{Guid.NewGuid():N}.com";
        mockHttpMessageHandler.AppendResponse($"{baseUri}/json/nodes", @"{""nodes"":[]}");
        var client = new WLedClient(mockHttpMessageHandler, baseUri);

        var nodes = await client.GetNodes();

        nodes.Should().BeEmpty();
    }

    [Fact]
    public async Task GetConfigPreservesUnknownKeys()
    {
        var json = @"{
            ""id"":{""name"":""WLED"",""mdns"":""wled-desk""},
            ""nw"":{""ins"":[{""ssid"":""home""}]},
            ""hw"":{""led"":{""total"":30}},
            ""custom_firmware_key"":42
        }";

        var mockHttpMessageHandler = new MockHttpMessageHandler();
        var baseUri = $"http://{Guid.NewGuid():N}.com";
        mockHttpMessageHandler.AppendResponse($"{baseUri}/json/cfg", json);
        var client = new WLedClient(mockHttpMessageHandler, baseUri);

        var config = await client.GetConfig();

        config.Identity!.Name.Should().Be("WLED");
        config.Identity.Unknown.Should().ContainKey("mdns");
        config.Unknown.Should().ContainKey("custom_firmware_key");

        // Round-trip should preserve the unknown keys.
        var roundTripped = JsonSerializer.Serialize(config);
        var element = JsonDocument.Parse(roundTripped).RootElement;
        element.GetProperty("id").GetProperty("mdns").GetString().Should().Be("wled-desk");
        element.GetProperty("custom_firmware_key").GetInt32().Should().Be(42);
        element.GetProperty("hw").GetProperty("led").GetProperty("total").GetInt32().Should().Be(30);
    }

    [Fact]
    public async Task UpdateConfigEmitsOnlyTouchedSection()
    {
        var partial = new DeviceConfig { Identity = new IdentityConfig { Name = "Kitchen" } };

        var (_, body) = await Capture(client => client.UpdateConfig(partial));

        var element = JsonDocument.Parse(body!).RootElement;
        element.GetProperty("id").GetProperty("name").GetString().Should().Be("Kitchen");
        element.TryGetProperty("nw", out _).Should().BeFalse();
        element.TryGetProperty("hw", out _).Should().BeFalse();
    }

    [Fact]
    public async Task UpdateConfigThrowsOnNetworkChangeWithoutOptIn()
    {
        var partial = new DeviceConfig { Network = new NetworkConfig() };

        var mockHttpMessageHandler = new MockHttpMessageHandler();
        var baseUri = $"http://{Guid.NewGuid():N}.com";
        mockHttpMessageHandler.AppendResponse($"{baseUri}/json/cfg");
        var client = new WLedClient(mockHttpMessageHandler, baseUri);

        Func<Task> act = () => client.UpdateConfig(partial);

        await act.Should().ThrowAsync<InvalidOperationException>();
        mockHttpMessageHandler.CapturedRequestList.Should().BeEmpty();
    }

    [Fact]
    public async Task UpdateConfigAllowsNetworkChangeWhenOptedIn()
    {
        var partial = new DeviceConfig { Network = new NetworkConfig() };

        var (_, body) = await Capture(client => client.UpdateConfig(partial, new UpdateConfigOptions { AllowNetworkChanges = true }));

        JsonDocument.Parse(body!).RootElement.TryGetProperty("nw", out _).Should().BeTrue();
    }

    private static async Task<(string Uri, string? Body)> Capture(Func<WLedClient, Task> act)
    {
        var mockHttpMessageHandler = new MockHttpMessageHandler();
        var baseUri = $"http://{Guid.NewGuid():N}.com";
        mockHttpMessageHandler.AppendResponse($"{baseUri}/json/cfg");
        var client = new WLedClient(mockHttpMessageHandler, baseUri);

        await act(client);

        return mockHttpMessageHandler.CapturedRequestList.Single();
    }
}
