namespace Kevsoft.WLED.Tests;

public class StateUpdateBuilderTests
{
    [Fact]
    public async Task UpdateStateEmitsOnlyTouchedFields()
    {
        var mockHttpMessageHandler = new MockHttpMessageHandler();
        var baseUri = $"http://{Guid.NewGuid():N}.com";
        mockHttpMessageHandler.AppendResponse($"{baseUri}/json/state");
        var client = new WLedClient(mockHttpMessageHandler, baseUri);

        await client.UpdateState(s => s
            .TurnOn()
            .Brightness(ByteAdjust.IncrementWrap(40))
            .Transition(TimeSpan.FromMilliseconds(700))
            .Segment(0, seg => seg
                .Effect(Selector.Random)
                .Palette(Selector.Next)));

        var (uri, body) = mockHttpMessageHandler.CapturedRequests.Single();
        uri.Should().Be($"{baseUri}/json/state");

        var root = JsonDocument.Parse(body!).RootElement;
        root.EnumerateObject().Select(p => p.Name).Should()
            .BeEquivalentTo("on", "bri", "transition", "seg");
        root.GetProperty("on").GetBoolean().Should().BeTrue();
        root.GetProperty("bri").GetString().Should().Be("w~40");
        root.GetProperty("transition").GetInt32().Should().Be(7);

        var segment = root.GetProperty("seg")[0];
        segment.EnumerateObject().Select(p => p.Name).Should().BeEquivalentTo("id", "fx", "pal");
        segment.GetProperty("id").GetInt32().Should().Be(0);
        segment.GetProperty("fx").GetString().Should().Be("r");
        segment.GetProperty("pal").GetString().Should().Be("~");
    }

    [Fact]
    public async Task UpdateStateToggleAndTransitionOnce()
    {
        var mockHttpMessageHandler = new MockHttpMessageHandler();
        var baseUri = $"http://{Guid.NewGuid():N}.com";
        mockHttpMessageHandler.AppendResponse($"{baseUri}/json/state");
        var client = new WLedClient(mockHttpMessageHandler, baseUri);

        await client.UpdateState(s => s
            .Toggle()
            .TransitionOnce(TimeSpan.FromSeconds(1)));

        var (_, body) = mockHttpMessageHandler.CapturedRequests.Single();
        var root = JsonDocument.Parse(body!).RootElement;
        root.GetProperty("on").GetString().Should().Be("t");
        root.GetProperty("tt").GetInt32().Should().Be(10);
    }
}
