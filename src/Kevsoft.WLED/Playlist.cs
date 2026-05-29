namespace Kevsoft.WLED;

/// <summary>
/// A saved WLED playlist read from <c>presets.json</c>.
/// </summary>
public sealed record Playlist(int Id, string? Name, PlaylistDefinition Definition);
