using PhilipDaubmeier.CompactTimeSeries;
using System;

namespace PhilipDaubmeier.GraphIoT.Core.Database
{
    public interface ITimeSeriesDbEntity
    {
        DateTime Key { get; set; }

        TimeSeries<T> GetSeries<T>(int index) where T : struct;

        void SetSeries<T>(int index, TimeSeries<T> series) where T : struct;
    }
}