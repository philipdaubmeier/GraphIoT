using PhilipDaubmeier.CompactTimeSeries;
using System;
using System.Collections.Generic;
using Xunit;

namespace PhilipDaubmeier.TimeseriesHostCommon.ViewModel.Tests
{
    public class GraphViewModelTest
    {
        [Fact]
        public void TestEmptyGraphViewModel()
        {
            var graphViewModel = new GraphViewModel();

            Assert.Equal(new DateTime(1970, 1, 1, 0, 0, 0), graphViewModel.Begin);
            Assert.Equal(TimeSpan.FromMilliseconds(0), graphViewModel.Spacing);
            Assert.Equal(string.Empty, graphViewModel.Name);
            Assert.Equal(string.Empty, graphViewModel.Format);
            Assert.Equal(new List<dynamic>(), graphViewModel.Points);

            Assert.Equal(0, graphViewModel.BeginUnixTimestamp);
            Assert.Equal(0, graphViewModel.SpacingMillis);
        }

        [Fact]
        public void TestMixedGraphViewModel()
        {
            var graphViewModel = new GraphViewModel() {
                Begin = new DateTime(2019, 01, 01, 12, 0, 0, DateTimeKind.Utc),
                Spacing = TimeSpan.FromMinutes(1),
                Name = "test",
                Format = "#.00 Unit",
                Points = new dynamic[] { 1, 2, 5d, 4f, "23" }
            };

            Assert.Equal(new DateTime(2019, 01, 01, 12, 0, 0, DateTimeKind.Utc), graphViewModel.Begin);
            Assert.Equal(TimeSpan.FromMinutes(1), graphViewModel.Spacing);
            Assert.Equal("test", graphViewModel.Name);
            Assert.Equal("#.00 Unit", graphViewModel.Format);
            Assert.Equal(new dynamic[] { 1, 2, 5d, 4f, "23" }, graphViewModel.Points);

            Assert.Equal(1546344000000, graphViewModel.BeginUnixTimestamp);
            Assert.Equal(60000, graphViewModel.SpacingMillis);

            Assert.Equal(new dynamic[] {
                new dynamic[] {1, 1546344000000 },
                new dynamic[] {2, 1546344060000 },
                new dynamic[] {5d, 1546344120000 },
                new dynamic[] {4f, 1546344180000 },
                new dynamic[] {"23", 1546344240000 }
            }, graphViewModel.TimestampedPoints());
        }

        [Fact]
        public void TestEmptyGenericGraphViewModel()
        {
            var span = new TimeSeriesSpan(new DateTime(2019, 01, 01, 12, 0, 0, DateTimeKind.Utc), TimeSeriesSpan.Spacing.Spacing1Min, 5);
            var timeseries = new TimeSeries<int>(span);
            var graphViewModel = new GraphViewModel<int>(timeseries, "test", "#.00 Unit");

            Assert.Equal(new DateTime(1970, 1, 1, 0, 0, 0), graphViewModel.Begin);
            Assert.Equal(TimeSpan.FromMinutes(1), graphViewModel.Spacing);
            Assert.Equal("test", graphViewModel.Name);
            Assert.Equal("#.00 Unit", graphViewModel.Format);
            Assert.Equal(new dynamic[] { }, graphViewModel.Points);
            Assert.Equal(new dynamic[] { }, graphViewModel.TimestampedPoints());

            Assert.Equal(0, graphViewModel.BeginUnixTimestamp);
            Assert.Equal(60000, graphViewModel.SpacingMillis);
        }

        [Fact]
        public void TestGenericGraphViewModel()
        {
            var span = new TimeSeriesSpan(new DateTime(2019, 01, 01, 12, 0, 0, DateTimeKind.Utc), TimeSeriesSpan.Spacing.Spacing1Min, 5);
            var timeseries = new TimeSeries<int>(span);
            timeseries[2] = 23;
            timeseries[4] = 42;

            var graphViewModel = new GraphViewModel<int>(timeseries, "test", "#.00 Unit");

            Assert.Equal(new DateTime(2019, 01, 01, 12, 2, 0, DateTimeKind.Utc), graphViewModel.Begin);
            Assert.Equal(TimeSpan.FromMinutes(1), graphViewModel.Spacing);
            Assert.Equal("test", graphViewModel.Name);
            Assert.Equal("#.00 Unit", graphViewModel.Format);
            Assert.Equal(new dynamic[] { 23, null, 42 }, graphViewModel.Points);

            Assert.Equal(1546344120000, graphViewModel.BeginUnixTimestamp);
            Assert.Equal(60000, graphViewModel.SpacingMillis);

            Assert.Equal(new dynamic[] {
                new dynamic[] {23, 1546344120000 },
                new dynamic[] {null, 1546344180000 },
                new dynamic[] {42, 1546344240000 }
            }, graphViewModel.TimestampedPoints());
        }
    }
}