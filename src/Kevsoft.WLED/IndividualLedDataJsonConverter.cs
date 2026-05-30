namespace Kevsoft.WLED;

internal sealed class IndividualLedDataJsonConverter : JsonConverter<IndividualLedData>
{
    public override IndividualLedData Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => throw new NotSupportedException($"{nameof(IndividualLedData)} is write-only; read current colours from /json/live instead.");

    public override void Write(Utf8JsonWriter writer, IndividualLedData value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();

        foreach (var token in value.Tokens)
        {
            if (token.IsIndex)
            {
                writer.WriteNumberValue(token.Index!.Value);
            }
            else
            {
                var color = token.Color!.Value;
                if (color.IsRgbw)
                {
                    writer.WriteStartArray();
                    writer.WriteNumberValue(color.R);
                    writer.WriteNumberValue(color.G);
                    writer.WriteNumberValue(color.B);
                    writer.WriteNumberValue(color.W!.Value);
                    writer.WriteEndArray();
                }
                else
                {
                    writer.WriteStringValue(color.ToHex());
                }
            }
        }

        writer.WriteEndArray();
    }
}
