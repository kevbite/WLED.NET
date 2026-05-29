namespace Kevsoft.WLED;

/// <summary>Serializes <see cref="Selector"/> as a number, <c>"~"</c>, <c>"~-"</c> or <c>"r"</c>.</summary>
public sealed class SelectorJsonConverter : JsonConverter<Selector>
{
    public override Selector Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            return Selector.Id(reader.GetInt32());
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            return reader.GetString() switch
            {
                "~" => Selector.Next,
                "~-" => Selector.Previous,
                "r" => Selector.Random,
                var value => throw new JsonException($"'{value}' is not a valid selector token."),
            };
        }

        throw new JsonException($"Unexpected token '{reader.TokenType}' when reading a selector.");
    }

    public override void Write(Utf8JsonWriter writer, Selector value, JsonSerializerOptions options)
    {
        if (value.Type == Selector.Kind.Id)
        {
            writer.WriteNumberValue(value.IdValue);
        }
        else
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
