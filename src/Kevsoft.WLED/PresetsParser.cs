namespace Kevsoft.WLED;

/// <summary>
/// Parses the <c>/presets.json</c> file, which is an object keyed by preset id where each
/// value is either a preset or a playlist. Playlists and the scratch slot (id 0) are excluded.
/// </summary>
internal static class PresetsParser
{
    public static IReadOnlyDictionary<int, Preset> ParsePresets(string json, JsonSerializerOptions options)
    {
        using var document = JsonDocument.Parse(json);
        var presets = new Dictionary<int, Preset>();

        foreach (var property in document.RootElement.EnumerateObject())
        {
            if (!int.TryParse(property.Name, out var id) || id == 0)
            {
                continue;
            }

            var element = property.Value;
            if (element.ValueKind != JsonValueKind.Object || !HasAnyProperty(element) || IsPlaylist(element))
            {
                continue;
            }

            presets[id] = ToPreset(id, element, options);
        }

        return presets;
    }

    internal static bool IsPlaylist(JsonElement element) =>
        element.TryGetProperty("playlist", out var playlist)
        && playlist.ValueKind == JsonValueKind.Object
        && playlist.TryGetProperty("ps", out var presetIds)
        && presetIds.ValueKind == JsonValueKind.Array
        && presetIds.GetArrayLength() > 0;

    private static bool HasAnyProperty(JsonElement element)
    {
        foreach (var _ in element.EnumerateObject())
        {
            return true;
        }

        return false;
    }

    private static Preset ToPreset(int id, JsonElement element, JsonSerializerOptions options)
    {
        var name = element.TryGetProperty("n", out var n) && n.ValueKind == JsonValueKind.String
            ? n.GetString()
            : null;
        var quickLabel = element.TryGetProperty("ql", out var ql) && ql.ValueKind == JsonValueKind.String
            ? ql.GetString()
            : null;
        var on = element.TryGetProperty("on", out var onElement)
                 && (onElement.ValueKind == JsonValueKind.True
                     || (onElement.ValueKind == JsonValueKind.Number && onElement.GetInt32() != 0));
        var mainSegment = element.TryGetProperty("mainseg", out var mainSegElement)
                          && mainSegElement.ValueKind == JsonValueKind.Number
            ? mainSegElement.GetInt32()
            : 0;

        var segments = element.TryGetProperty("seg", out var seg) && seg.ValueKind == JsonValueKind.Array
            ? seg.Deserialize<SegmentResponse[]>(options) ?? Array.Empty<SegmentResponse>()
            : Array.Empty<SegmentResponse>();

        return new Preset(id, name, quickLabel, on, mainSegment, segments);
    }
}
