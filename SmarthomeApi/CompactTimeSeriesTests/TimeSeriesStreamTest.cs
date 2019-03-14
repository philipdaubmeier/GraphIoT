using System;
using System.Collections.Generic;
using Xunit;

namespace CompactTimeSeries.Tests
{
    public class TimeSeriesStreamTest
    {
        private const int count = 10;
        private static readonly DateTime begin = new DateTime(2019, 01, 09, 13, 23, 00, DateTimeKind.Utc);
        private static readonly DateTime end = begin.AddMinutes(count);

        private static readonly DateTime outsideRange = new DateTime(2010, 08, 12, 07, 44, 00, DateTimeKind.Utc);
        private static readonly DateTime insideRangeExactSecond = new DateTime(2019, 01, 09, 13, 28, 00, DateTimeKind.Utc);
        private static readonly DateTime insideRangeOddSecond1 = new DateTime(2019, 01, 09, 13, 26, 01, DateTimeKind.Utc);
        private static readonly DateTime insideRangeOddSecond2 = new DateTime(2019, 01, 09, 13, 30, 59, DateTimeKind.Utc);

        [Fact]
        public void TestIntTimeSeriesStreamIndexers()
        {
            var TimeSeriesStream = new TimeSeriesStream<int>(begin, end, count);

            // test initial values, all should be null whether or not inside or outside the boundaries
            Assert.Null(TimeSeriesStream[outsideRange]);
            Assert.Null(TimeSeriesStream[insideRangeExactSecond]);
            Assert.Null(TimeSeriesStream[insideRangeOddSecond1]);
            Assert.Null(TimeSeriesStream[insideRangeOddSecond2]);
            Assert.Null(TimeSeriesStream[begin]);
            Assert.Null(TimeSeriesStream[end]);

            // write with DateTime indexer outside boundaries - should silently be ignored
            TimeSeriesStream[outsideRange] = 11;
            Assert.Null(TimeSeriesStream[outsideRange]);

            // write with DateTime indexer inside boundaries and check if the same values can be read again
            TimeSeriesStream[insideRangeExactSecond] = 23;
            Assert.Equal(23, TimeSeriesStream[insideRangeExactSecond]);
            TimeSeriesStream[insideRangeOddSecond1] = 42;
            Assert.Equal(42, TimeSeriesStream[insideRangeOddSecond1]);
            TimeSeriesStream[insideRangeOddSecond2] = 43;
            Assert.Equal(43, TimeSeriesStream[insideRangeOddSecond2]);
            TimeSeriesStream[begin] = 1;
            Assert.Equal(1, TimeSeriesStream[begin]);
            TimeSeriesStream[end] = 99;
            Assert.Equal(99, TimeSeriesStream[end]);

            // read with int indexer: were the values put in the correct buckets?
            Assert.Equal(1, TimeSeriesStream[0]);
            Assert.Null(TimeSeriesStream[1]);
            Assert.Null(TimeSeriesStream[2]);
            Assert.Equal(42, TimeSeriesStream[3]);
            Assert.Null(TimeSeriesStream[4]);
            Assert.Equal(23, TimeSeriesStream[5]);
            Assert.Null(TimeSeriesStream[6]);
            Assert.Equal(43, TimeSeriesStream[7]);
            Assert.Null(TimeSeriesStream[8]);
            Assert.Equal(99, TimeSeriesStream[9]);

            // test outside of index boundaries, should throw out of range exception
            Assert.Throws<ArgumentOutOfRangeException>(() => TimeSeriesStream[-1]);
            Assert.Throws<ArgumentOutOfRangeException>(() => TimeSeriesStream[10]);

            // finally write via the int indexer and check via int as well as DateTime indexer
            TimeSeriesStream[7] = 56;
            Assert.Equal(56, TimeSeriesStream[7]);
            Assert.Equal(56, TimeSeriesStream[insideRangeOddSecond2]);
            TimeSeriesStream[0] = 39;
            Assert.Equal(39, TimeSeriesStream[0]);
            Assert.Equal(39, TimeSeriesStream[begin]);
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
            var TimeSeriesStream = new TimeSeriesStream<int>(begin, end, count);

            TimeSeriesStream[0] = value;
            if (value == short.MinValue)
                Assert.Null(TimeSeriesStream[0]);
            else if (value < short.MinValue)
                Assert.Equal(short.MinValue, TimeSeriesStream[0]);
            else if (value > short.MaxValue)
                Assert.Equal(short.MaxValue, TimeSeriesStream[0]);
            else
                Assert.Equal(value, TimeSeriesStream[0]);
        }

        [Theory]
        [InlineData(null)]
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
        public void TestDoubleTimeSeriesStreamValueRange(double? value)
        {
            var TimeSeriesStream = new TimeSeriesStream<double>(begin, end, count, 0);

            TimeSeriesStream[0] = value;
            if (value < short.MinValue)
                Assert.Equal(short.MinValue, TimeSeriesStream[0]);
            else if (value > short.MaxValue)
                Assert.Equal(short.MaxValue, TimeSeriesStream[0]);
            else
                Assert.Equal(value, TimeSeriesStream[0]);
        }

