using System;
using System.Collections.Generic;

namespace CompactTimeSeries
{
    public interface ITimeSeries<T> : IEnumerable<KeyValuePair<DateTime, T?>> where T : struct
    {
        List<T> ToList(T defaultValue);

        void Accumulate(DateTime time, T item);

        T? this[DateTime time] { set; get; }

        T? this[int index] { get; set; }

        int Count { get; }
    }
}