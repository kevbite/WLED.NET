namespace Kevsoft.WLED;

/// <summary>
/// Unzips a <see cref="PlaylistDefinition"/> into WLED's parallel <c>ps</c>/<c>dur</c>/
/// <c>transition</c> arrays plus <c>repeat</c>/<c>end</c>/<c>r</c>.
/// </summary>
public sealed class PlaylistRequestJsonConverter : JsonConverter<PlaylistRequest>
{
    public override PlaylistRequest Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        throw new NotSupportedException("Playlists are read via /presets.json, not deserialized from a request.");

    public override void Write(Utf8JsonWriter writer, PlaylistRequest value, JsonSerializerOptions options)
    {
        var definition = value.Definition;
        var entries = definition.Entries;

        writer.WriteStartObject();

        writer.WritePropertyName("ps");
        writer.WriteStartArray();
        foreach (var entry in entries)
        {
            writer.WriteNumberValue(entry.PresetId);
        }

        writer.WriteEndArray();

        writer.WritePropertyName("dur");
        writer.WriteStartArray();
        foreach (var entry in entries)
        {
            writer.WriteNumberValue(ToTenths(entry.Duration));
        }

        writer.WriteEndArray();

        if (entries.Any(entry => entry.Transition.HasValue))
        {
            writer.WritePropertyName("transition");
            writer.WriteStartArray();
            foreach (var entry in entries)
            {
                writer.WriteNumberValue(entry.Transition.HasValue ? ToTenths(entry.Transition.Value) : 0);
            }

            writer.WriteEndArray();
        }

        writer.WriteNumber("repeat", definition.Repeat);

        if (definition.EndPresetId is { } end)
        {
            writer.WriteNumber("end", end);
        }

        if (definition.Shuffle)
        {
            writer.WriteBoolean("r", true);
        }

        writer.WriteEndObject();
    }

    internal static int ToTenths(TimeSpan duration)
    {
        var tenths = Math.Round(duration.TotalMilliseconds / 100.0, MidpointRounding.AwayFromZero);
        return tenths < 0 ? 0 : (int)tenths;
    }
}
