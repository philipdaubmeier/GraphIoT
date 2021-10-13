using PhilipDaubmeier.WeConnectClient.Model.Core;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PhilipDaubmeier.WeConnectClient.Model
{
    internal class VinConverter : JsonConverter<Vin?>
    {
        public override void Write(Utf8JsonWriter writer, Vin? value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value ?? string.Empty);
        }

        public override Vin? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            try
            {
                var str = reader.GetString();
                return str is null ? null : new Vin(str);
            }
            catch
            {
                return null;
            }
        }
    }
}