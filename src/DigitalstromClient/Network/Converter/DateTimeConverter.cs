using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PhilipDaubmeier.DigitalstromClient.Network
{
    internal class DateTimeConverter : JsonConverter<DateTime>
    {
        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var str = reader.GetString();

            if (DateTime.TryParseExact(str, "yyyy'-'MM'-'dd HH':'mm':'ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt))
                return dt;

            if (DateTime.TryParseExact(str, "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTime utcDt))
                return utcDt.ToUniversalTime();

            return DateTime.Parse(str);
        }
    }
}