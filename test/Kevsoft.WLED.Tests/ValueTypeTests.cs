using System;

namespace Kevsoft.WLED.Tests;

public class ValueTypeTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    [InlineData(1024)]
    public void SegmentIdAcceptsZeroOrGreater(int value)
    {
        SegmentId id = value;

        ((int)id).Should().Be(value);
        id.Value.Should().Be(value);
    }

    [Fact]
    public void SegmentIdRejectsNegative()
    {
        Action act = () => SegmentId.From(-1);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void EffectIdRejectsNegative()
    {
        Action act = () => _ = new EffectId(-1);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void PaletteIdRejectsNegative()
    {
        Action act = () => _ = new PaletteId(-1);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(250)]
    public void PresetIdAcceptsValidRange(int value)
    {
        ((int)PresetId.From(value)).Should().Be(value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(251)]
    [InlineData(-1)]
    public void PresetIdRejectsOutOfRange(int value)
    {
        Action act = () => PresetId.From(value);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(251)]
    public void PlaylistIdRejectsOutOfRange(int value)
    {
        Action act = () => PlaylistId.From(value);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(9)]
    public void LedMapIdAcceptsValidRange(int value)
    {
        ((int)LedMapId.From(value)).Should().Be(value);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(10)]
    public void LedMapIdRejectsOutOfRange(int value)
    {
        Action act = () => LedMapId.From(value);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void SegmentBoundsExposesLength()
    {
        var bounds = SegmentBounds.From(10, 30);

        bounds.Start.Should().Be(10);
        bounds.Stop.Should().Be(30);
        bounds.Length.Should().Be(20);
    }

    [Fact]
    public void SegmentBoundsRejectsNegative()
    {
        Action act = () => SegmentBounds.From(-1, 10);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void MatrixBoundsRejectsNegative()
    {
        Action act = () => MatrixBounds.From(0, 8, -1, 8);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void IdsAreValueEqual()
    {
        SegmentId.From(3).Should().Be(SegmentId.From(3));
        (PresetId.From(1) == PresetId.From(1)).Should().BeTrue();
        (LedMapId.From(1) != LedMapId.From(2)).Should().BeTrue();
        SegmentBounds.From(0, 5).Should().Be(SegmentBounds.From(0, 5));
        MatrixBounds.From(0, 8, 0, 8).Should().Be(MatrixBounds.From(0, 8, 0, 8));
    }
}
