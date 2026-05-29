namespace Kevsoft.WLED;

/// <summary>
/// Reads a WLED color, which may be a 3/4 element numeric array (<c>[255,170,0]</c>) or a
/// hex string (<c>"FFAA00"</c>), and always writes it as a numeric array.
/// </summary>
public sealed class ColorJsonConverter : JsonConverter<Color>
{
    public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            return Color.FromHex(reader.GetString()!);
        }

        if (reader.TokenType != JsonTokenType.StartArray)
        {
            throw new JsonException($"Unexpected token '{reader.TokenType}' when reading a color.");
        }

        Span<byte> components = stackalloc byte[4];
        var count = 0;
        while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
        {
            if (count >= 4)
            {
                throw new JsonException("A color may contain at most 4 components.");
            }

            components[count++] = reader.GetByte();
        }

        return count switch
        {
            3 => Color.Rgb(components[0], components[1], components[2]),
            4 => Color.Rgbw(components[0], components[1], components[2], components[3]),
            _ => throw new JsonException($"A color must contain 3 (RGB) or 4 (RGBW) components but had {count}."),
        };
    }

    public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        writer.WriteNumberValue(value.R);
        writer.WriteNumberValue(value.G);
        writer.WriteNumberValue(value.B);
        if (value.IsRgbw)
        {
            writer.WriteNumberValue(value.W!.Value);
        }

        writer.WriteEndArray();
    }
}
