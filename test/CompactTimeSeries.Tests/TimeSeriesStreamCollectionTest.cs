using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using Xunit;

namespace PhilipDaubmeier.CompactTimeSeries.Tests
{
    public class TimeSeriesStreamCollectionTest
    {
        private const int count = 10;
        private static readonly DateTime begin = new DateTime(2019, 01, 09, 13, 23, 00, DateTimeKind.Utc);
        private static readonly DateTime end = begin.AddMinutes(count);
        private static readonly TimeSeriesSpan span = new TimeSeriesSpan(begin, end, count);

        private static List<Guid> guids = new List<Guid>() { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
        private static int guidSize = 16;
        private static Action<Guid, Stream> writeGuid = (g, s) =>
        {
            using (var writer = new BinaryWriter(s, Encoding.UTF8, true))
                writer.Write(g.ToByteArray());
        };
        private static Func<Stream, Guid> readGuid = s =>
        {
            using (var reader = new BinaryReader(s, Encoding.UTF8, true))
                return new Guid(reader.ReadBytes(guidSize));
        };

        [Fact]
        public void TestIntTimeSeriesStreamCollectionIndexers()
        {
            var timeseriesCollection = new TimeSeriesStreamCollection<Guid, int>(guids, guidSize, writeGuid, span);

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

        [Fact]
        public void TestIntTimeSeriesStreamCollectionCreateModifyWriteReadModify()
        {
            List<int?> addValues1 = new List<int?>() { 1, 3, 5, 3, 1, null, null, null, null, null };
            List<int?> addValues2 = new List<int?>() { null, null, null, null, null, 21, 23, 25, 23, 21 };
            var allValues = addValues1.Zip(addValues2, (x, y) => x ?? y).ToList();

            // Create
            var timeseriesCollectionOrig = new TimeSeriesStreamCollection<Guid, int>(guids, guidSize, writeGuid, span);

            // Modify
            foreach (var item in timeseriesCollectionOrig)
                for (int i = 0; i< Math.Min(addValues1.Count, item.Value.Count); i++)
                    if (addValues1[i].HasValue)
                        item.Value[i] = addValues1[i];

            // Write
            var compressed = timeseriesCollectionOrig.ToCompressedByteArray();

            // Read
            var timeseriesCollectionRead = new TimeSeriesStreamCollection<Guid, int>(compressed, guidSize, readGuid, span);

            // Modify
            foreach (var item in timeseriesCollectionRead)
                for (int i = 0; i < Math.Min(addValues2.Count, item.Value.Count); i++)
                    if (addValues2[i].HasValue)
                        item.Value[i] = addValues2[i];

            // Assert
            foreach (var item in timeseriesCollectionRead)
                Assert.Equal(allValues, item.Value.Select(x => x.Value).Take(allValues.Count).ToList());
        }
    }
}