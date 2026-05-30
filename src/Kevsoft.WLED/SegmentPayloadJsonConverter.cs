namespace Kevsoft.WLED;

/// <summary>
/// Serializes <see cref="SegmentPayload"/> as either a single object (selected-segment form)
/// or an array of objects (id-targeted form), matching what WLED accepts for <c>seg</c>.
/// </summary>
public sealed class SegmentPayloadJsonConverter : JsonConverter<SegmentPayload>
{
    public override SegmentPayload? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.Null:
                return null;
            case JsonTokenType.StartObject:
                return SegmentPayload.Selected(JsonSerializer.Deserialize<SegmentRequest>(ref reader, options)!);
            case JsonTokenType.StartArray:
                return SegmentPayload.List(JsonSerializer.Deserialize<SegmentRequest[]>(ref reader, options)!);
            default:
                throw new JsonException($"Unexpected token '{reader.TokenType}' when reading a segment payload.");
        }
    }

    public override void Write(Utf8JsonWriter writer, SegmentPayload value, JsonSerializerOptions options)
    {
        if (value.Single is not null)
        {
            JsonSerializer.Serialize(writer, value.Single, options);
        }
        else
        {
            JsonSerializer.Serialize(writer, value.Many ?? Array.Empty<SegmentRequest>(), options);
        }
    }
}
