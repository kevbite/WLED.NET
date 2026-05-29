namespace Kevsoft.WLED.Tests;

public class ColorJsonTests
{
    private static readonly JsonSerializerOptions Options = new();

    [Fact]
    public void ReadsThreeByteArray()
    {
        var color = JsonSerializer.Deserialize<Color>("[255,170,0]", Options);

        color.Should().Be(Color.Rgb(255, 170, 0));
    }

    [Fact]
    public void ReadsFourByteArray()
    {
        var color = JsonSerializer.Deserialize<Color>("[255,170,0,64]", Options);

        color.Should().Be(Color.Rgbw(255, 170, 0, 64));
    }

    [Fact]
    public void ReadsHexString()
    {
        var color = JsonSerializer.Deserialize<Color>("\"FFAA00\"", Options);

        color.Should().Be(Color.Rgb(255, 170, 0));
    }

    [Fact]
    public void WritesRgbAsArray()
    {
        var json = JsonSerializer.Serialize(Color.Rgb(255, 170, 0), Options);

        json.Should().Be("[255,170,0]");
    }

    [Fact]
    public void WritesRgbwAsArray()
    {
        var json = JsonSerializer.Serialize(Color.Rgbw(255, 170, 0, 64), Options);

        json.Should().Be("[255,170,0,64]");
    }

    [Fact]
    public void SegmentColorsReadsMixedSlots()
    {
        var colors = JsonSerializer.Deserialize<SegmentColors>("[[255,170,0],\"00FF00\",[0,0,0,128]]", Options);

        colors.Should().Be(new SegmentColors(
            Color.Rgb(255, 170, 0),
            Color.Rgb(0, 255, 0),
            Color.Rgbw(0, 0, 0, 128)));
    }

    [Fact]
    public void SegmentColorsRoundTrips()
    {
        var colors = new SegmentColors(Color.Rgb(1, 2, 3), Color.Rgb(4, 5, 6));

        var json = JsonSerializer.Serialize(colors, Options);

        json.Should().Be("[[1,2,3],[4,5,6]]");
    }

    [Fact]
    public void SegmentColorsPrimaryOnlyOmitsTrailingSlots()
    {
        var colors = new SegmentColors(Color.Rgb(1, 2, 3));

        JsonSerializer.Serialize(colors, Options).Should().Be("[[1,2,3]]");
    }
}
