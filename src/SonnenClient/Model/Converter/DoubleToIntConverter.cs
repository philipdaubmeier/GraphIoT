using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PhilipDaubmeier.SonnenClient.Model
{
    internal class DoubleToIntConverter : JsonConverter<int?>
    {
        public override void Write(Utf8JsonWriter writer, int? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
                writer.WriteNumberValue(value.Value);
            else
                writer.WriteNullValue();
        }

        public override int? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
                return null;

            if (reader.TokenType == JsonTokenType.Number)
            {
                if (reader.TryGetInt64(out long l))
                    return (int)l;

                return (int)reader.GetDouble();
            }

            return null;
        }
    }
}