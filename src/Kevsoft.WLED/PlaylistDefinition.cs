namespace Kevsoft.WLED;

/// <summary>
/// A playlist: an ordered set of <see cref="PlaylistEntry"/> steps with repeat/end behaviour.
/// </summary>
public sealed class PlaylistDefinition
{
    /// <summary>The ordered steps that make up the playlist.</summary>
    public IReadOnlyList<PlaylistEntry> Entries { get; init; } = Array.Empty<PlaylistEntry>();

    /// <summary>How many times the playlist cycles before finishing. <c>0</c> means indefinitely.</summary>
    public int Repeat { get; init; }

    /// <summary>The preset to apply once the playlist finishes, or <c>null</c> to stay on the last step.</summary>
    public int? EndPresetId { get; init; }

    /// <summary>Play the entries in a random order.</summary>
    public bool Shuffle { get; init; }
}
