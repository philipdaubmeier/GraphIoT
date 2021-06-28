using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PhilipDaubmeier.ViessmannClient.Model
{
    internal class ObjectToInferredTypesConverter : JsonConverter<object>
    {
        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            if (value is bool boolean)
                writer.WriteBooleanValue(boolean);

            if (value is long longNumber)
                writer.WriteNumberValue(longNumber);

            if (value is double doubleNumber)
                writer.WriteNumberValue(doubleNumber);

            writer.WriteStringValue(value.ToString());
        }

        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.True)
                return true;

            if (reader.TokenType == JsonTokenType.False)
                return false;

            if (reader.TokenType == JsonTokenType.Number)
            {
                if (reader.TryGetInt64(out long l))
                    return l;

                return reader.GetDouble();
            }

            if (reader.TokenType == JsonTokenType.String)
            {
                if (reader.TryGetDateTime(out DateTime datetime))
                    return datetime;

                return reader.GetString() ?? string.Empty;
            }

            using JsonDocument document = JsonDocument.ParseValue(ref reader);
            return document.RootElement.Clone();
        }
    }
}