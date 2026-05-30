namespace Kevsoft.WLED.Tests;

public class CommandValueTests
{
    private static readonly JsonSerializerOptions Options = new();

    [Fact]
    public void ToggleableTokens()
    {
        JsonSerializer.Serialize(Toggleable.On, Options).Should().Be("true");
        JsonSerializer.Serialize(Toggleable.Off, Options).Should().Be("false");
        JsonSerializer.Serialize(Toggleable.Toggle, Options).Should().Be("\"t\"");
        JsonSerializer.Serialize((Toggleable)true, Options).Should().Be("true");
    }

    [Fact]
    public void ToggleableReads()
    {
        JsonSerializer.Deserialize<Toggleable>("true", Options).Should().Be(Toggleable.On);
        JsonSerializer.Deserialize<Toggleable>("false", Options).Should().Be(Toggleable.Off);
        JsonSerializer.Deserialize<Toggleable>("\"t\"", Options).Should().Be(Toggleable.Toggle);
    }

    [Fact]
    public void ByteAdjustTokens()
    {
        JsonSerializer.Serialize(ByteAdjust.Set(128), Options).Should().Be("128");
        JsonSerializer.Serialize((ByteAdjust)200, Options).Should().Be("200");
        JsonSerializer.Serialize(ByteAdjust.Increment(), Options).Should().Be("\"~\"");
        JsonSerializer.Serialize(ByteAdjust.Increment(10), Options).Should().Be("\"~10\"");
        JsonSerializer.Serialize(ByteAdjust.Decrement(), Options).Should().Be("\"~-\"");
        JsonSerializer.Serialize(ByteAdjust.Decrement(10), Options).Should().Be("\"~-10\"");
        JsonSerializer.Serialize(ByteAdjust.IncrementWrap(40), Options).Should().Be("\"w~40\"");
    }

    [Theory]
    [InlineData("128")]
    [InlineData("\"~\"")]
    [InlineData("\"~10\"")]
    [InlineData("\"~-\"")]
    [InlineData("\"~-10\"")]
    [InlineData("\"w~40\"")]
    public void ByteAdjustRoundTrips(string json)
    {
        var value = JsonSerializer.Deserialize<ByteAdjust>(json, Options);

        JsonSerializer.Serialize(value, Options).Should().Be(json);
    }

    [Fact]
    public void SelectorTokens()
    {
        JsonSerializer.Serialize(Selector.Id(5), Options).Should().Be("5");
        JsonSerializer.Serialize((Selector)7, Options).Should().Be("7");
        JsonSerializer.Serialize(Selector.Next, Options).Should().Be("\"~\"");
        JsonSerializer.Serialize(Selector.Previous, Options).Should().Be("\"~-\"");
        JsonSerializer.Serialize(Selector.Random, Options).Should().Be("\"r\"");
        JsonSerializer.Serialize(Selector.RandomInRange(5, 10), Options).Should().Be("\"5~10r\"");
    }

    [Fact]
    public void SelectorRoundTrips()
    {
        JsonSerializer.Deserialize<Selector>("5", Options).Should().Be(Selector.Id(5));
        JsonSerializer.Deserialize<Selector>("\"~\"", Options).Should().Be(Selector.Next);
        JsonSerializer.Deserialize<Selector>("\"~-\"", Options).Should().Be(Selector.Previous);
        JsonSerializer.Deserialize<Selector>("\"r\"", Options).Should().Be(Selector.Random);
        JsonSerializer.Deserialize<Selector>("\"5~10r\"", Options).Should().Be(Selector.RandomInRange(5, 10));
    }

    [Fact]
    public void SelectorRejectsInvalidRange()
    {
        var act = () => Selector.RandomInRange(10, 5);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void SelectorRejectsInvalidToken()
    {
        var act = () => JsonSerializer.Deserialize<Selector>("\"nope\"", Options);

        act.Should().Throw<JsonException>();
    }

    [Fact]
    public void PresetSelectorTokens()
    {
        JsonSerializer.Serialize(PresetSelector.Id(3), Options).Should().Be("3");
        JsonSerializer.Serialize(PresetSelector.Cycle(1, 6), Options).Should().Be("\"1~6~\"");
        JsonSerializer.Serialize(PresetSelector.RandomInRange(4, 10), Options).Should().Be("\"4~10r\"");
    }

    [Fact]
    public void PresetSelectorRejectsInvalidRange()
    {
        var act = () => PresetSelector.Cycle(6, 1);

        act.Should().Throw<ArgumentException>();
    }
}
