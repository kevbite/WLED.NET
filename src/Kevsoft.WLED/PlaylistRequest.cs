namespace Kevsoft.WLED;

/// <summary>
/// Write-only representation of a playlist, serialized into WLED's parallel
/// <c>ps</c>/<c>dur</c>/<c>transition</c> arrays by <see cref="PlaylistRequestJsonConverter"/>.
/// </summary>
[JsonConverter(typeof(PlaylistRequestJsonConverter))]
public sealed class PlaylistRequest
{
    internal PlaylistDefinition Definition { get; }

    internal PlaylistRequest(PlaylistDefinition definition) => Definition = definition;

    /// <summary>Creates a request from a playlist definition.</summary>
    public static PlaylistRequest From(PlaylistDefinition definition)
    {
        if (definition is null)
        {
            throw new ArgumentNullException(nameof(definition));
        }

        return new PlaylistRequest(definition);
    }
}