        [Fact]
        public void TestIntTimeSeriesStreamToList()
        {
            var TimeSeriesStream = new TimeSeriesStream<int>(begin, end, count);

            TimeSeriesStream[insideRangeExactSecond] = 23;
            TimeSeriesStream[insideRangeOddSecond1] = 42;
            TimeSeriesStream[insideRangeOddSecond2] = 43;
            TimeSeriesStream[begin] = 1;
            TimeSeriesStream[end] = 99;
            
            var expected = new List<int>() { 1, 77, 77, 42, 77, 23, 77, 43, 77, 99 };
            Assert.Equal(expected, TimeSeriesStream.ToList(77));
        }

        [Fact]
        public void TestIntTimeSeriesStreamCroppedFrontToList()
        {
            var TimeSeriesStream = new TimeSeriesStream<int>(begin, end, count);

            TimeSeriesStream[insideRangeExactSecond] = 23;
            TimeSeriesStream[insideRangeOddSecond1] = 42;
            TimeSeriesStream[insideRangeOddSecond2] = 43;
            TimeSeriesStream[end] = 99;

            var expected = new List<int>() { 42, 77, 23, 77, 43, 77, 99 };
            Assert.Equal(expected, TimeSeriesStream.ToList(77));
        }

        [Fact]
        public void TestIntTimeSeriesStreamCroppedBackToList()
        {
            var TimeSeriesStream = new TimeSeriesStream<int>(begin, end, count);

            TimeSeriesStream[insideRangeExactSecond] = 23;
            TimeSeriesStream[insideRangeOddSecond1] = 42;
            TimeSeriesStream[insideRangeOddSecond2] = 43;
            TimeSeriesStream[begin] = 1;

            var expected = new List<int>() { 1, 77, 77, 42, 77, 23, 77, 43 };
            Assert.Equal(expected, TimeSeriesStream.ToList(77));
        }

        [Fact]
        public void TestIntTimeSeriesStreamCroppedBothToList()
        {
            var TimeSeriesStream = new TimeSeriesStream<int>(begin, end, count);

            TimeSeriesStream[insideRangeExactSecond] = 23;
            TimeSeriesStream[insideRangeOddSecond1] = 42;
            TimeSeriesStream[insideRangeOddSecond2] = 43;

            var expected = new List<int>() { 42, 77, 23, 77, 43 };
            Assert.Equal(expected, TimeSeriesStream.ToList(77));
        }

        [Fact]
        public void TestIntTimeSeriesStreamAccumulate()
        {
            var TimeSeriesStream = new TimeSeriesStream<int>(begin, end, count);

            TimeSeriesStream.Accumulate(insideRangeExactSecond, 1);
            Assert.Equal(1, TimeSeriesStream[insideRangeExactSecond]);
            TimeSeriesStream.Accumulate(insideRangeExactSecond, 22);
            Assert.Equal(23, TimeSeriesStream[insideRangeExactSecond]);
            TimeSeriesStream.Accumulate(insideRangeExactSecond, 100);
            Assert.Equal(123, TimeSeriesStream[insideRangeExactSecond]);
            TimeSeriesStream.Accumulate(insideRangeExactSecond, -123);
            Assert.Equal(0, TimeSeriesStream[insideRangeExactSecond]);

            // shoud silently be cropped to short.MaxValue (32767)
            TimeSeriesStream.Accumulate(insideRangeExactSecond, int.MaxValue);
            Assert.Equal(short.MaxValue, TimeSeriesStream[insideRangeExactSecond]);

            // ...and be cropped again
            TimeSeriesStream.Accumulate(insideRangeExactSecond, int.MaxValue);
            Assert.Equal(short.MaxValue, TimeSeriesStream[insideRangeExactSecond]);
        }

        [Fact]
        public void TestIntTimeSeriesStreamProperties()
        {
            var TimeSeriesStream = new TimeSeriesStream<int>(begin, end, count);
            
            Assert.Equal(begin, TimeSeriesStream.Begin);
            Assert.Equal(end, TimeSeriesStream.End);
            Assert.Equal(count, TimeSeriesStream.Count);
        }

        [Fact]
        public void TestIntTimeSeriesStreamEnumerable()
        {
            var TimeSeriesStream = new TimeSeriesStream<int>(begin, end, count);

            TimeSeriesStream[insideRangeExactSecond] = 23;
            TimeSeriesStream[insideRangeOddSecond1] = 42;
            TimeSeriesStream[insideRangeOddSecond2] = 43;

            int i = 0;
            foreach (var item in TimeSeriesStream)
            {
                Assert.Equal(begin.AddMinutes(i), item.Key);
                Assert.Equal(TimeSeriesStream[i], item.Value);
                i++;
            }
        }
    }
}