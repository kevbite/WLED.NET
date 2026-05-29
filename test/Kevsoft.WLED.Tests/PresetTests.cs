namespace Kevsoft.WLED.Tests;

public class PresetTests
{
    [Fact]
    public async Task GetPresetsExcludesScratchSlotAndPlaylists()
    {
        var json = @"{
            ""0"": {},
            ""1"": {""on"":true,""mainseg"":0,""n"":""Sunset"",""ql"":""SS"",""seg"":[{""id"":0,""start"":0,""stop"":10}]},
            ""2"": {""playlist"":{""ps"":[1,2],""dur"":[10,10]},""on"":true,""n"":""Party""},
            ""3"": {""on"":false,""n"":""Off scene"",""seg"":[]}
        }";

        var mockHttpMessageHandler = new MockHttpMessageHandler();
        var baseUri = $"http://{Guid.NewGuid():N}.com";
        mockHttpMessageHandler.AppendResponse($"{baseUri}/presets.json", json);
        var client = new WLedClient(mockHttpMessageHandler, baseUri);

        var presets = await client.GetPresets();

        presets.Keys.Should().BeEquivalentTo(new[] { 1, 3 });
        presets[1].Name.Should().Be("Sunset");
        presets[1].QuickLabel.Should().Be("SS");
        presets[1].On.Should().BeTrue();
        presets[1].Segments.Should().HaveCount(1);
        presets[1].Segments[0].Stop.Should().Be(10);
        presets[3].On.Should().BeFalse();
    }

    [Fact]
    public async Task ApplyPresetCycleSerializesCycleToken()
    {
        var (_, body) = await Capture(client => client.ApplyPreset(PresetSelector.Cycle(1, 6)));

        var root = JsonDocument.Parse(body!).RootElement;
        root.GetProperty("ps").GetString().Should().Be("1~6~");
    }

    [Fact]
    public async Task ApplyPresetByIdSerializesNumber()
    {
        var (_, body) = await Capture(client => client.ApplyPreset(5));

        var root = JsonDocument.Parse(body!).RootElement;
        root.GetProperty("ps").GetInt32().Should().Be(5);
    }

    [Fact]
    public async Task SavePresetSerializesSlotAndFlags()
    {
        var (_, body) = await Capture(client => client.SavePreset(3, new SavePresetOptions
        {
            Name = "X",
            QuickLabel = "QL",
            IncludeBrightness = false
        }));

        var root = JsonDocument.Parse(body!).RootElement;
        root.GetProperty("psave").GetInt32().Should().Be(3);
        root.GetProperty("n").GetString().Should().Be("X");
        root.GetProperty("ql").GetString().Should().Be("QL");
        root.GetProperty("sb").GetBoolean().Should().BeTrue();
        root.GetProperty("ib").GetBoolean().Should().BeFalse();
        root.GetProperty("sc").GetBoolean().Should().BeTrue();
    }

    [Fact]
    public async Task SavePresetWithoutOptionsUsesDefaults()
    {
        var (_, body) = await Capture(client => client.SavePreset(2));

        var root = JsonDocument.Parse(body!).RootElement;
        root.GetProperty("psave").GetInt32().Should().Be(2);
        root.GetProperty("sb").GetBoolean().Should().BeTrue();
        root.GetProperty("ib").GetBoolean().Should().BeTrue();
        root.GetProperty("sc").GetBoolean().Should().BeTrue();
        root.TryGetProperty("n", out _).Should().BeFalse();
    }

    [Fact]
    public async Task DeletePresetSerializesSlot()
    {
        var (_, body) = await Capture(client => client.DeletePreset(3));

        var root = JsonDocument.Parse(body!).RootElement;
        root.GetProperty("pdel").GetInt32().Should().Be(3);
    }

    private static async Task<(string Uri, string? Body)> Capture(Func<WLedClient, Task> act)
    {
        var mockHttpMessageHandler = new MockHttpMessageHandler();
        var baseUri = $"http://{Guid.NewGuid():N}.com";
        mockHttpMessageHandler.AppendResponse($"{baseUri}/json/state");
        var client = new WLedClient(mockHttpMessageHandler, baseUri);

        await act(client);

        var (uri, body) = mockHttpMessageHandler.CapturedRequests.Single();
        return (uri, body);
    }
}
