namespace Kevsoft.WLED.Tests;

public class EffectMetadataActionableTests
{
    private static EffectMetadata Meta(int id, string name, Dictionary<string, int>? defaults = null)
        => new(
            id,
            name,
            Array.Empty<EffectControl>(),
            Array.Empty<EffectControl>(),
            Array.Empty<EffectColorSlot>(),
            UsesPalette: true,
            EffectDimensionality.OneDimensional,
            ReactsToVolume: false,
            ReactsToFrequency: false,
            defaults ?? new Dictionary<string, int>());

    [Fact]
    public void FindByNameIsCaseInsensitive()
    {
        var list = new[] { Meta(0, "Solid"), Meta(3, "Rainbow") };

        list.FindByName("rainbow").EffectId.Should().Be(3);
    }

    [Fact]
    public void FindByIdThrowsWhenMissing()
    {
        var list = new[] { Meta(0, "Solid") };

        list.Invoking(l => l.FindById(9)).Should().Throw<KeyNotFoundException>();
    }

    [Fact]
    public void TryFindByNameReturnsFalseWhenMissing()
    {
        var list = new[] { Meta(0, "Solid") };

        list.TryFindByName("Nope", out _).Should().BeFalse();
    }

    [Fact]
    public async Task ApplyEffectDefaultsSetsSlidersFromMetadata()
    {
        var meta = Meta(3, "Rainbow", new Dictionary<string, int>
        {
            ["sx"] = 200,
            ["ix"] = 128,
            ["c1"] = 10,
            ["c3"] = 40,
        });

        var (_, body) = await Capture(s => s.Segment(0, seg => seg.Effect(meta).ApplyEffectDefaults(meta)));

        var segment = JsonDocument.Parse(body!).RootElement.GetProperty("seg").EnumerateArray().Single();
        segment.GetProperty("fx").GetInt32().Should().Be(3);
        segment.GetProperty("sx").GetInt32().Should().Be(200);
        segment.GetProperty("ix").GetInt32().Should().Be(128);
        segment.GetProperty("c1").GetInt32().Should().Be(10);
        segment.GetProperty("c3").GetInt32().Should().Be(31); // clamped to the 0-31 range
    }

    private static async Task<(string Uri, string? Body)> Capture(Action<StateUpdate> configure)
    {
        var mockHttpMessageHandler = new MockHttpMessageHandler();
        var baseUri = $"http://{Guid.NewGuid():N}.com";
        mockHttpMessageHandler.AppendResponse($"{baseUri}/json/state");
        var client = new WLedClient(mockHttpMessageHandler, baseUri);

        await client.UpdateState(configure);

        return mockHttpMessageHandler.CapturedRequestList.Single();
    }
}
