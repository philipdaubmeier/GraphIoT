using PhilipDaubmeier.WeConnectClient.Model.Core;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PhilipDaubmeier.WeConnectClient.Model
{
    internal class VinConverter : JsonConverter<Vin>
    {
        public override void Write(Utf8JsonWriter writer, Vin value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }

        public override Vin Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return new Vin(reader.GetString());
        }
    }
}