using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PhilipDaubmeier.DigitalstromClient.Network
{
    internal class StringToIntConverter : JsonConverter<int>
    {
        public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }

        public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt32(out int number))
                return number;

            if (int.TryParse(reader.GetString(), out int strnumber))
                return strnumber;

            return 0;
        }
    }
}