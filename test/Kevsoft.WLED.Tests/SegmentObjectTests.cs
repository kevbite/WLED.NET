namespace Kevsoft.WLED.Tests;

public class SegmentObjectTests
{
    private static readonly JsonSerializerOptions Options = new();

    [Fact]
    public void RelativeColorTemperatureRoundTrips()
    {
        var cct = ColorTemperature.Relative(128);

        var json = JsonSerializer.Serialize(cct, Options);
        var roundTripped = JsonSerializer.Deserialize<ColorTemperature>(json, Options);

        json.Should().Be("128");
        roundTripped.Should().Be(cct);
        roundTripped.IsKelvin.Should().BeFalse();
    }

    [Fact]
    public void KelvinColorTemperatureRoundTrips()
    {
        var cct = ColorTemperature.Kelvin(6500);

        var json = JsonSerializer.Serialize(cct, Options);
        var roundTripped = JsonSerializer.Deserialize<ColorTemperature>(json, Options);

        json.Should().Be("6500");
        roundTripped.Should().Be(cct);
        roundTripped.IsKelvin.Should().BeTrue();
    }

    [Theory]
    [InlineData(999)]
    [InlineData(20001)]
    public void KelvinOutOfRangeThrows(int kelvin)
    {
        var act = () => ColorTemperature.Kelvin(kelvin);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(1000)]
    [InlineData(16000)]
    [InlineData(20000)]
    public void KelvinAcceptsForwardCompatibleRange(int kelvin)
    {
        var cct = ColorTemperature.Kelvin(kelvin);

        JsonSerializer.Serialize(cct, Options).Should().Be(kelvin.ToString());
        cct.IsKelvin.Should().BeTrue();
    }

    [Fact]
    public void KelvinUncheckedAllowsValuesBeyondRange()
    {
        var cct = ColorTemperature.KelvinUnchecked(25000);

        JsonSerializer.Serialize(cct, Options).Should().Be("25000");
        cct.IsKelvin.Should().BeTrue();
    }

    [Fact]
    public void KelvinUncheckedRejectsRelativeRangeValues()
    {
        var act = () => ColorTemperature.KelvinUnchecked(200);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void ColorTemperatureReadInfersKelvinAboveByteRange()
    {
        var relative = JsonSerializer.Deserialize<ColorTemperature>("200", Options);
        var kelvin = JsonSerializer.Deserialize<ColorTemperature>("4200", Options);

        relative.IsKelvin.Should().BeFalse();
        kelvin.IsKelvin.Should().BeTrue();
        kelvin.Value.Should().Be(4200);
    }

    [Fact]
    public void SegmentColorsRoundTripViaSegmentResponse()
    {
        var json = @"{""col"":[[255,170,0],[0,0,0,128]]}";

        var segment = JsonSerializer.Deserialize<SegmentResponse>(json, Options)!;

        segment.Colors.Primary.Should().Be(Color.Rgb(255, 170, 0));
        segment.Colors.Secondary.Should().Be(Color.Rgbw(0, 0, 0, 128));
        segment.Colors.Tertiary.Should().BeNull();
    }

    [Fact]
    public async Task SegmentColorBuilderEmitsColorSlots()
    {
        var (_, segment) = await CaptureSegment(seg => seg
            .Color(Color.Rgb(255, 0, 0), Color.Rgb(0, 255, 0)));

        var colors = segment.GetProperty("col");
        colors.GetArrayLength().Should().Be(2);
        colors[0].EnumerateArray().Select(x => x.GetInt32()).Should().Equal(255, 0, 0);
        colors[1].EnumerateArray().Select(x => x.GetInt32()).Should().Equal(0, 255, 0);
    }

    [Fact]
    public async Task SegmentBuilderEmitsTwoDimensionalFields()
    {
        var (_, segment) = await CaptureSegment(seg => seg
            .Range2D(0, 16, 0, 8)
            .Transpose()
            .MirrorY());

        segment.GetProperty("start").GetInt32().Should().Be(0);
        segment.GetProperty("stop").GetInt32().Should().Be(16);
        segment.GetProperty("startY").GetInt32().Should().Be(0);
        segment.GetProperty("stopY").GetInt32().Should().Be(8);
        segment.GetProperty("tp").GetBoolean().Should().BeTrue();
        segment.GetProperty("mY").GetBoolean().Should().BeTrue();
    }

    [Fact]
    public async Task SegmentBuilderEmitsEffectParameters()
    {
        var (_, segment) = await CaptureSegment(seg => seg
            .Speed(200)
            .Intensity(ByteAdjust.Increment(10))
            .Cct(ColorTemperature.Kelvin(5000))
            .CustomSlider1(5)
            .CustomSlider3(31)
            .Set(2)
            .SoundSimulation(SoundSimulation.WeWillRockYou));

        segment.GetProperty("sx").GetInt32().Should().Be(200);
        segment.GetProperty("ix").GetString().Should().Be("~10");
        segment.GetProperty("cct").GetInt32().Should().Be(5000);
        segment.GetProperty("c1").GetInt32().Should().Be(5);
        segment.GetProperty("c3").GetInt32().Should().Be(31);
        segment.GetProperty("set").GetInt32().Should().Be(2);
        segment.GetProperty("si").GetInt32().Should().Be((byte)SoundSimulation.WeWillRockYou);
    }

    [Fact]
    public void CustomSlider3RejectsOutOfRange()
    {
        var act = () => new StateUpdate().Segment(0, seg => seg.CustomSlider3(32));

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void SetRejectsOutOfRange()
    {
        var act = () => new StateUpdate().Segment(0, seg => seg.Set(4));

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public async Task SegmentBuilderAcceptsSegmentBounds()
    {
        var (_, segment) = await CaptureSegment(seg => seg
            .Range(SegmentBounds.From(5, 25)));

        segment.GetProperty("start").GetInt32().Should().Be(5);
        segment.GetProperty("stop").GetInt32().Should().Be(25);
    }

    [Fact]
    public async Task SegmentBuilderAcceptsMatrixBounds()
    {
        var (_, segment) = await CaptureSegment(seg => seg
            .Range2D(MatrixBounds.From(0, 16, 0, 8)));

        segment.GetProperty("start").GetInt32().Should().Be(0);
        segment.GetProperty("stop").GetInt32().Should().Be(16);
        segment.GetProperty("startY").GetInt32().Should().Be(0);
        segment.GetProperty("stopY").GetInt32().Should().Be(8);
    }

    private static async Task<(string Uri, JsonElement Segment)> CaptureSegment(Action<SegmentUpdate> configure)
    {
        var mockHttpMessageHandler = new MockHttpMessageHandler();
        var baseUri = $"http://{Guid.NewGuid():N}.com";
        mockHttpMessageHandler.AppendResponse($"{baseUri}/json/state");
        var client = new WLedClient(mockHttpMessageHandler, baseUri);

        await client.UpdateState(s => s.Segment(0, configure));

        var (uri, body) = mockHttpMessageHandler.CapturedRequests.Single();
        var root = JsonDocument.Parse(body!).RootElement;
        return (uri, root.GetProperty("seg")[0].Clone());
    }
}
