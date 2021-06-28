using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PhilipDaubmeier.SonnenClient.Model
{
    internal class ResolutionConverter : JsonConverter<Resolution>
    {
        public override void Write(Utf8JsonWriter writer, Resolution value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }

        public override Resolution Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return new Resolution(reader.GetString() ?? string.Empty);
        }
    }
}