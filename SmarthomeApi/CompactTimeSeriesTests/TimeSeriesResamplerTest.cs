using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace CompactTimeSeries.Tests
{
    public class TimeSeriesResamplerTest
    {
        private const int count = 10;
        private static readonly DateTime begin = new DateTime(2019, 01, 09, 13, 23, 00, DateTimeKind.Utc);
        private static readonly DateTime end = begin.AddMinutes(count);
        private static readonly TimeSeriesSpan span = new TimeSeriesSpan(begin, end, count);
        private static readonly TimeSeriesSpan spanDownsampling = new TimeSeriesSpan(begin, end, count / 2);
        private static readonly TimeSeriesSpan spanUpsampling = new TimeSeriesSpan(begin, end, count * 2);

        [Fact]
        public void TestConstructTimeSeriesResampler()
        {
            var resampler1 = new TimeSeriesResampler<TimeSeriesStream<int>, int>(spanDownsampling);
            Assert.NotNull(resampler1);
            var resampler2 = new TimeSeriesResampler<TimeSeriesStream<double>, double>(spanDownsampling);
            Assert.NotNull(resampler2);

            var resampler3 = new TimeSeriesResampler<TimeSeries<int>, int>(spanDownsampling);
            Assert.NotNull(resampler3);
            var resampler4 = new TimeSeriesResampler<TimeSeries<double>, double>(spanDownsampling);
            Assert.NotNull(resampler4);
            var resampler5 = new TimeSeriesResampler<TimeSeries<bool>, bool>(spanDownsampling);
            Assert.NotNull(resampler5);
        }

        [Fact]
        public void TestIntTimeSeriesDownsampling()
        {
            var timeseries = new TimeSeriesStream<int>(span);
            
            timeseries[0] = 1;
            timeseries[3] = 23;
            timeseries[7] = 43;
            timeseries[8] = 5;
            timeseries[9] = 99;

            var resampler = new TimeSeriesResampler<TimeSeriesStream<int>, int>(spanDownsampling);
            resampler.SampleAccumulate(timeseries);

            Assert.Equal(new List<int>(){ 1, 23, -1, 43, 104 }, resampler.Resampled.ToList(-1));
        }

        [Fact]
        public void TestIntTimeSeriesUpDownsampling()
        {
            var timeseries = new TimeSeriesStream<int>(span);

            timeseries[0] = 1;
            timeseries[3] = 23;
            timeseries[7] = 43;
            timeseries[8] = 5;
            timeseries[9] = 99;

            // upsample
            var resampler = new TimeSeriesResampler<TimeSeriesStream<int>, int>(spanUpsampling);
            resampler.SampleAccumulate(timeseries);

            // downsample to the original
            var resampler2 = new TimeSeriesResampler<TimeSeriesStream<int>, int>(span);
            resampler2.SampleAccumulate(resampler.Resampled);

            Assert.Equal(timeseries.ToList(-1), resampler2.Resampled.ToList(-1));
        }

        [Fact]
        public void TestIntTimeSeriesSampleAverage()
        {
            var timeseries = new TimeSeriesStream<int>(span);

            timeseries[0] = 0;
            timeseries[1] = 10;
            timeseries[2] = 0;
            timeseries[3] = 50;
            timeseries[4] = 10;
            timeseries[5] = 20;
            timeseries[6] = 50;
            timeseries[7] = 60;
            timeseries[8] = 5;
            timeseries[9] = 10;

            var resampler = new TimeSeriesResampler<TimeSeriesStream<int>, int>(spanDownsampling);
            resampler.SampleAverage(timeseries, x => x, x => (int)x);

            Assert.Equal(new List<int>() { 5, 25, 15, 55, 7 }, resampler.Resampled.ToList(-1));
        }

        [Fact]
        public void TestDoubleTimeSeriesSampleAverage()
        {
            var timeseries = new TimeSeriesStream<double>(span);

            timeseries[0] = 0d;
            timeseries[1] = 10d;
            timeseries[2] = 0.0;
            timeseries[3] = 0.5;
            timeseries[4] = 0.01;
            timeseries[5] = 0.02;
            timeseries[6] = 50.7;
            timeseries[7] = 60.7;
            timeseries[8] = 50000;
            timeseries[9] = 100000;

            var resampler = new TimeSeriesResampler<TimeSeriesStream<double>, double>(spanDownsampling);
            resampler.SampleAverage(timeseries, x => (decimal)x, x => (double)x);

            // results get cropped to 1 decimal place and to a max of short.MaxValue / 10
            Assert.Equal(new List<double>() { 5d, 0.2, 0.0, 55.7, 3276.7 }, resampler.Resampled.ToList(-1d));
        }

        [Fact]
        public void TestIntTimeSeriesOddResampling()
        {
            var timeseries = new TimeSeriesStream<int>(new TimeSeriesSpan(begin, end, 5));

            timeseries[0] = 7;
            timeseries[1] = 10;
            timeseries[2] = 23;
            timeseries[3] = 50;
            timeseries[4] = 12;

            // Resample with slightly later start and slightly earlier end
            var resampler1 = new TimeSeriesResampler<TimeSeriesStream<int>, int>(
                new TimeSeriesSpan(begin.AddSeconds(5), end.AddSeconds(-5), 5));
            resampler1.SampleAverage(timeseries, x => x, x => (int)x);

            Assert.Equal(new List<int>() { 10, 23, -1, 50, 12 }, resampler1.Resampled.ToList(-1));

            // Resample with slightly earlier start and slightly later end
            var resampler2 = new TimeSeriesResampler<TimeSeriesStream<int>, int>(
                new TimeSeriesSpan(begin.AddSeconds(-5), end.AddSeconds(5), 5));
            resampler2.SampleAverage(timeseries, x => x, x => (int)x);

            Assert.Equal(new List<int>() { 7, 10, 36, 12 }, resampler2.Resampled.ToList(-1));
        }

        [Fact]
        public void TestBoolTimeSeriesResampling()
        {
            var timeseries = new TimeSeries<bool>(span);

            timeseries[0] = true;
            timeseries[1] = true;
            timeseries[2] = null;
            timeseries[3] = null;
            timeseries[4] = true;
            timeseries[5] = false;
            timeseries[6] = null;
            timeseries[7] = true;
            timeseries[8] = null;
            timeseries[9] = false;

            var resampler = new TimeSeriesResampler<TimeSeries<bool>, bool>(spanDownsampling);
            resampler.SampleAggregate(timeseries, x => x.Any(b => b));

            Assert.Equal(new List<bool>() { true, false, true, true, false }, resampler.Resampled.ToList(false));
        }
    }
}