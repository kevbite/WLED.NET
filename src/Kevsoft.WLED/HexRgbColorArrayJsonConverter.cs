namespace Kevsoft.WLED;

/// <summary>
/// Reads an array of hex colour strings (e.g. <c>["FF0000","00FF00"]</c>) into
/// <see cref="RgbColor"/> values.
/// </summary>
public sealed class HexRgbColorArrayJsonConverter : JsonConverter<RgbColor[]>
{
    public override RgbColor[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
        {
            throw new JsonException($"Unexpected token '{reader.TokenType}' when reading a colour array.");
        }

        var colors = new List<RgbColor>();
        while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
        {
            colors.Add(RgbColor.FromHex(reader.GetString()!));
        }

        return colors.ToArray();
    }

    public override void Write(Utf8JsonWriter writer, RgbColor[] value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        foreach (var color in value)
        {
            writer.WriteStringValue(color.ToHex());
        }

        writer.WriteEndArray();
    }
}
