using PhilipDaubmeier.ViessmannClient.Model.Features;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PhilipDaubmeier.ViessmannClient.Model
{
    internal class ScheduleMessageConverter : JsonConverter<ScheduleOrMessage>
    {
        public override void Write(Utf8JsonWriter writer, ScheduleOrMessage value, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }

        public override ScheduleOrMessage Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var result = new ScheduleOrMessage();
            try { result.Schedule = JsonSerializer.Deserialize<Schedule>(ref reader, options); } catch { }
            try { result.Messages = JsonSerializer.Deserialize<List<Message>>(ref reader, options); } catch { }

            return result;
        }
    }
}