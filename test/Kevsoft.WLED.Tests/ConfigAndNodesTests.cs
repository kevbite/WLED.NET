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
    public async Task GetConfigParsesTypedSectionsAndPreservesUnknownKeys()
    {
        var json = @"{
            ""id"":{""name"":""WLED"",""mdns"":""wled-desk"",""inv"":""Light""},
            ""if"":{""mqtt"":{""en"":true,""broker"":""mqtt.local"",""port"":1883,""user"":""u"",""cid"":""WLED-1"",""rtn"":false}},
            ""def"":{""on"":true,""bri"":128,""ps"":5},
            ""custom_firmware_key"":42
        }";

        var mockHttpMessageHandler = new MockHttpMessageHandler();
        var baseUri = $"http://{Guid.NewGuid():N}.com";
        mockHttpMessageHandler.AppendResponse($"{baseUri}/json/cfg", json);
        var client = new WLedClient(mockHttpMessageHandler, baseUri);

        var config = await client.GetConfig();

        config.Identity!.Name.Should().Be("WLED");
        config.Identity.MdnsName.Should().Be("wled-desk");
        config.Identity.Unknown.Should().ContainKey("inv");
        config.Interfaces!.Mqtt!.Enabled.Should().BeTrue();
        config.Interfaces.Mqtt.Broker.Should().Be("mqtt.local");
        config.Interfaces.Mqtt.Port.Should().Be(1883);
        config.Interfaces.Mqtt.User.Should().Be("u");
        config.Interfaces.Mqtt.ClientId.Should().Be("WLED-1");
        config.Interfaces.Mqtt.Unknown.Should().ContainKey("rtn");
        config.Defaults!.On.Should().BeTrue();
        config.Defaults.Brightness.Should().Be(128);
        config.Defaults.PresetId.Should().Be(5);

        // Round-trip should preserve both typed and unknown keys.
        var element = JsonDocument.Parse(JsonSerializer.Serialize(config)).RootElement;
        element.GetProperty("id").GetProperty("inv").GetString().Should().Be("Light");
        element.GetProperty("if").GetProperty("mqtt").GetProperty("broker").GetString().Should().Be("mqtt.local");
        element.GetProperty("def").GetProperty("bri").GetInt32().Should().Be(128);
        element.GetProperty("custom_firmware_key").GetInt32().Should().Be(42);
    }

    [Fact]
    public async Task GetConfigPreservesUnknownKeys()
    {
        var json = @"{
            ""id"":{""name"":""WLED"",""mdns"":""wled-desk"",""inv"":""Light""},
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
        config.Identity.Unknown.Should().ContainKey("inv");
        config.Unknown.Should().ContainKey("custom_firmware_key");

        // Round-trip should preserve the unknown keys.
        var roundTripped = JsonSerializer.Serialize(config);
        var element = JsonDocument.Parse(roundTripped).RootElement;
        element.GetProperty("id").GetProperty("mdns").GetString().Should().Be("wled-desk");
        element.GetProperty("id").GetProperty("inv").GetString().Should().Be("Light");
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
