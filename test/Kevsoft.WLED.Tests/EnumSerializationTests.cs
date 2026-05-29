namespace Kevsoft.WLED.Tests;

public class EnumSerializationTests
{
    private static readonly JsonSerializerOptions Options = new();

    [Theory]
    [InlineData(NightlightMode.Instant, 0)]
    [InlineData(NightlightMode.Sunrise, 3)]
    public void NightlightModeSerializesAsNumber(NightlightMode mode, int expected)
    {
        JsonSerializer.Serialize(mode, Options).Should().Be(expected.ToString());
        JsonSerializer.Deserialize<NightlightMode>(expected.ToString(), Options).Should().Be(mode);
    }

    [Theory]
    [InlineData(LiveDataOverride.Off, 0)]
    [InlineData(LiveDataOverride.UntilLiveEnds, 1)]
    [InlineData(LiveDataOverride.UntilReboot, 2)]
    public void LiveDataOverrideSerializesAsNumber(LiveDataOverride value, int expected)
    {
        JsonSerializer.Serialize(value, Options).Should().Be(expected.ToString());
        JsonSerializer.Deserialize<LiveDataOverride>(expected.ToString(), Options).Should().Be(value);
    }

    [Fact]
    public void LightCapabilityFlagsRoundTrip()
    {
        var value = (LightCapability)7;

        value.Should().Be(LightCapability.Rgb | LightCapability.WhiteChannel | LightCapability.ColorTemperature);
        JsonSerializer.Serialize(value, Options).Should().Be("7");
        JsonSerializer.Deserialize<LightCapability>("7", Options).Should().Be(value);
    }

    [Fact]
    public void SyncGroupFlagsRoundTrip()
    {
        var value = SyncGroup.Group1 | SyncGroup.Group8;

        JsonSerializer.Serialize(value, Options).Should().Be("129");
        JsonSerializer.Deserialize<SyncGroup>("129", Options).Should().Be(value);
    }
}
