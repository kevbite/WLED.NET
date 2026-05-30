namespace Kevsoft.WLED.Tests;

public class EffectMetadataTests
{
    [Fact]
    public async Task ParsesAuroraExample()
    {
        var metadata = await Parse(
            new[] { "!,!;;!;1;sx=24,pal=50" },
            new[] { "Aurora" });

        var aurora = metadata.Single();
        aurora.EffectId.Should().Be(0);
        aurora.Name.Should().Be("Aurora");
        aurora.Sliders.Select(s => (s.Key, s.Label)).Should().Equal(("sx", "Effect speed"), ("ix", "Effect intensity"));
        aurora.Options.Should().BeEmpty();
        aurora.Colors.Should().BeEmpty();
        aurora.UsesPalette.Should().BeTrue();
        aurora.Dimensionality.Should().Be(EffectDimensionality.OneDimensional);
        aurora.Defaults.Should().BeEquivalentTo(new Dictionary<string, int> { ["sx"] = 24, ["pal"] = 50 });
    }

    [Fact]
    public async Task ParsesSliderAndCheckboxPositions()
    {
        var metadata = await Parse(
            new[] { ",Saturation,,,,Invert" },
            new[] { "X" });

        var x = metadata.Single();
        x.Sliders.Select(s => (s.Key, s.Label)).Should().Equal(("ix", "Saturation"));
        x.Options.Select(o => (o.Key, o.Label)).Should().Equal(("o1", "Invert"));
    }

    [Fact]
    public async Task ParsesCheckboxOnly()
    {
        var metadata = await Parse(
            new[] { ",,,,,Random colors" },
            new[] { "X" });

        var x = metadata.Single();
        x.Sliders.Should().BeEmpty();
        x.Options.Select(o => (o.Key, o.Label)).Should().Equal(("o1", "Random colors"));
    }

    [Fact]
    public async Task Custom3SliderReportsRestrictedRange()
    {
        var metadata = await Parse(
            new[] { "!,!,Custom1,Custom2,Custom3" },
            new[] { "X" });

        var custom3 = metadata.Single().Sliders.Single(s => s.Key == "c3");
        custom3.Minimum.Should().Be(0);
        custom3.Maximum.Should().Be(31);
    }

    [Fact]
    public async Task ParsesVolumeReactiveTwoDimensionalFlags()
    {
        var metadata = await Parse(
            new[] { "!,!;!,!,!;!;2v" },
            new[] { "X" });

        var x = metadata.Single();
        x.Dimensionality.Should().Be(EffectDimensionality.TwoDimensional);
        x.ReactsToVolume.Should().BeTrue();
        x.ReactsToFrequency.Should().BeFalse();
        x.IsAudioReactive.Should().BeTrue();
    }

    [Fact]
    public async Task AppliesFallbacksWhenLaterSectionsMissing()
    {
        var metadata = await Parse(
            new[] { "!,!" },
            new[] { "Solid" });

        var solid = metadata.Single();
        solid.Sliders.Select(s => s.Key).Should().Equal("sx", "ix");
        solid.Colors.Select(c => c.Key).Should().Equal("Fx", "Bg", "Cs");
        solid.UsesPalette.Should().BeTrue();
        solid.Dimensionality.Should().Be(EffectDimensionality.OneDimensional);
    }

    [Fact]
    public async Task EmptyParameterSectionMeansNoSliders()
    {
        var metadata = await Parse(
            new[] { "" },
            new[] { "Solid" });

        var solid = metadata.Single();
        solid.Sliders.Should().BeEmpty();
        solid.Options.Should().BeEmpty();
        solid.Colors.Select(c => c.Key).Should().Equal("Fx", "Bg", "Cs");
        solid.UsesPalette.Should().BeTrue();
    }

    [Fact]
    public async Task EmptyColorSectionMeansNoColors()
    {
        var metadata = await Parse(
            new[] { "!;;!;1" },
            new[] { "X" });

        metadata.Single().Colors.Should().BeEmpty();
    }

    [Fact]
    public async Task FiltersReservedEffectsButKeepsIdsAligned()
    {
        var metadata = await Parse(
            new[] { "!,!", "", "!,!" },
            new[] { "Aurora", "RSVD", "Solid" });

        metadata.Select(m => (m.EffectId, m.Name)).Should().Equal((0, "Aurora"), (2, "Solid"));
    }

    [Fact]
    public async Task SupportsReportsControlUsage()
    {
        var metadata = await Parse(
            new[] { "!,!;!;!;1" },
            new[] { "X" });

        var x = metadata.Single();
        x.Supports(SegmentControl.Speed).Should().BeTrue();
        x.Supports(SegmentControl.Custom3).Should().BeFalse();
        x.Supports(SegmentControl.Color1).Should().BeTrue();
        x.Supports(SegmentControl.Color2).Should().BeFalse();
        x.Supports(SegmentControl.Palette).Should().BeTrue();
    }

    private static async Task<IReadOnlyList<EffectMetadata>> Parse(string[] fxdata, string[] effects)
    {
        var mockHttpMessageHandler = new MockHttpMessageHandler();
        var baseUri = $"http://{Guid.NewGuid():N}.com";
        mockHttpMessageHandler.AppendResponse($"{baseUri}/json/fxdata", JsonSerializer.Serialize(fxdata));
        mockHttpMessageHandler.AppendResponse($"{baseUri}/json/eff", JsonSerializer.Serialize(effects));
        var client = new WLedClient(mockHttpMessageHandler, baseUri);

        return await client.GetEffectMetadata();
    }
}
