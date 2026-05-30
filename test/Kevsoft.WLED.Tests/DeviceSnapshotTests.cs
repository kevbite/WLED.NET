namespace Kevsoft.WLED.Tests;

public class DeviceSnapshotTests
{
    private readonly IFixture _fixture = new Fixture().Customize(new WledFixtureCustomization());

    [Fact]
    public async Task GetDeviceIssuesSingleRootRequest()
    {
        var (client, handler, _) = CreateClient();

        await client.GetDevice();

        handler.CapturedRequestList.Should().ContainSingle()
            .Which.Uri.Should().EndWith("/json");
    }

    [Fact]
    public async Task GetDeviceResolvesCatalogsAndDerivedProperties()
    {
        var (client, _, root) = CreateClient(r =>
        {
            r.Effects = new[] { "Solid", "Blink", "Rainbow" };
            r.Palettes = new[] { "Default", "Party" };
            r.Information.Name = "Kitchen";
            r.Information.VersionName = "0.15.0";
            r.State.Segments[0].EffectId = 2;
            r.State.Segments[0].ColorPaletteId = 1;
        });

        var device = await client.GetDevice();

        device.Name.Should().Be("Kitchen");
        device.Version.Should().Be("0.15.0");
        device.Effects.FindByName("Rainbow").Id.Should().Be(2);
        device.Segments[0].Effect.Name.Should().Be("Rainbow");
        device.Segments[0].Palette.Name.Should().Be("Party");
        device.EffectMetadata.Should().BeNull();
    }

    [Fact]
    public async Task GetDeviceWithoutMetadataDoesNotRequestFxData()
    {
        var (client, handler, _) = CreateClient();

        await client.GetDevice();

        handler.CapturedRequestList.Should().NotContain(r => r.Uri.EndsWith("/json/fxdata"));
    }

    [Fact]
    public async Task GetDeviceWithMetadataFetchesAndAlignsFxData()
    {
        var (client, _, root) = CreateClient(
            configure: r =>
            {
                r.Effects = new[] { "Solid", "Blink", "Rainbow" };
                r.State.Segments[0].EffectId = 2;
            },
            fxData: "[\"\",\"\",\"Speed,Intensity;!,!,!;!;1\"]");

        var device = await client.GetDevice(new DeviceSnapshotOptions { IncludeEffectMetadata = true });

        device.EffectMetadata.Should().NotBeNull();
        device.Segments[0].EffectMetadata.Should().NotBeNull();
        device.Segments[0].EffectMetadata!.Name.Should().Be("Rainbow");
    }

    [Fact]
    public async Task GetDeviceFlagsSelectedAndActiveSegments()
    {
        var (client, _, _) = CreateClient(r =>
        {
            r.State.Segments[0].Selected = true;
            r.State.Segments[0].Start = 0;
            r.State.Segments[0].Stop = 10;
        });

        var device = await client.GetDevice();

        device.SelectedSegments.Should().Contain(s => s.Id == device.Segments[0].Id);
        device.ActiveSegments.Should().Contain(s => s.Id == device.Segments[0].Id);
    }

    [Fact]
    public async Task GetDeviceReportsWhiteChannelSupportFromCapabilities()
    {
        var (client, _, _) = CreateClient(r =>
        {
            r.Information.Leds.LightCapabilities = LightCapability.Rgb | LightCapability.WhiteChannel;
        });

        var device = await client.GetDevice();

        device.SupportsWhiteChannel.Should().BeTrue();
    }

    private (WLedClient Client, MockHttpMessageHandler Handler, WLedRootResponse Root) CreateClient(
        Action<WLedRootResponse>? configure = null,
        string? fxData = null)
    {
        var root = _fixture.Create<WLedRootResponse>();
        configure?.Invoke(root);

        var baseUri = $"http://{Guid.NewGuid():N}.com";
        var handler = new MockHttpMessageHandler();
        handler.AppendResponse($"{baseUri}/json", JsonBuilder.CreateRootResponse(root));
        if (fxData is not null)
        {
            handler.AppendResponse($"{baseUri}/json/fxdata", fxData);
            handler.AppendResponse($"{baseUri}/json/eff",
                "[" + string.Join(",", root.Effects.Select(e => $"\"{e}\"")) + "]");
        }

        return (new WLedClient(handler, baseUri), handler, root);
    }
}
