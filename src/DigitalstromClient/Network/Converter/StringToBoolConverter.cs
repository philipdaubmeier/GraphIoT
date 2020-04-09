using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PhilipDaubmeier.DigitalstromClient.Network
{
    internal class StringToBoolConverter : JsonConverter<bool>
    {
        public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }

        public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.True)
                return true;

            if (reader.TokenType == JsonTokenType.False)
                return false;

            if (reader.GetString().Trim().Equals("true", StringComparison.InvariantCultureIgnoreCase))
                return true;

            return false;
        }
    }
}