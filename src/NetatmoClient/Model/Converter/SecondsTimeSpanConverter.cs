using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PhilipDaubmeier.NetatmoClient.Model
{
    internal class SecondsTimeSpanConverter : JsonConverter<TimeSpan>
    {
        public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue((int)value.TotalSeconds);
        }

        public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return new TimeSpan(0, 0, Math.Min(int.MaxValue, reader.GetInt32()));
        }
    }
}