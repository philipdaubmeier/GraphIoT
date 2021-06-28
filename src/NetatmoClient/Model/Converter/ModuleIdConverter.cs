using PhilipDaubmeier.NetatmoClient.Model.Core;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PhilipDaubmeier.NetatmoClient.Model
{
    internal class ModuleIdConverter : JsonConverter<ModuleId>
    {
        public override void Write(Utf8JsonWriter writer, ModuleId value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }

        public override ModuleId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.GetString() ?? string.Empty;
        }
    }
}