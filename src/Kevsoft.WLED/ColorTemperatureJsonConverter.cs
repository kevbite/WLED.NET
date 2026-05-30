namespace Kevsoft.WLED;

/// <summary>Serializes <see cref="ColorTemperature"/> as its numeric relative or Kelvin value.</summary>
public sealed class ColorTemperatureJsonConverter : JsonConverter<ColorTemperature>
{
    public override ColorTemperature Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.Number)
        {
            throw new JsonException($"Unexpected token '{reader.TokenType}' when reading a colour temperature.");
        }

        return ColorTemperature.FromWire(reader.GetInt32());
    }

    public override void Write(Utf8JsonWriter writer, ColorTemperature value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.Value);
    }
}
