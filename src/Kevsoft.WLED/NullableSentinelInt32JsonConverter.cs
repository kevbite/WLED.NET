namespace Kevsoft.WLED;

/// <summary>
/// Maps WLED's <c>-1</c> "none" sentinel to <c>null</c> when reading, and writes
/// <c>null</c> back as <c>-1</c>.
/// </summary>
public sealed class NullableSentinelInt32JsonConverter : JsonConverter<int?>
{
    public override int? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        var value = reader.GetInt32();
        return value < 0 ? null : value;
    }

    public override void Write(Utf8JsonWriter writer, int? value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value ?? -1);
    }
}
