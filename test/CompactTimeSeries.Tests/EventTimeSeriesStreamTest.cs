using System;
using System.Linq;
using Xunit;

namespace PhilipDaubmeier.CompactTimeSeries.Tests
{
    public class EventTimeSeriesStreamTest
    {
        private const int count = 10;
        private static readonly DateTime begin = new(2019, 01, 09, 13, 23, 00, DateTimeKind.Utc);
        private static readonly DateTime end = begin.AddMinutes(count);
        private static readonly TimeSeriesSpan span = new(begin, end, count);

        [Fact]
        public void TestEventTimeSeriesStreamIntSeries()
        {
            using var eventstream = new EventTimeSeriesStream<Tuple<DateTime, int>, EventValueSerializer<int>>(span);

            var expectedEmpty = Enumerable.Range(0, count).SelectMany(x => new byte[] { 0xff, 0xff, 0xff, 0xff, 0x00, 0x00, 0x00, 0x00 }).ToArray();
            Assert.Equal(expectedEmpty, eventstream.ToByteArray());

            eventstream.WriteEvent(new Tuple<DateTime, int>(begin.AddSeconds(3), 42));

            var expected1Item = Enumerable.Range(0, count).SelectMany(x => x == 0 ? new byte[] { 0xb8, 0x0b, 0x00, 0x00, 0x2a, 0x00, 0x00, 0x00 } :
                                                                                    new byte[] { 0xff, 0xff, 0xff, 0xff, 0x00, 0x00, 0x00, 0x00 }).ToArray();
            Assert.Equal(expected1Item, eventstream.ToByteArray());

            eventstream.WriteEvent(new Tuple<DateTime, int>(begin.AddSeconds(7), -1));

            var expected2Item = Enumerable.Range(0, count).SelectMany(x => x == 0 ? new byte[] { 0xb8, 0x0b, 0x00, 0x00, 0x2a, 0x00, 0x00, 0x00 } :
                                                                           x == 1 ? new byte[] { 0x58, 0x1b, 0x00, 0x00, 0xff, 0xff, 0xff, 0xff } :
                                                                                    new byte[] { 0xff, 0xff, 0xff, 0xff, 0x00, 0x00, 0x00, 0x00 }).ToArray();
            Assert.Equal(expected2Item, eventstream.ToByteArray());

            for (int i = 0; i < 8; i++)
                eventstream.WriteEvent(new Tuple<DateTime, int>(begin.AddSeconds(7).AddMilliseconds(i + 1), -1));

            var expected10Items = Enumerable.Range(0, count).SelectMany(x => x == 0 ? new byte[] { 0xb8, 0x0b, 0x00, 0x00, 0x2a, 0x00, 0x00, 0x00 } :
                                                                                      new byte[] { (byte)(0x57 + x), 0x1b, 0x00, 0x00, 0xff, 0xff, 0xff, 0xff }).ToArray();
            Assert.Equal(expected10Items, eventstream.ToByteArray());
        }

        [Theory]
        [InlineData(0)]
        [InlineData(byte.MaxValue)]
        [InlineData(short.MinValue)]
        [InlineData(short.MaxValue)]
        [InlineData(ushort.MinValue)]
        [InlineData(ushort.MaxValue)]
        [InlineData(int.MinValue)]
        [InlineData(int.MaxValue)]
        public void TestEventTimeSeriesStreamIntValues(int value)
        {
            using var eventstream = new EventTimeSeriesStream<Tuple<DateTime, int>, EventValueSerializer<int>>(span);

            for (int i = 0; i < count; i++)
                eventstream.WriteEvent(new Tuple<DateTime, int>(begin.AddSeconds(7).AddMilliseconds(i + 1), value));

            var bytes = eventstream.ToByteArray();
            Assert.NotNull(bytes);

            var neweventstream = EventTimeSeriesStream<Tuple<DateTime, int>, EventValueSerializer<int>>.FromByteArray(span, bytes!);

            Assert.Equal(count, neweventstream.Count());

            foreach (var item in neweventstream)
                Assert.Equal(value, item.Item2);
        }
    }
}