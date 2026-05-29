namespace Kevsoft.WLED.Tests;

public class IntentMethodTests
{
    [Fact]
    public async Task TurnOnPostsOnTrue()
    {
        var (uri, body) = await Capture(client => client.TurnOn());

        uri.Should().EndWith("/json/state");
        JsonDocument.Parse(body!).RootElement.GetProperty("on").GetBoolean().Should().BeTrue();
    }

    [Fact]
    public async Task TurnOffPostsOnFalse()
    {
        var (_, body) = await Capture(client => client.TurnOff());

        JsonDocument.Parse(body!).RootElement.GetProperty("on").GetBoolean().Should().BeFalse();
    }

    [Fact]
    public async Task TogglePostsOnToggle()
    {
        var (_, body) = await Capture(client => client.Toggle());

        JsonDocument.Parse(body!).RootElement.GetProperty("on").GetString().Should().Be("t");
    }

    [Fact]
    public async Task SetBrightnessPostsBri()
    {
        var (_, body) = await Capture(client => client.SetBrightness(128));

        JsonDocument.Parse(body!).RootElement.GetProperty("bri").GetInt32().Should().Be(128);
    }

    [Fact]
    public async Task SetBrightnessRelativePostsSignedString()
    {
        var (_, body) = await Capture(client => client.SetBrightness(ByteAdjust.Increment(10)));

        JsonDocument.Parse(body!).RootElement.GetProperty("bri").GetString().Should().Be("~10");
    }

    [Fact]
    public async Task SetColorRgbPostsSelectedSegmentColor()
    {
        var (_, body) = await Capture(client => client.SetColor(RgbColor.FromHex("FFAA00")));

        var root = JsonDocument.Parse(body!).RootElement;
        var segment = root.GetProperty("seg").EnumerateArray().Single();
        segment.TryGetProperty("id", out _).Should().BeFalse();
        var color = segment.GetProperty("col").EnumerateArray().First().EnumerateArray()
            .Select(x => x.GetInt32()).ToArray();
        color.Should().Equal(255, 170, 0);
    }

    [Fact]
    public async Task SetColorWithSegmentIdTargetsThatSegment()
    {
        var (_, body) = await Capture(client => client.SetColor(RgbColor.FromHex("010203"), segmentId: 2));

        var segment = JsonDocument.Parse(body!).RootElement.GetProperty("seg").EnumerateArray().Single();
        segment.GetProperty("id").GetInt32().Should().Be(2);
    }

    [Fact]
    public async Task SetColorRgbwPostsFourChannels()
    {
        var (_, body) = await Capture(client => client.SetColor(RgbwColor.FromHex("01020304")));

        var segment = JsonDocument.Parse(body!).RootElement.GetProperty("seg").EnumerateArray().Single();
        var color = segment.GetProperty("col").EnumerateArray().First().EnumerateArray()
            .Select(x => x.GetInt32()).ToArray();
        color.Should().Equal(1, 2, 3, 4);
    }

    [Fact]
    public async Task SetEffectPostsSegmentEffectId()
    {
        var (_, body) = await Capture(client => client.SetEffect(42));

        var segment = JsonDocument.Parse(body!).RootElement.GetProperty("seg").EnumerateArray().Single();
        segment.GetProperty("fx").GetInt32().Should().Be(42);
    }

    [Fact]
    public async Task SetPalettePostsSegmentPaletteId()
    {
        var (_, body) = await Capture(client => client.SetPalette(7, segmentId: 1));

        var segment = JsonDocument.Parse(body!).RootElement.GetProperty("seg").EnumerateArray().Single();
        segment.GetProperty("id").GetInt32().Should().Be(1);
        segment.GetProperty("pal").GetInt32().Should().Be(7);
    }

    [Fact]
    public async Task RebootPostsRebootTrue()
    {
        var (_, body) = await Capture(client => client.Reboot());

        JsonDocument.Parse(body!).RootElement.GetProperty("rb").GetBoolean().Should().BeTrue();
    }

    private static async Task<(string Uri, string? Body)> Capture(Func<WLedClient, Task> act)
    {
        var mockHttpMessageHandler = new MockHttpMessageHandler();
        var baseUri = $"http://{Guid.NewGuid():N}.com";
        mockHttpMessageHandler.AppendResponse($"{baseUri}/json/state");
        var client = new WLedClient(mockHttpMessageHandler, baseUri);

        await act(client);

        var (uri, body) = mockHttpMessageHandler.CapturedRequestList.Single();
        return (uri, body);
    }
}
