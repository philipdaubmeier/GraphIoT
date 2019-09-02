using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.TimeseriesHostCommon.Parsers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace PhilipDaubmeier.TimeseriesHostCommon.Database
{
    public abstract class TimeSeriesDbEntityBase : ITimeSeriesDbEntity
    {
        public abstract DateTime Key { get; set; }

        protected abstract TimeSeriesSpan Span { get; }

        protected abstract int DecimalPlaces { get; }

        public TimeSeries<T> GetSeries<T>(int index) where T : struct
        {
            return (CurveProperty(index)?.GetGetMethod()?.Invoke(this, null) as string)?.ToTimeseries<T>(Span, DecimalPlaces);
        }

        public void SetSeries<T>(int index, TimeSeries<T> series) where T : struct
        {
            CurveProperty(index)?.GetSetMethod()?.Invoke(this, new object[] { series.ToBase64(DecimalPlaces) });
        }

        private static readonly Dictionary<Type, List<PropertyInfo>> _curveProperties = new Dictionary<Type, List<PropertyInfo>>();
        private PropertyInfo CurveProperty(int index)
        {
            if (!_curveProperties.ContainsKey(GetType()))
            {
                _curveProperties.Add(GetType(), GetType().GetProperties()
                    .Where(prop => prop.Name.EndsWith("Curve", StringComparison.InvariantCultureIgnoreCase))
                    .Where(prop => Attribute.IsDefined(prop, typeof(MaxLengthAttribute)))
                    .Where(prop => prop.PropertyType == typeof(string))
                    .Where(prop => prop.CanRead && prop.CanWrite)
                    .ToList());
            }

            var props = _curveProperties[GetType()];
            if (index < 0 || index >= props.Count)
                return null;

            return props[index];
        }
    }
}