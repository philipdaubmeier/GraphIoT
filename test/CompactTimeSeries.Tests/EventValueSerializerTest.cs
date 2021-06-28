using System;
using System.IO;
using System.Text;
using Xunit;

namespace PhilipDaubmeier.CompactTimeSeries.Tests
{
    public class EventValueSerializerTest
    {
        [Theory]
        [InlineData(0)]
        [InlineData(byte.MaxValue)]
        [InlineData(short.MinValue)]
        [InlineData(short.MaxValue)]
        [InlineData(ushort.MinValue)]
        [InlineData(ushort.MaxValue)]
        [InlineData(int.MinValue)]
        [InlineData(int.MaxValue)]
        public void TestEventValueSerializerInt(int value)
        {
            var serializer = new EventValueSerializer<int>();

            Assert.Equal(4, serializer.SizeOf);

            var timestamp = DateTime.UtcNow;
            Tuple<DateTime, int> read;
            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream, Encoding.UTF8, true))
                    serializer.Serialize(writer, new Tuple<DateTime, int>(timestamp, value));

                stream.Seek(0, SeekOrigin.Begin);
                using var reader = new BinaryReader(stream, Encoding.UTF8, true);
                read = serializer.Deserialize(reader, timestamp);
            }

            Assert.Equal(timestamp, read.Item1);
            Assert.Equal(value, read.Item2);
        }

        [Theory]
        [InlineData(0d)]
        [InlineData(42d)]
        [InlineData(42.23003211)]
        [InlineData(double.NaN)]
        [InlineData(double.NegativeInfinity)]
        [InlineData(double.PositiveInfinity)]
        [InlineData(short.MinValue)]
        [InlineData(short.MaxValue)]
        [InlineData(ushort.MinValue)]
        [InlineData(ushort.MaxValue)]
        [InlineData(int.MinValue)]
        [InlineData(int.MaxValue)]
        public void TestEventValueSerializerDouble(double value)
        {
            var serializer = new EventValueSerializer<double>();

            Assert.Equal(8, serializer.SizeOf);

            var timestamp = DateTime.UtcNow;
            Tuple<DateTime, double> read;
            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream, Encoding.UTF8, true))
                    serializer.Serialize(writer, new Tuple<DateTime, double>(timestamp, value));

                stream.Seek(0, SeekOrigin.Begin);
                using var reader = new BinaryReader(stream, Encoding.UTF8, true);
                read = serializer.Deserialize(reader, timestamp);
            }

            Assert.Equal(timestamp, read.Item1);
            Assert.Equal(value, read.Item2);
        }
    }
}