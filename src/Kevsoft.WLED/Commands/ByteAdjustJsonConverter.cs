namespace Kevsoft.WLED;

/// <summary>Serializes <see cref="ByteAdjust"/> as a number or an adjustment token.</summary>
public sealed class ByteAdjustJsonConverter : JsonConverter<ByteAdjust>
{
    public override ByteAdjust Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            return ByteAdjust.Set(reader.GetByte());
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            return ParseToken(reader.GetString()!);
        }

        throw new JsonException($"Unexpected token '{reader.TokenType}' when reading a byte adjustment.");
    }

    public override void Write(Utf8JsonWriter writer, ByteAdjust value, JsonSerializerOptions options)
    {
        if (value.Op == ByteAdjust.Operation.Set)
        {
            writer.WriteNumberValue(value.Amount);
        }
        else
        {
            writer.WriteStringValue(value.ToToken());
        }
    }

    private static ByteAdjust ParseToken(string token)
    {
        if (token.StartsWith("w~", StringComparison.Ordinal))
        {
            return ByteAdjust.IncrementWrap(ParseAmount(token, token.Substring(2), 0));
        }

        if (token == "~")
        {
            return ByteAdjust.Increment();
        }

        if (token == "~-")
        {
            return ByteAdjust.Decrement();
        }

        if (token.StartsWith("~-", StringComparison.Ordinal))
        {
            return ByteAdjust.Decrement(ParseAmount(token, token.Substring(2), 1));
        }

        if (token.StartsWith("~", StringComparison.Ordinal))
        {
            return ByteAdjust.Increment(ParseAmount(token, token.Substring(1), 1));
        }

        throw new JsonException($"'{token}' is not a valid byte adjustment token.");
    }

    private static byte ParseAmount(string token, string text, byte fallback)
    {
        if (text.Length == 0)
        {
            return fallback;
        }

        if (!byte.TryParse(text, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out var amount))
        {
            throw new JsonException($"'{token}' contains an invalid amount.");
        }

        return amount;
    }
}
