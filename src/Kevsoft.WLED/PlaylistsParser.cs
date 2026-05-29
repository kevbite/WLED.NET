namespace Kevsoft.WLED;

/// <summary>
/// Parses playlist entries out of the <c>/presets.json</c> file, zipping WLED's parallel
/// <c>ps</c>/<c>dur</c>/<c>transition</c> arrays back into typed <see cref="PlaylistEntry"/> steps.
/// </summary>
internal static class PlaylistsParser
{
    public static IReadOnlyDictionary<int, Playlist> ParsePlaylists(string json)
    {
        using var document = JsonDocument.Parse(json);
        var playlists = new Dictionary<int, Playlist>();

        foreach (var property in document.RootElement.EnumerateObject())
        {
            if (!int.TryParse(property.Name, out var id) || id == 0)
            {
                continue;
            }

            var element = property.Value;
            if (element.ValueKind != JsonValueKind.Object || !PresetsParser.IsPlaylist(element))
            {
                continue;
            }

            var name = element.TryGetProperty("n", out var n) && n.ValueKind == JsonValueKind.String
                ? n.GetString()
                : null;

            playlists[id] = new Playlist(id, name, ToDefinition(element.GetProperty("playlist")));
        }

        return playlists;
    }

    private static PlaylistDefinition ToDefinition(JsonElement playlist)
    {
        var presetIds = playlist.GetProperty("ps");
        var count = presetIds.GetArrayLength();

        var durations = ReadTenthsArray(playlist, "dur", count);
        var transitions = ReadTenthsArray(playlist, "transition", count);

        var entries = new List<PlaylistEntry>(count);
        for (var i = 0; i < count; i++)
        {
            entries.Add(new PlaylistEntry(
                presetIds[i].GetInt32(),
                durations is { } d ? d[i] : TimeSpan.Zero,
                transitions is { } t ? t[i] : null));
        }

        var repeat = playlist.TryGetProperty("repeat", out var repeatElement) && repeatElement.ValueKind == JsonValueKind.Number
            ? repeatElement.GetInt32()
            : 0;
        int? end = playlist.TryGetProperty("end", out var endElement) && endElement.ValueKind == JsonValueKind.Number
            ? endElement.GetInt32()
            : null;
        var shuffle = playlist.TryGetProperty("r", out var shuffleElement)
                      && (shuffleElement.ValueKind == JsonValueKind.True
                          || (shuffleElement.ValueKind == JsonValueKind.Number && shuffleElement.GetInt32() != 0));

        return new PlaylistDefinition
        {
            Entries = entries,
            Repeat = repeat,
            EndPresetId = end,
            Shuffle = shuffle
        };
    }

    /// <summary>
    /// Reads a field that WLED stores either as a scalar (broadcast to every entry) or as an
    /// array. Returns <c>null</c> when the field is absent.
    /// </summary>
    private static TimeSpan[]? ReadTenthsArray(JsonElement playlist, string name, int count)
    {
        if (!playlist.TryGetProperty(name, out var element))
        {
            return null;
        }

        var values = new TimeSpan[count];
        if (element.ValueKind == JsonValueKind.Number)
        {
            var broadcast = FromTenths(element.GetInt32());
            for (var i = 0; i < count; i++)
            {
                values[i] = broadcast;
            }

            return values;
        }

        if (element.ValueKind != JsonValueKind.Array)
        {
            return null;
        }

        var length = element.GetArrayLength();
        for (var i = 0; i < count; i++)
        {
            var index = length == 0 ? 0 : Math.Min(i, length - 1);
            values[i] = length == 0 ? TimeSpan.Zero : FromTenths(element[index].GetInt32());
        }

        return values;
    }

    private static TimeSpan FromTenths(int tenths) => TimeSpan.FromMilliseconds(tenths * 100.0);
}
