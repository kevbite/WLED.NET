namespace Kevsoft.WLED.Tests;

public class CatalogTests
{
    private static readonly string[] Effects =
    {
        "Solid", "Blink", "RSVD", "Rainbow", "-", "Android"
    };

    private static readonly string[] Palettes =
    {
        "Default", "Random Cycle", "Party", "Aurora"
    };

    [Fact]
    public void FromNamesAlignsEntryIdWithIndex()
    {
        var catalog = EffectCatalog.FromNames(Effects);

        catalog.Count.Should().Be(6);
        catalog[3].Should().Be(new EffectCatalogEntry(3, "Rainbow"));
    }

    [Fact]
    public void ReservedEntriesAreFlagged()
    {
        var catalog = EffectCatalog.FromNames(Effects);

        catalog.FindById(2).IsReserved.Should().BeTrue();
        catalog.FindById(4).IsReserved.Should().BeTrue();
        catalog.FindById(0).IsReserved.Should().BeFalse();
    }

    [Fact]
    public void AvailableOnlyExcludesReservedPlaceholders()
    {
        var catalog = EffectCatalog.FromNames(Effects);

        catalog.AvailableOnly().Select(x => x.Name)
            .Should().Equal("Solid", "Blink", "Rainbow", "Android");
    }

    [Fact]
    public void FindByIdThrowsWhenMissing()
    {
        var catalog = EffectCatalog.FromNames(Effects);

        catalog.Invoking(c => c.FindById(99)).Should().Throw<KeyNotFoundException>();
    }

    [Fact]
    public void FindByNameIsCaseInsensitive()
    {
        var catalog = EffectCatalog.FromNames(Effects);

        catalog.FindByName("rainbow").Id.Should().Be(3);
    }

    [Fact]
    public void FindByNameThrowsWhenMissing()
    {
        var catalog = EffectCatalog.FromNames(Effects);

        catalog.Invoking(c => c.FindByName("Nope")).Should().Throw<KeyNotFoundException>();
    }

    [Fact]
    public void FindByNameThrowsWhenAmbiguous()
    {
        // "RSVD" appears once, "-" once, but both reserved; craft a duplicate to prove ambiguity throws.
        var catalog = EffectCatalog.FromNames(new[] { "Glow", "glow" });

        catalog.Invoking(c => c.FindByName("Glow")).Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void TryFindByNameReturnsFalseWhenAmbiguous()
    {
        var catalog = EffectCatalog.FromNames(new[] { "Glow", "glow" });

        catalog.TryFindByName("Glow", out _).Should().BeFalse();
    }

    [Fact]
    public void TryFindByNameReturnsMatch()
    {
        var catalog = PaletteCatalog.FromNames(Palettes);

        catalog.TryFindByName("aurora", out var entry).Should().BeTrue();
        entry.Id.Should().Be(3);
    }

    [Fact]
    public async Task GetEffectCatalogReadsNamedEntries()
    {
        var baseUri = $"http://{Guid.NewGuid():N}.com";
        var mockHttpMessageHandler = new MockHttpMessageHandler();
        mockHttpMessageHandler.AppendResponse($"{baseUri}/json/eff", "[\"Solid\",\"Blink\",\"Rainbow\"]");
        var client = new WLedClient(mockHttpMessageHandler, baseUri);

        var catalog = await client.GetEffectCatalog();

        catalog.FindByName("Rainbow").Id.Should().Be(2);
    }

    [Fact]
    public async Task GetPaletteCatalogReadsNamedEntries()
    {
        var baseUri = $"http://{Guid.NewGuid():N}.com";
        var mockHttpMessageHandler = new MockHttpMessageHandler();
        mockHttpMessageHandler.AppendResponse($"{baseUri}/json/pal", "[\"Default\",\"Party\",\"Aurora\"]");
        var client = new WLedClient(mockHttpMessageHandler, baseUri);

        var catalog = await client.GetPaletteCatalog();

        catalog.FindByName("Aurora").Id.Should().Be(2);
    }

    [Fact]
    public async Task SetEffectFromCatalogEntryPostsId()
    {
        var (_, body) = await Capture(client => client.SetEffect(new EffectCatalogEntry(9, "Rainbow")));

        var segment = JsonDocument.Parse(body!).RootElement.GetProperty("seg");
        segment.ValueKind.Should().Be(JsonValueKind.Object);
        segment.GetProperty("fx").GetInt32().Should().Be(9);
    }

    [Fact]
    public async Task SetPaletteFromCatalogEntryWithIdTargetsSegment()
    {
        var (_, body) = await Capture(client => client.SetPalette(new PaletteCatalogEntry(3, "Aurora"), segmentId: 1));

        var seg = JsonDocument.Parse(body!).RootElement.GetProperty("seg");
        seg.ValueKind.Should().Be(JsonValueKind.Array);
        var segment = seg.EnumerateArray().Single();
        segment.GetProperty("id").GetInt32().Should().Be(1);
        segment.GetProperty("pal").GetInt32().Should().Be(3);
    }

    [Fact]
    public async Task SetEffectFromReservedEntryThrows()
    {
        var mockHttpMessageHandler = new MockHttpMessageHandler();
        var baseUri = $"http://{Guid.NewGuid():N}.com";
        var client = new WLedClient(mockHttpMessageHandler, baseUri);

        await client.Invoking(c => c.SetEffect(new EffectCatalogEntry(2, "RSVD")))
            .Should().ThrowAsync<ArgumentException>();
    }

    private static async Task<(string Uri, string? Body)> Capture(Func<WLedClient, Task> act)
    {
        var mockHttpMessageHandler = new MockHttpMessageHandler();
        var baseUri = $"http://{Guid.NewGuid():N}.com";
        mockHttpMessageHandler.AppendResponse($"{baseUri}/json/state");
        var client = new WLedClient(mockHttpMessageHandler, baseUri);

        await act(client);

        var (uri, body) = mockHttpMessageHandler.CapturedRequestList.Single();
        return (uri, body);
    }
}
