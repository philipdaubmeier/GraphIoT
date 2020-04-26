using PhilipDaubmeier.NetatmoClient.Model.Core;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PhilipDaubmeier.NetatmoClient.Model
{
    internal class MeasureConverter : JsonConverter<Measure>
    {
        public override void Write(Utf8JsonWriter writer, Measure value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }

        public override Measure Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.GetString();
        }
    }
}