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

    [Fact]
    public async Task UpdateStateSupportsTransitionAboveByteRange()
    {
        var mockHttpMessageHandler = new MockHttpMessageHandler();
        var baseUri = $"http://{Guid.NewGuid():N}.com";
        mockHttpMessageHandler.AppendResponse($"{baseUri}/json/state");
        var client = new WLedClient(mockHttpMessageHandler, baseUri);

        await client.UpdateState(s => s.Transition(TimeSpan.FromSeconds(60)));

        var (_, body) = mockHttpMessageHandler.CapturedRequests.Single();
        var root = JsonDocument.Parse(body!).RootElement;
        root.GetProperty("transition").GetInt32().Should().Be(600);
    }

    [Fact]
    public async Task UpdateStateClampsTransitionToUshortMax()
    {
        var mockHttpMessageHandler = new MockHttpMessageHandler();
        var baseUri = $"http://{Guid.NewGuid():N}.com";
        mockHttpMessageHandler.AppendResponse($"{baseUri}/json/state");
        var client = new WLedClient(mockHttpMessageHandler, baseUri);

        await client.UpdateState(s => s.Transition(TimeSpan.FromHours(3)));

        var (_, body) = mockHttpMessageHandler.CapturedRequests.Single();
        var root = JsonDocument.Parse(body!).RootElement;
        root.GetProperty("transition").GetInt32().Should().Be(ushort.MaxValue);
    }

    [Fact]
    public async Task SelectedSegmentsEmitsObjectFormWithoutId()
    {
        var (_, body) = await Capture(s => s
            .SelectedSegments(seg => seg
                .Color(RgbColor.FromHex("FFAA00"))
                .Effect(Selector.Random)));

        var seg = JsonDocument.Parse(body!).RootElement.GetProperty("seg");
        seg.ValueKind.Should().Be(JsonValueKind.Object);
        seg.TryGetProperty("id", out _).Should().BeFalse();
        seg.GetProperty("fx").GetString().Should().Be("r");
    }

    [Fact]
    public async Task SelectedSegmentsAccumulatesAcrossCalls()
    {
        var (_, body) = await Capture(s => s
            .SelectedSegments(seg => seg.Effect(3))
            .SelectedSegments(seg => seg.Brightness(128)));

        var seg = JsonDocument.Parse(body!).RootElement.GetProperty("seg");
        seg.ValueKind.Should().Be(JsonValueKind.Object);
        seg.GetProperty("fx").GetInt32().Should().Be(3);
        seg.GetProperty("bri").GetInt32().Should().Be(128);
    }

    [Fact]
    public async Task SegmentByIdEmitsArrayForm()
    {
        var (_, body) = await Capture(s => s.Segment(2, seg => seg.Effect(1)));

        var seg = JsonDocument.Parse(body!).RootElement.GetProperty("seg");
        seg.ValueKind.Should().Be(JsonValueKind.Array);
        seg.EnumerateArray().Single().GetProperty("id").GetInt32().Should().Be(2);
    }

    [Fact]
    public void MixingSelectedThenExplicitThrows()
    {
        var update = new StateUpdate();
        update.SelectedSegments(seg => seg.Effect(1));

        update.Invoking(u => u.Segment(0, seg => seg.Effect(2)))
            .Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void MixingExplicitThenSelectedThrows()
    {
        var update = new StateUpdate();
        update.Segment(0, seg => seg.Effect(1));

        update.Invoking(u => u.SelectedSegments(seg => seg.Effect(2)))
            .Should().Throw<InvalidOperationException>();
    }

    private static async Task<(string Uri, string? Body)> Capture(Action<StateUpdate> configure)
    {
        var mockHttpMessageHandler = new MockHttpMessageHandler();
        var baseUri = $"http://{Guid.NewGuid():N}.com";
        mockHttpMessageHandler.AppendResponse($"{baseUri}/json/state");
        var client = new WLedClient(mockHttpMessageHandler, baseUri);

        await client.UpdateState(configure);

        var (uri, body) = mockHttpMessageHandler.CapturedRequestList.Single();
        return (uri, body);
    }
}
