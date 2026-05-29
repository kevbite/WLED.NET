namespace Kevsoft.WLED.Tests;

public class InfoReadTests
{
    private static readonly JsonSerializerOptions Options = new();

    [Fact]
    public void LightCapabilitiesReadAsFlags()
    {
        var info = JsonSerializer.Deserialize<InformationResponse>(
            @"{""leds"":{""lc"":7,""seglc"":[1,3,6]}}", Options)!;

        info.Leds.LightCapabilities.Should()
            .Be(LightCapability.Rgb | LightCapability.WhiteChannel | LightCapability.ColorTemperature);
        info.Leds.SegmentLightCapabilities.Should().Equal(
            LightCapability.Rgb,
            LightCapability.Rgb | LightCapability.WhiteChannel,
            LightCapability.WhiteChannel | LightCapability.ColorTemperature);
    }

    [Fact]
    public void WebSocketClientsSentinelMapsToNull()
    {
        var unsupported = JsonSerializer.Deserialize<InformationResponse>(@"{""ws"":-1}", Options)!;
        var connected = JsonSerializer.Deserialize<InformationResponse>(@"{""ws"":3}", Options)!;

        unsupported.WebSocketClients.Should().BeNull();
        connected.WebSocketClients.Should().Be(3);
    }

    [Fact]
    public void DiscoveredDevicesSentinelMapsToNull()
    {
        var disabled = JsonSerializer.Deserialize<InformationResponse>(@"{""ndc"":-1}", Options)!;
        var enabled = JsonSerializer.Deserialize<InformationResponse>(@"{""ndc"":5}", Options)!;

        disabled.DiscoveredDevices.Should().BeNull();
        enabled.DiscoveredDevices.Should().Be(5);
    }

    [Fact]
    public void WifiAndFilesystemDeserialize()
    {
        var json = @"{
            ""wifi"":{""bssid"":""AA:BB:CC:DD:EE:FF"",""signal"":72,""channel"":6},
            ""fs"":{""u"":256,""t"":1024,""pmt"":1700000000}
        }";

        var info = JsonSerializer.Deserialize<InformationResponse>(json, Options)!;

        info.Wifi.Bssid.Should().Be("AA:BB:CC:DD:EE:FF");
        info.Wifi.Signal.Should().Be(72);
        info.Wifi.Channel.Should().Be(6);
        info.Filesystem.Used.Should().Be(256);
        info.Filesystem.Total.Should().Be(1024);
        info.Filesystem.LastModified.Should().Be(DateTimeOffset.FromUnixTimeSeconds(1700000000));
    }

    [Fact]
    public void FilesystemComputedProperties()
    {
        var fs = new FilesystemResponse { Used = 256, Total = 1024, PresetsModifiedTimestamp = 0 };

        fs.Free.Should().Be(768);
        fs.UsedPercentage.Should().BeApproximately(25, 0.0001);
        fs.FreePercentage.Should().BeApproximately(75, 0.0001);
        fs.LastModified.Should().BeNull();
    }

    [Fact]
    public async Task GetStateInfoReadsStateAndInfo()
    {
        var mockHttpMessageHandler = new MockHttpMessageHandler();
        var baseUri = $"http://{Guid.NewGuid():N}.com";
        mockHttpMessageHandler.AppendResponse($"{baseUri}/json/si",
            @"{""state"":{""bri"":128},""info"":{""name"":""Desk""}}");
        var client = new WLedClient(mockHttpMessageHandler, baseUri);

        var result = await client.GetStateInfo();

        result.State.Brightness.Should().Be(128);
        result.Info.Name.Should().Be("Desk");
    }

    [Fact]
    public async Task GetNetworksReadsNetworkArray()
    {
        var mockHttpMessageHandler = new MockHttpMessageHandler();
        var baseUri = $"http://{Guid.NewGuid():N}.com";
        mockHttpMessageHandler.AppendResponse($"{baseUri}/json/net",
            @"{""networks"":[{""ssid"":""Home"",""rssi"":-60,""bssid"":""AA"",""channel"":11,""enc"":4}]}");
        var client = new WLedClient(mockHttpMessageHandler, baseUri);

        var networks = await client.GetNetworks();

        networks.Should().HaveCount(1);
        networks[0].Ssid.Should().Be("Home");
        networks[0].Rssi.Should().Be(-60);
        networks[0].Channel.Should().Be(11);
        networks[0].Encryption.Should().Be(4);
    }

    [Fact]
    public async Task GetLiveColorsReadsHexLeds()
    {
        var mockHttpMessageHandler = new MockHttpMessageHandler();
        var baseUri = $"http://{Guid.NewGuid():N}.com";
        mockHttpMessageHandler.AppendResponse($"{baseUri}/json/live",
            @"{""leds"":[""FF0000"",""00FF00"",""0000FF""],""n"":3}");
        var client = new WLedClient(mockHttpMessageHandler, baseUri);

        var live = await client.GetLiveColors();

        live.Should().NotBeNull();
        live!.Length.Should().Be(3);
        live.Leds.Should().Equal(
            new RgbColor(255, 0, 0),
            new RgbColor(0, 255, 0),
            new RgbColor(0, 0, 255));
    }

    [Fact]
    public async Task GetLiveColorsReturnsNullWhenUnsupported()
    {
        var mockHttpMessageHandler = new MockHttpMessageHandler();
        var baseUri = $"http://{Guid.NewGuid():N}.com";
        var client = new WLedClient(mockHttpMessageHandler, baseUri);

        var live = await client.GetLiveColors();

        live.Should().BeNull();
    }
}
