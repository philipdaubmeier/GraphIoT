using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PhilipDaubmeier.DigitalstromClient.Network
{
    internal class IntWrapperConverter<T> : JsonConverter<T>
    {
        private readonly Func<T, int> _convertToInt;
        private readonly Func<int, T> _convertFromInt;

        public IntWrapperConverter(Func<T, int> convertToInt, Func<int, T> convertFromInt)
            => (_convertToInt, _convertFromInt) = (convertToInt, convertFromInt);

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(_convertToInt(value));
        }

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt32(out int number))
                return _convertFromInt(number);

            if (int.TryParse(reader.GetString(), out int strnumber))
                return _convertFromInt(strnumber);

            return _convertFromInt(0);
        }
    }
}