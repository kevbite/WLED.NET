namespace Kevsoft.WLED;

/// <summary>
/// A single step in a playlist: which preset to show, for how long, and how long to take
/// transitioning to it. Coupling these together makes mismatched parallel arrays impossible.
/// </summary>
public sealed record PlaylistEntry(int PresetId, TimeSpan Duration, TimeSpan? Transition = null);
