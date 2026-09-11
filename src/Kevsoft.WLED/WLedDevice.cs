namespace Kevsoft.WLED;

/// <summary>
/// Options controlling what <see cref="IWLedClient.GetDevice"/> includes in the snapshot.
/// </summary>
public sealed class DeviceSnapshotOptions
{
    /// <summary>
    /// When <c>true</c>, the snapshot also fetches and parses effect metadata (<c>/json/fxdata</c>),
    /// which costs one extra HTTP request. Defaults to <c>false</c>.
    /// </summary>
    public bool IncludeEffectMetadata { get; set; }
}

/// <summary>
/// A cohesive, read-only snapshot of a WLED device, combining its state, info and effect/palette
/// catalogs (and optionally effect metadata) from a single <c>GET /json</c> request.
/// </summary>
public sealed class WLedDevice
{
    internal WLedDevice(
        StateResponse state,
        InformationResponse information,
        EffectCatalog effects,
        PaletteCatalog palettes,
        IReadOnlyList<EffectMetadata>? effectMetadata)
    {
        State = state;
        Information = information;
        Effects = effects;
        Palettes = palettes;
        EffectMetadata = effectMetadata;

        var segments = new WLedDeviceSegment[state.Segments?.Length ?? 0];
        for (var i = 0; i < segments.Length; i++)
        {
            segments[i] = new WLedDeviceSegment(state.Segments![i], effects, palettes, effectMetadata);
        }

        Segments = segments;
    }

    /// <summary>The raw device state.</summary>
    public StateResponse State { get; }

    /// <summary>The raw device information.</summary>
    public InformationResponse Information { get; }

    /// <summary>The effects catalog.</summary>
    public EffectCatalog Effects { get; }

    /// <summary>The palettes catalog.</summary>
    public PaletteCatalog Palettes { get; }

    /// <summary>
    /// Parsed effect metadata, or <c>null</c> when it was not requested via
    /// <see cref="DeviceSnapshotOptions.IncludeEffectMetadata"/>.
    /// </summary>
    public IReadOnlyList<EffectMetadata>? EffectMetadata { get; }

    /// <summary>The device's segments as a queryable read model.</summary>
    public IReadOnlyList<WLedDeviceSegment> Segments { get; }

    /// <summary>The friendly device name.</summary>
    public string Name => Information.Name;

    /// <summary>The firmware version name.</summary>
    public string Version => Information.VersionName;

    /// <summary>Whether the light is currently on.</summary>
    public bool IsOn => State.On;

    /// <summary>The master brightness (0–255).</summary>
    public byte Brightness => State.Brightness;

    /// <summary>The id of the currently active preset, or <c>null</c> when none is active.</summary>
    public int? CurrentPresetId => State.PresetId;

    /// <summary>The id of the currently active playlist, or <c>null</c> when none is active.</summary>
    public int? CurrentPlaylistId => State.PlaylistId;

    /// <summary><c>true</c> if the device has a dedicated white channel.</summary>
    public bool SupportsWhiteChannel
#pragma warning disable CS0618 // Type or member is obsolete
        => Information.Leds.LightCapabilities.HasFlag(LightCapability.WhiteChannel) || Information.Leds.Rgbw;
#pragma warning restore CS0618 // Type or member is obsolete

    /// <summary><c>true</c> if the device supports colour temperature (CCT) control.</summary>
    public bool SupportsColorTemperature
#pragma warning disable CS0618 // Type or member is obsolete
        => Information.Leds.LightCapabilities.HasFlag(LightCapability.ColorTemperature) || Information.Leds.SupportsColorTemperature.GetValueOrDefault();
#pragma warning restore CS0618 // Type or member is obsolete

    /// <summary>The segments that are currently selected.</summary>
    public IEnumerable<WLedDeviceSegment> SelectedSegments => Segments.Where(s => s.IsSelected);

    /// <summary>The segments that are valid (non-empty, i.e. <c>stop &gt; start</c>).</summary>
    public IEnumerable<WLedDeviceSegment> ActiveSegments => Segments.Where(s => s.IsActive);

    /// <summary>The main segment (per <c>state.mainseg</c>), or <c>null</c> if it cannot be resolved.</summary>
    public WLedDeviceSegment? MainSegment => Segments.FirstOrDefault(s => s.Id == State.MainSegment);
}

/// <summary>
/// A single segment within a <see cref="WLedDevice"/> snapshot, with its current effect and palette
/// resolved against the device catalogs.
/// </summary>
public sealed class WLedDeviceSegment
{
    private readonly SegmentResponse _segment;

    internal WLedDeviceSegment(
        SegmentResponse segment,
        EffectCatalog effects,
        PaletteCatalog palettes,
        IReadOnlyList<EffectMetadata>? effectMetadata)
    {
        _segment = segment;

        Effect = effects.TryFindById(segment.EffectId, out var effect)
            ? effect
            : new EffectCatalogEntry(segment.EffectId, string.Empty);

        Palette = palettes.TryFindById(segment.ColorPaletteId, out var palette)
            ? palette
            : new PaletteCatalogEntry(segment.ColorPaletteId, string.Empty);

        if (effectMetadata is not null)
        {
            foreach (var metadata in effectMetadata)
            {
                if (metadata.EffectId == segment.EffectId)
                {
                    EffectMetadata = metadata;
                    break;
                }
            }
        }
    }

    /// <summary>The segment id.</summary>
    public int Id => _segment.Id;

    /// <summary>The segment name, or <c>null</c> if unnamed.</summary>
    public string? Name => _segment.Name;

    /// <summary><c>true</c> if the segment is selected.</summary>
    public bool IsSelected => _segment.Selected;

    /// <summary><c>true</c> if the individual segment is powered on.</summary>
    public bool IsOn => _segment.SegmentState;

    /// <summary><c>true</c> if the segment is valid (non-empty, i.e. <c>stop &gt; start</c>).</summary>
    public bool IsActive => _segment.Stop > _segment.Start;

    /// <summary>The segment brightness (0–255).</summary>
    public byte Brightness => _segment.Brightness;

    /// <summary>The segment colour slots.</summary>
    public SegmentColors Colors => _segment.Colors;

    /// <summary>The segment's current effect, resolved from the catalog.</summary>
    public EffectCatalogEntry Effect { get; }

    /// <summary>The segment's current palette, resolved from the catalog.</summary>
    public PaletteCatalogEntry Palette { get; }

    /// <summary>
    /// The metadata for the segment's current effect, or <c>null</c> when metadata was not requested
    /// (or the effect is reserved/unknown).
    /// </summary>
    public EffectMetadata? EffectMetadata { get; }

    /// <summary>The underlying raw segment response.</summary>
    public SegmentResponse Raw => _segment;
}
