namespace Kevsoft.WLED;

/// <summary>
/// Fluent builder for a <see cref="PlaylistDefinition"/>.
/// </summary>
public sealed class PlaylistBuilder
{
    private readonly List<PlaylistEntry> _entries = new();
    private int _repeat;
    private int? _endPresetId;
    private bool _shuffle;

    /// <summary>Add a step that shows <paramref name="preset"/> for <paramref name="duration"/>.</summary>
    public PlaylistBuilder Add(int preset, TimeSpan duration, TimeSpan? transition = null)
    {
        _entries.Add(new PlaylistEntry(preset, duration, transition));
        return this;
    }

    /// <summary>Set how many times the playlist cycles. <c>0</c> means indefinitely.</summary>
    public PlaylistBuilder Repeat(int times)
    {
        _repeat = times;
        return this;
    }

    /// <summary>Apply the given preset once the playlist finishes.</summary>
    public PlaylistBuilder EndOn(int presetId)
    {
        _endPresetId = presetId;
        return this;
    }

    /// <summary>Play the entries in a random order.</summary>
    public PlaylistBuilder Shuffle(bool shuffle = true)
    {
        _shuffle = shuffle;
        return this;
    }

    internal PlaylistDefinition Build() => new()
    {
        Entries = _entries.ToArray(),
        Repeat = _repeat,
        EndPresetId = _endPresetId,
        Shuffle = _shuffle
    };
}
