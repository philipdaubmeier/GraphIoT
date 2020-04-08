using Newtonsoft.Json;
using System;

namespace PhilipDaubmeier.NetatmoClient.Model
{
    internal class UnixDateTimeConverter : JsonConverter<DateTime>
    {
        public override void WriteJson(JsonWriter writer, DateTime value, JsonSerializer serializer)
        {
            writer.WriteValue(new DateTimeOffset(value.ToUniversalTime()).ToUnixTimeSeconds());
        }

        public override DateTime ReadJson(JsonReader reader, Type objectType, DateTime existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return DateTimeOffset.FromUnixTimeSeconds((long)(reader.Value ?? 0)).UtcDateTime;
        }
    }
}