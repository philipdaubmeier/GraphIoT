using Newtonsoft.Json;
using System;

namespace PhilipDaubmeier.NetatmoClient.Model
{
    internal class SecondsTimeSpanConverter : JsonConverter<TimeSpan>
    {
        public override void WriteJson(JsonWriter writer, TimeSpan value, JsonSerializer serializer)
        {
            writer.WriteValue((int)value.TotalSeconds);
        }

        public override TimeSpan ReadJson(JsonReader reader, Type objectType, TimeSpan existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return new TimeSpan(0, 0, (int)Math.Min(int.MaxValue, (long)reader.Value));
        }
    }
}