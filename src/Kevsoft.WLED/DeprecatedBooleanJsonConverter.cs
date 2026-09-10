using System;
using System.Collections.Generic;
using System.Text;

namespace Kevsoft.WLED
{
    internal sealed class DeprecatedBooleanJsonConverter : JsonConverter<bool?>
    {
        public override bool HandleNull => true;

        public override bool? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.TokenType switch
            {
                JsonTokenType.True or JsonTokenType.False => reader.GetBoolean(),
                JsonTokenType.Number when reader.TryGetInt32(out var value) && value == 0 => null,
                _ => throw new JsonException("Unexpected token type")
            };
        }

        public override void Write(Utf8JsonWriter writer, bool? value, JsonSerializerOptions options)
        {
            if(value.HasValue)
            {
                writer.WriteBooleanValue(value.Value);
            }
            else
            {
                writer.WriteNullValue();
            }
        }
    }
}
