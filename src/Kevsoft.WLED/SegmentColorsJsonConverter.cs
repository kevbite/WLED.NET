namespace Kevsoft.WLED;

/// <summary>
/// Reads and writes a segment's <c>col</c> array, which holds up to three color slots.
/// </summary>
public sealed class SegmentColorsJsonConverter : JsonConverter<SegmentColors>
{
    private static readonly ColorJsonConverter ColorConverter = new();

    public override SegmentColors Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
        {
            throw new JsonException($"Unexpected token '{reader.TokenType}' when reading segment colors.");
        }

        var slots = new List<Color>();
        while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
        {
            slots.Add(ColorConverter.Read(ref reader, typeof(Color), options));
        }

        return SegmentColors.FromSlots(slots);
    }

    public override void Write(Utf8JsonWriter writer, SegmentColors value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        foreach (var color in value.Slots)
        {
            ColorConverter.Write(writer, color, options);
        }

        writer.WriteEndArray();
    }
}
