using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PhilipDaubmeier.CompactTimeSeries.Tests
{
    public class TimeSeriesCompressorTest
    {
        private const int count = 10;
        private static readonly DateTime begin = new DateTime(2019, 01, 09, 13, 23, 00, DateTimeKind.Utc);
        private static readonly DateTime end = begin.AddMinutes(count);
        private static readonly TimeSeriesSpan span = new TimeSeriesSpan(begin, end, count);

        [Fact]
        public void TestIntTimeSeriesCompression()
        {
            var timeseriesCollection = new TimeSeriesStreamCollection<byte, int>(
                new List<byte>() { 0x01 }, 1, (b, stream) => stream.WriteByte(b), span);
            var timeseries = timeseriesCollection[0x01];
            timeseries[0] = 1;
            timeseries[3] = 23;
            timeseries[5] = 42;
            timeseries[7] = 43;
            timeseries[9] = 99;

            var compressor = new TimeSeriesCompressor<byte, int>(timeseriesCollection);
            var compressedBytes = compressor.ToCompressedByteArray();

            var expected = new byte[]{ 0x1f, 0x8b, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0b, 0x63, 0x64,
                0x60, 0x60, 0x60, 0x04, 0x42, 0x10, 0x10, 0x03, 0x62, 0x61, 0x30, 0x9f, 0x81, 0xc1, 0xe2, 0x3f,
                0x58, 0x08, 0x0d, 0x0c, 0x46, 0x41, 0x00, 0xb3, 0x3c, 0x5f, 0x36, 0xc2, 0x00, 0x00, 0x00 };
            
            Assert.Equal(expected, compressedBytes);
        }
    }
}