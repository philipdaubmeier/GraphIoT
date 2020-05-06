using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.GraphIoT.Core.Parsers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace PhilipDaubmeier.GraphIoT.Core.Database
{
    public abstract class TimeSeriesDbEntityBase : ITimeSeriesDbEntity
    {
        public abstract DateTime Key { get; set; }

        public abstract TimeSeriesSpan Span { get; }

        protected abstract int DecimalPlaces { get; }

        protected TimeSeriesSpan SpanMonth30Min => new TimeSeriesSpan(Key, Key.AddMonths(1), (int)Math.Floor((Key.AddMonths(1) - Key).TotalDays * 24 * 2));
        protected TimeSeriesSpan SpanMonth160Min => new TimeSeriesSpan(Key, Key.AddMonths(1), (int)Math.Floor((Key.AddMonths(1) - Key).TotalDays * 9));

        protected TimeSeriesSpan SpanDay1Sec => new TimeSeriesSpan(Key, TimeSeriesSpan.Spacing.Spacing1Sec, (int)TimeSeriesSpan.Spacing.Spacing1Day);
        protected TimeSeriesSpan SpanDay1Min => new TimeSeriesSpan(Key, Key.AddDays(1), TimeSeriesSpan.Spacing.Spacing1Min);
        protected TimeSeriesSpan SpanDay5Min => new TimeSeriesSpan(Key, Key.AddDays(1), TimeSeriesSpan.Spacing.Spacing5Min);

        public void SetSeriesValue<T>(int index, DateTime time, T value) where T : struct
        {
            var series = GetSeries<T>(index);
            series[time] = value;
            SetSeries(index, series);
        }

        public void CopyInto<T>(int index, TimeSeries<T> value) where T : struct
        {
            var series = GetSeries<T>(index);
            for (int i = 0; i < value.Span.Count; i++)
            {
                var time = value.Span.Begin + (i * value.Span.Duration);
                series[time] = value[time];
            }

            SetSeries(index, series);
        }

        public TimeSeries<T> GetSeries<T>(int index) where T : struct
        {
            var method = CurveProperty(index)?.GetGetMethod();
            if (method is null)
                throw new Exception($"There is no readable curve property at index {index}");

            var attribute = CurveProperty(index).GetCustomAttribute<TimeSeriesAttribute>();
            if (attribute is null)
                throw new Exception($"Curve property not annotated with TimeSeries attribute at index {index}");

            if (typeof(T) != attribute.Type)
                throw new Exception($"Can not read property at index {index} as time series of type {typeof(T)}, it is defined to be {attribute.Type}");

            return (method.Invoke(this, null) as string).ToTimeseries<T>(Span, DecimalPlaces);
        }

        public void SetSeries<T>(int index, TimeSeries<T> series) where T : struct
        {
            var property = CurveProperty(index);
            if (property is null)
                throw new Exception($"There is no curve property at index {index}");

            var attribute = property.GetCustomAttribute<TimeSeriesAttribute>();
            if (attribute is null)
                throw new Exception($"Curve property not annotated with TimeSeries attribute at index {index}");

            if (typeof(T) != attribute.Type)
                throw new Exception($"Can not set property at index {index} with time series of type {typeof(T)}, it is defined to be {attribute.Type}");

            property.GetSetMethod()?.Invoke(this, new object[] { series.ToBase64(DecimalPlaces) });
        }

        public void ResampleFrom<T>(TimeSeriesDbEntityBase other, int index, Func<IEnumerable<T>, T> aggregate) where T : struct
        {
            var series = GetSeries<T>(index);
            var resampler = new TimeSeriesResampler<TimeSeries<T>, T>(series.Span) { Resampled = series };
            resampler.SampleAggregate(other.GetSeries<T>(index), aggregate);
            SetSeries(index, resampler.Resampled);
        }

        private static readonly Semaphore _loadPropertiesSemaphore = new Semaphore(1, 1);
        private static readonly Dictionary<Type, List<PropertyInfo>> _curveProperties = new Dictionary<Type, List<PropertyInfo>>();
        private PropertyInfo? CurveProperty(int index)
        {
            try
            {
                _loadPropertiesSemaphore.WaitOne();

                if (!_curveProperties.TryGetValue(GetType(), out var props))
                {
                    _curveProperties.Add(GetType(), props = GetType().GetProperties()
                        .Where(prop => prop.Name.EndsWith("Curve", StringComparison.InvariantCultureIgnoreCase))
                        .Where(prop => Attribute.IsDefined(prop, typeof(MaxLengthAttribute)))
                        .Where(prop => prop.PropertyType == typeof(string))
                        .Where(prop => prop.CanRead && prop.CanWrite)
                        .ToList());
                }

                if (props == null || index < 0 || index >= props.Count)
                    return null;

                return props[index];
            }
            finally { _loadPropertiesSemaphore.Release(); }
        }
    }
}