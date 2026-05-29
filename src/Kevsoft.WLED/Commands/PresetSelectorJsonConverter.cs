namespace Kevsoft.WLED;

/// <summary>Serializes <see cref="PresetSelector"/> as a number or a cycle/random token.</summary>
public sealed class PresetSelectorJsonConverter : JsonConverter<PresetSelector>
{
    public override PresetSelector Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            return PresetSelector.Id(reader.GetInt32());
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            return ParseToken(reader.GetString()!);
        }

        throw new JsonException($"Unexpected token '{reader.TokenType}' when reading a preset selector.");
    }

    public override void Write(Utf8JsonWriter writer, PresetSelector value, JsonSerializerOptions options)
    {
        if (value.Type == PresetSelector.Kind.Id)
        {
            writer.WriteNumberValue(value.From);
        }
        else
        {
            writer.WriteStringValue(value.ToToken());
        }
    }

    private static PresetSelector ParseToken(string token)
    {
        if (token.EndsWith("~", StringComparison.Ordinal))
        {
            var parts = token.TrimEnd('~').Split('~');
            if (parts.Length == 2 && int.TryParse(parts[0], out var from) && int.TryParse(parts[1], out var to))
            {
                return PresetSelector.Cycle(from, to);
            }
        }
        else if (token.EndsWith("r", StringComparison.Ordinal))
        {
            var parts = token.Substring(0, token.Length - 1).Split('~');
            if (parts.Length == 2 && int.TryParse(parts[0], out var from) && int.TryParse(parts[1], out var to))
            {
                return PresetSelector.RandomInRange(from, to);
            }
        }

        throw new JsonException($"'{token}' is not a valid preset selector token.");
    }
}
