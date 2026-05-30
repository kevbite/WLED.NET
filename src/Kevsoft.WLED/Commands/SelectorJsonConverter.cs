namespace Kevsoft.WLED;

/// <summary>Serializes <see cref="Selector"/> as a number, <c>"~"</c>, <c>"~-"</c>, <c>"r"</c> or <c>"from~tor"</c>.</summary>
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
            var value = reader.GetString();
            return value switch
            {
                "~" => Selector.Next,
                "~-" => Selector.Previous,
                "r" => Selector.Random,
                _ => ParseRangedRandom(value),
            };
        }

        throw new JsonException($"Unexpected token '{reader.TokenType}' when reading a selector.");
    }

    private static Selector ParseRangedRandom(string? value)
    {
        // Expected form: "{from}~{to}r", e.g. "5~10r".
        if (value is not null && value.Length > 1 && value[value.Length - 1] == 'r')
        {
            var body = value.Substring(0, value.Length - 1);
            var separator = body.IndexOf('~');
            if (separator > 0 && separator < body.Length - 1)
            {
                var fromText = body.Substring(0, separator);
                var toText = body.Substring(separator + 1);
                if (int.TryParse(fromText, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out var from)
                    && int.TryParse(toText, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out var to)
                    && to >= from)
                {
                    return Selector.RandomInRange(from, to);
                }
            }
        }

        throw new JsonException($"'{value}' is not a valid selector token.");
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
