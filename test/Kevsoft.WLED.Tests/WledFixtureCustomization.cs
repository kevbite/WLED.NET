namespace Kevsoft.WLED.Tests;

/// <summary>
/// Teaches AutoFixture how to build the library's value types, which only expose
/// validating factory methods rather than public constructors.
/// </summary>
public sealed class WledFixtureCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Register(() => Color.Rgb(
            fixture.Create<byte>(),
            fixture.Create<byte>(),
            fixture.Create<byte>()));

        fixture.Register((Color primary) => new SegmentColors(primary));

        fixture.Register(() => ColorTemperature.Relative(fixture.Create<byte>()));
    }
}
