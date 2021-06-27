using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PhilipDaubmeier.SonnenClient.Model
{
    internal class IntToStringConverter : JsonConverter<string?>
    {
        public override void Write(Utf8JsonWriter writer, string? value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value?.ToString());
        }

        public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                if (reader.TryGetInt64(out long l))
                    return l.ToString();

                return reader.GetDouble().ToString();
            }

            return reader.GetString();
        }
    }
}