using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace CompactTimeSeries.Tests
{
    public class TimeSeriesStreamCollectionTest
    {
        private const int count = 10;
        private static readonly DateTime begin = new DateTime(2019, 01, 09, 13, 23, 00, DateTimeKind.Utc);
        private static readonly DateTime end = begin.AddMinutes(count);

        private static List<Guid> guids = new List<Guid>() { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
        private static int guidSize = 16;
        private static Action<Guid, Stream> writeGuid = (g, s) =>
        {
            using (var writer = new BinaryWriter(s, Encoding.UTF8, true))
                writer.Write(g.ToByteArray());
        };

        [Fact]
        public void TestIntTimeSeriesStreamCollectionIndexers()
        {
            var timeseriesCollection = new TimeSeriesStreamCollection<Guid, int>(guids, guidSize, writeGuid, begin, end, count);

            List<ITimeSeries<int>> timeseries = new List<ITimeSeries<int>>();
            int i = 0;
            foreach (var item in timeseriesCollection)
            {
                timeseries.Add(item.Value);

                // make sure the order and contents of keys are the same as in the original key list
                Assert.Equal(item.Key, guids[i]);

                i++;
            }

            // make sure the same object is returned by the indexer as by the Enumerable
            Assert.Same(timeseries[0], timeseriesCollection[guids[0]]);
            Assert.Same(timeseries[1], timeseriesCollection[guids[1]]);
        }
    }
}