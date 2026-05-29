namespace Kevsoft.WLED.Tests;

public class StateObjectTests
{
    private static readonly JsonSerializerOptions Options = new();

    [Theory]
    [InlineData("-1", null)]
    [InlineData("5", 5)]
    public void PresetIdSentinelMapsToNull(string raw, int? expected)
    {
        var json = $@"{{""ps"":{raw},""pl"":-1,""nl"":{{""rem"":-1}}}}";

        var state = JsonSerializer.Deserialize<StateResponse>(json, Options)!;

        state.PresetId.Should().Be(expected);
        state.PlaylistId.Should().BeNull();
        state.Nightlight.Remaining.Should().BeNull();
    }

    [Fact]
    public void LiveDataOverrideReadsAsEnum()
    {
        var state = JsonSerializer.Deserialize<StateResponse>(@"{""lor"":2}", Options)!;

        state.LiveDataOverride.Should().Be(LiveDataOverride.UntilReboot);
    }

    [Fact]
    public void NightlightModeReadsAsEnum()
    {
        var state = JsonSerializer.Deserialize<StateResponse>(@"{""nl"":{""mode"":3}}", Options)!;

        state.Nightlight.Mode.Should().Be(NightlightMode.Sunrise);
    }

    [Fact]
    public void SyncGroupsReadAsFlags()
    {
        var state = JsonSerializer.Deserialize<StateResponse>(@"{""udpn"":{""sgrp"":3,""rgrp"":4}}", Options)!;

        state.UdpPackets.SendGroups.Should().Be(SyncGroup.Group1 | SyncGroup.Group2);
        state.UdpPackets.ReceiveGroups.Should().Be(SyncGroup.Group3);
    }

    [Fact]
    public async Task WriteOnlyCommandsSerialize()
    {
        var mockHttpMessageHandler = new MockHttpMessageHandler();
        var baseUri = $"http://{Guid.NewGuid():N}.com";
        mockHttpMessageHandler.AppendResponse($"{baseUri}/json/state");
        var client = new WLedClient(mockHttpMessageHandler, baseUri);

        await client.UpdateState(s => s
            .EnterLiveMode()
            .SetTime(DateTimeOffset.FromUnixTimeSeconds(1_700_000_000))
            .LoadLedMap(2)
            .RemoveLastCustomPalette()
            .NextPreset());

        var (_, body) = mockHttpMessageHandler.CapturedRequests.Single();
        var root = JsonDocument.Parse(body!).RootElement;

        root.GetProperty("live").GetBoolean().Should().BeTrue();
        root.GetProperty("time").GetInt64().Should().Be(1_700_000_000);
        root.GetProperty("ledmap").GetByte().Should().Be(2);
        root.GetProperty("rmcpal").GetBoolean().Should().BeTrue();
        root.GetProperty("np").GetBoolean().Should().BeTrue();
    }

    [Fact]
    public void LoadLedMapRejectsOutOfRange()
    {
        var act = () => new StateUpdate().LoadLedMap(10);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}
