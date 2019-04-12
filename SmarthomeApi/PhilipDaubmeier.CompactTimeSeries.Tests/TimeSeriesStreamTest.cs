using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PhilipDaubmeier.CompactTimeSeries.Tests
{
    public class TimeSeriesStreamTest
    {
        private const int count = 10;
        private static readonly DateTime begin = new DateTime(2019, 01, 09, 13, 23, 00, DateTimeKind.Utc);
        private static readonly DateTime end = begin.AddMinutes(count);
        private static readonly TimeSeriesSpan span = new TimeSeriesSpan(begin, end, count);

        private static readonly DateTime outsideRange = new DateTime(2010, 08, 12, 07, 44, 00, DateTimeKind.Utc);
        private static readonly DateTime insideRangeExactSecond = new DateTime(2019, 01, 09, 13, 28, 00, DateTimeKind.Utc);
        private static readonly DateTime insideRangeOddSecond1 = new DateTime(2019, 01, 09, 13, 26, 01, DateTimeKind.Utc);
        private static readonly DateTime insideRangeOddSecond2 = new DateTime(2019, 01, 09, 13, 30, 59, DateTimeKind.Utc);

        [Fact]
        public void TestIntTimeSeriesStreamIndexers()
        {
            var timeseries = new TimeSeriesStream<int>(span);

            // test initial values, all should be null whether or not inside or outside the boundaries
            Assert.Null(timeseries[outsideRange]);
            Assert.Null(timeseries[insideRangeExactSecond]);
            Assert.Null(timeseries[insideRangeOddSecond1]);
            Assert.Null(timeseries[insideRangeOddSecond2]);
            Assert.Null(timeseries[begin]);
            Assert.Null(timeseries[end]);

            // write with DateTime indexer outside boundaries - should silently be ignored
            timeseries[outsideRange] = 11;
            Assert.Null(timeseries[outsideRange]);

            // write with DateTime indexer inside boundaries and check if the same values can be read again
            timeseries[insideRangeExactSecond] = 23;
            Assert.Equal(23, timeseries[insideRangeExactSecond]);
            timeseries[insideRangeOddSecond1] = 42;
            Assert.Equal(42, timeseries[insideRangeOddSecond1]);
            timeseries[insideRangeOddSecond2] = 43;
            Assert.Equal(43, timeseries[insideRangeOddSecond2]);
            timeseries[begin] = 1;
            Assert.Equal(1, timeseries[begin]);
            timeseries[end] = 99;
            Assert.Equal(99, timeseries[end]);

            // read with int indexer: were the values put in the correct buckets?
            Assert.Equal(1, timeseries[0]);
            Assert.Null(timeseries[1]);
            Assert.Null(timeseries[2]);
            Assert.Equal(42, timeseries[3]);
            Assert.Null(timeseries[4]);
            Assert.Equal(23, timeseries[5]);
            Assert.Null(timeseries[6]);
            Assert.Equal(43, timeseries[7]);
            Assert.Null(timeseries[8]);
            Assert.Equal(99, timeseries[9]);

            // test writing outside of index boundaries, should throw out of range exception
            Assert.Throws<ArgumentOutOfRangeException>(() => timeseries[-1] = 22);
            Assert.Throws<ArgumentOutOfRangeException>(() => timeseries[10] = 22);

            // test reading outside of index boundaries, should throw out of range exception
            Assert.Throws<ArgumentOutOfRangeException>(() => timeseries[-1]);
            Assert.Throws<ArgumentOutOfRangeException>(() => timeseries[10]);

            // finally write via the int indexer and check via int as well as DateTime indexer
            timeseries[7] = 56;
            Assert.Equal(56, timeseries[7]);
            Assert.Equal(56, timeseries[insideRangeOddSecond2]);
            timeseries[0] = 39;
            Assert.Equal(39, timeseries[0]);
            Assert.Equal(39, timeseries[begin]);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(0)]
        [InlineData(byte.MaxValue)]
        [InlineData(short.MinValue)]
        [InlineData(short.MaxValue)]
        [InlineData(ushort.MinValue)]
        [InlineData(ushort.MaxValue)]
        [InlineData(int.MinValue)]
        [InlineData(int.MaxValue)]
        public void TestIntTimeSeriesStreamValueRange(int? value)
        {
            var timeseries = new TimeSeriesStream<int>(span);

            timeseries[0] = value;
            if (value <= short.MinValue)
                Assert.Equal(short.MinValue + 1, timeseries[0]);
            else if (value > short.MaxValue)
                Assert.Equal(short.MaxValue, timeseries[0]);
            else
                Assert.Equal(value, timeseries[0]);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(0d)]
        [InlineData(42d)]
        [InlineData(4.253)]
        [InlineData(0.4253)]
        [InlineData(0.04253)]
        [InlineData(0.004253)]
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
        public void TestDoubleTimeSeriesStreamValueRange(double? value)
        {
            foreach (var decimalPlaces in Enumerable.Range(0, 3))
            {
                var timeseries = new TimeSeriesStream<double>(span, decimalPlaces);

                timeseries[0] = value;

                var factor = Math.Pow(10, decimalPlaces);
                var internalValue = !value.HasValue ? null : value * factor;

                if (double.IsNaN(value ?? 0d))
                    Assert.Null(timeseries[0]);
                else if (internalValue <= short.MinValue)
                    Assert.Equal((short.MinValue + 1) / factor, timeseries[0]);
                else if (internalValue > short.MaxValue)
                    Assert.Equal(short.MaxValue / factor, timeseries[0]);
                else
                {
                    var expected = !value.HasValue ? (double?)null : Math.Floor(value.Value * factor) / factor;
                    Assert.Equal(expected, timeseries[0]);
                }
            }
        }

        [Fact]
        public void TestIntTimeSeriesStreamToList()
        {
            var timeseries = new TimeSeriesStream<int>(span);

            timeseries[insideRangeExactSecond] = 23;
            timeseries[insideRangeOddSecond1] = 42;
            timeseries[insideRangeOddSecond2] = 43;
            timeseries[begin] = 1;
            timeseries[end] = 99;
            
            var expected = new List<int>() { 1, 77, 77, 42, 77, 23, 77, 43, 77, 99 };
            Assert.Equal(expected, timeseries.Trimmed(77));
        }

        [Fact]
        public void TestIntTimeSeriesStreamCroppedFrontToList()
        {
            var timeseries = new TimeSeriesStream<int>(span);

            timeseries[insideRangeExactSecond] = 23;
            timeseries[insideRangeOddSecond1] = 42;
            timeseries[insideRangeOddSecond2] = 43;
            timeseries[end] = 99;

            var expected = new List<int>() { 42, 77, 23, 77, 43, 77, 99 };
            Assert.Equal(expected, timeseries.Trimmed(77));
        }

        [Fact]
        public void TestIntTimeSeriesStreamCroppedBackToList()
        {
            var timeseries = new TimeSeriesStream<int>(span);

            timeseries[insideRangeExactSecond] = 23;
            timeseries[insideRangeOddSecond1] = 42;
            timeseries[insideRangeOddSecond2] = 43;
            timeseries[begin] = 1;

            var expected = new List<int>() { 1, 77, 77, 42, 77, 23, 77, 43 };
            Assert.Equal(expected, timeseries.Trimmed(77));
        }

        [Fact]
        public void TestIntTimeSeriesStreamCroppedBothToList()
        {
            var timeseries = new TimeSeriesStream<int>(span);

            timeseries[insideRangeExactSecond] = 23;
            timeseries[insideRangeOddSecond1] = 42;
            timeseries[insideRangeOddSecond2] = 43;

            var expected = new List<int>() { 42, 77, 23, 77, 43 };
            Assert.Equal(expected, timeseries.Trimmed(77));
        }

        [Fact]
        public void TestIntTimeSeriesStreamAccumulate()
        {
            var timeseries = new TimeSeriesStream<int>(span);

            timeseries.Accumulate(insideRangeExactSecond, 1);
            Assert.Equal(1, timeseries[insideRangeExactSecond]);
            timeseries.Accumulate(insideRangeExactSecond, 22);
            Assert.Equal(23, timeseries[insideRangeExactSecond]);
            timeseries.Accumulate(insideRangeExactSecond, 100);
            Assert.Equal(123, timeseries[insideRangeExactSecond]);
            timeseries.Accumulate(insideRangeExactSecond, -123);
            Assert.Equal(0, timeseries[insideRangeExactSecond]);

            // shoud silently be cropped to short.MaxValue (32767)
            timeseries.Accumulate(insideRangeExactSecond, int.MaxValue);
            Assert.Equal(short.MaxValue, timeseries[insideRangeExactSecond]);

            // ...and be cropped again
            timeseries.Accumulate(insideRangeExactSecond, short.MaxValue * 10);
            Assert.Equal(short.MaxValue, timeseries[insideRangeExactSecond]);
        }

        [Fact]
        public void TestIntTimeSeriesStreamProperties()
        {
            var timeseries = new TimeSeriesStream<int>(span);
            
            Assert.Equal(begin, timeseries.Begin);
            Assert.Equal(end, timeseries.End);
            Assert.Equal(count, timeseries.Count);
        }

        [Fact]
        public void TestIntTimeSeriesStreamEnumerable()
        {
            var timeseries = new TimeSeriesStream<int>(span);

            timeseries[insideRangeExactSecond] = 23;
            timeseries[insideRangeOddSecond1] = 42;
            timeseries[insideRangeOddSecond2] = 43;

            int i = 0;
            foreach (var item in timeseries)
            {
                Assert.Equal(begin.AddMinutes(i), item.Key);
                Assert.Equal(timeseries[i], item.Value);
                i++;
            }
        }
    }
}