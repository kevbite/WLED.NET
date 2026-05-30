namespace Kevsoft.WLED;

/// <summary>Serializes <see cref="Toggleable"/> as <c>true</c>, <c>false</c> or <c>"t"</c>.</summary>
public sealed class ToggleableJsonConverter : JsonConverter<Toggleable>
{
    public override Toggleable Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.True:
                return Toggleable.On;
            case JsonTokenType.False:
                return Toggleable.Off;
            case JsonTokenType.String:
                var value = reader.GetString();
                if (string.Equals(value, "t", StringComparison.OrdinalIgnoreCase))
                {
                    return Toggleable.Toggle;
                }

                throw new JsonException($"'{value}' is not a valid toggle value.");
            default:
                throw new JsonException($"Unexpected token '{reader.TokenType}' when reading a toggle value.");
        }
    }

    public override void Write(Utf8JsonWriter writer, Toggleable value, JsonSerializerOptions options)
    {
        if (value.IsToggle)
        {
            writer.WriteStringValue("t");
        }
        else
        {
            writer.WriteBooleanValue(value.Value!.Value);
        }
    }
}
