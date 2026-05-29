namespace Kevsoft.WLED.Tests;

public class ColorTests
{
    [Theory]
    [InlineData("FFAA00", 255, 170, 0)]
    [InlineData("#FFAA00", 255, 170, 0)]
    [InlineData("ffaa00", 255, 170, 0)]
    [InlineData("000000", 0, 0, 0)]
    public void RgbColorParsesHex(string hex, byte r, byte g, byte b)
    {
        var color = RgbColor.FromHex(hex);

        color.Should().Be(new RgbColor(r, g, b));
    }

    [Fact]
    public void RgbColorRoundTripsToHex()
    {
        new RgbColor(255, 170, 0).ToHex().Should().Be("FFAA00");
    }

    [Fact]
    public void RgbwColorParsesAndRoundTrips()
    {
        var color = RgbwColor.FromHex("#FFAA0040");

        color.Should().Be(new RgbwColor(255, 170, 0, 64));
        color.ToHex().Should().Be("FFAA0040");
    }

    [Fact]
    public void RgbColorRejectsRgbwHex()
    {
        var act = () => RgbColor.FromHex("FFAA0040");

        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void RgbwColorRejectsRgbHex()
    {
        var act = () => RgbwColor.FromHex("FFAA00");

        act.Should().Throw<FormatException>();
    }

    [Theory]
    [InlineData("FFAA")]
    [InlineData("GGAA00")]
    [InlineData("")]
    public void FromHexRejectsInvalidValues(string hex)
    {
        var act = () => Color.FromHex(hex);

        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void ColorRgbHasNoWhite()
    {
        var color = Color.Rgb(1, 2, 3);

        color.IsRgbw.Should().BeFalse();
        color.W.Should().BeNull();
        color.ToBytes().Should().Equal((byte)1, (byte)2, (byte)3);
    }

    [Fact]
    public void ColorRgbwHasWhite()
    {
        var color = Color.Rgbw(1, 2, 3, 4);

        color.IsRgbw.Should().BeTrue();
        color.W.Should().Be(4);
        color.ToBytes().Should().Equal((byte)1, (byte)2, (byte)3, (byte)4);
    }

    [Fact]
    public void ImplicitConversionFromRgbColor()
    {
        Color color = new RgbColor(10, 20, 30);

        color.Should().Be(Color.Rgb(10, 20, 30));
    }

    [Fact]
    public void ImplicitConversionFromRgbwColor()
    {
        Color color = new RgbwColor(10, 20, 30, 40);

        color.Should().Be(Color.Rgbw(10, 20, 30, 40));
    }
}
