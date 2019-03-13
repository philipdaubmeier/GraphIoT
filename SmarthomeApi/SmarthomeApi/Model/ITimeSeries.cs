using System;
using System.Collections.Generic;

namespace SmarthomeApi.Model
{
    public interface ITimeSeries<T> : IEnumerable<KeyValuePair<DateTime, T?>> where T : struct
    {
    }
}
