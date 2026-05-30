namespace Kevsoft.WLED;

/// <summary>
/// Options controlling how the current device state is captured when saving a preset.
/// </summary>
/// <remarks>
/// Saving a preset persists the device's <em>current live state</em>, not an arbitrary
/// state you supply. Set the desired state first, then save.
/// </remarks>
public sealed class SavePresetOptions
{
    /// <summary>The preset name.</summary>
    public string? Name { get; init; }

    /// <summary>The quick-load label shown in the UI.</summary>
    public string? QuickLabel { get; init; }

    /// <summary>Save each segment's start/stop bounds with the preset. Defaults to <c>true</c>.</summary>
    public bool SaveSegmentBounds { get; init; } = true;

    /// <summary>Include the master brightness in the preset. Defaults to <c>true</c>.</summary>
    public bool IncludeBrightness { get; init; } = true;

    /// <summary>Save which segments are selected with the preset. Defaults to <c>true</c>.</summary>
    public bool SaveSelectedSegments { get; init; } = true;
}
