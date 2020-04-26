using PhilipDaubmeier.DigitalstromClient.Model.Core;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PhilipDaubmeier.DigitalstromClient.Network
{
    internal class GroupNameConverter : JsonConverter<GroupName>
    {
        public override void Write(Utf8JsonWriter writer, GroupName value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }

        public override GroupName Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.GetString();
        }
    }
}