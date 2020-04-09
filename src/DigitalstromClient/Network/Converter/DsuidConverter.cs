using PhilipDaubmeier.DigitalstromClient.Model.Core;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PhilipDaubmeier.DigitalstromClient.Network
{
    internal class DsuidConverter : JsonConverter<Dsuid>
    {
        public override void Write(Utf8JsonWriter writer, Dsuid value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }

        public override Dsuid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return new Dsuid(reader.GetString());
        }
    }
}