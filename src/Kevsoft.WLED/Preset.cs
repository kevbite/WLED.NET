namespace Kevsoft.WLED;

/// <summary>
/// A saved WLED preset: a named snapshot of the device state that can be re-applied.
/// </summary>
public sealed record Preset(
    int Id,
    string? Name,
    string? QuickLabel,
    bool On,
    int MainSegmentId,
    IReadOnlyList<SegmentResponse> Segments);
