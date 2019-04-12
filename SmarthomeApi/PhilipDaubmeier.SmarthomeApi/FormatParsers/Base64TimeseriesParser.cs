using PhilipDaubmeier.CompactTimeSeries;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmarthomeApi.FormatParsers
{
    public static class Base64TimeseriesParser
    {
        private const int defaultDecimalPlaces = 1;
        
        public static TimeSeries<T> ToTimeseries<T>(this string base64, TimeSeriesSpan span, int decimalPlaces = defaultDecimalPlaces) where T : struct
        {
            var timeseries = new TimeSeries<T>(span);
            if (string.IsNullOrWhiteSpace(base64))
                return timeseries;

            var bytes = Convert.FromBase64String(base64);
            if (typeof(T) == typeof(bool))
            {
                int i = 0;
                var boolValues = bytes.SelectMany(d => new byte[] { (byte)((d >> 6) & 0x3),
                    (byte)((d >> 4) & 0x3), (byte)((d >> 2) & 0x3), (byte)(d & 0x3) })
                    .Select(d => d > 0x01 ? null : (bool?)(d == 0x01)).Take(span.Count).ToList();
                foreach (var b in boolValues)
                {
                    (timeseries as TimeSeries<bool>)[i++] = b;
                }
                return timeseries;
            }
            
            for (int i = 0; i < bytes.Length - 1 && i < span.Count * 2; i += 2)
            {
                var value = (short)(bytes[i] << 8 | bytes[i + 1]);
                if (typeof(T) == typeof(int))
                    (timeseries as TimeSeries<int>)[i / 2] = value == short.MinValue ? null : (short?)value;
                else if (typeof(T) == typeof(double))
                    (timeseries as TimeSeries<double>)[i / 2] = value == short.MinValue ? null : (double?)value / Math.Pow(10d, decimalPlaces);
            }

            return timeseries;
        }

        public static string ToBase64(this TimeSeries<double> timeseries, int decimalPlaces = defaultDecimalPlaces)
        {
            return ToBase64(timeseries.Select(d => d.Value.HasValue ? 
                (int)(d.Value.Value * Math.Pow(10d, decimalPlaces)) : (int?)null));
        }

        public static string ToBase64(this TimeSeries<int> timeseries)
        {
            return ToBase64(timeseries.Select(d => d.Value));
        }

        public static string ToBase64(this TimeSeries<bool> timeseries)
        {
            var array = new byte[timeseries.Count / 4 + (timeseries.Count % 4 != 0 ? 1 : 0)];
            Func<bool?, int, byte> packNullableBool = (b, shift) => (byte)((!b.HasValue ? 0x03 : b.Value ? 0x01 : 0x00) << shift);
            Func<int, bool?> tryGetTsBool = tsIndex => tsIndex < timeseries.Count ? timeseries[tsIndex] : null;
            Func<int, int, byte> pack = (index, offset) => packNullableBool(tryGetTsBool(index * 4 + offset), 8 - ((offset + 1) * 2));
            for (int i = 0; i < array.Length; i++)
                array[i] = (byte)(pack(i, 0) | pack(i, 1) | pack(i, 2) | pack(i, 3));

            return Convert.ToBase64String(array);
        }

        private static string ToBase64(IEnumerable<int?> timeseries)
        {
            var array = timeseries.Select(d => d.HasValue ?
                Math.Min(short.MaxValue, Math.Max(short.MinValue - 1, (short)d.Value)) : short.MinValue)
                .SelectMany(d => new byte[] { (byte)(d >> 8), (byte)(d & 0xff) }).ToArray();

            return Convert.ToBase64String(array);
        }
    }
}
