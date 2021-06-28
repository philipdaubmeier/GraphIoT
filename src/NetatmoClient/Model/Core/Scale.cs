using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PhilipDaubmeier.NetatmoClient.Model.Core
{
    public enum MeasureScale
    {
        ScaleMax,
        Scale30Min,
        Scale1Hour,
        Scale3Hours,
        Scale1Day,
        Scale1Week,
        Scale1Month
    }

    [DebuggerDisplay("{Measure,nq}")]
    public record Scale(MeasureScale Measure) : IComparable, IComparable<Scale>, IEquatable<Scale>
    {
        private static Dictionary<string, MeasureScale>? _mappingDict = null;
        private static Dictionary<string, MeasureScale> Mapping
        {
            get
            {
                if (_mappingDict == null)
                    _mappingDict = ((MeasureScale[])Enum.GetValues(typeof(MeasureScale)))
                        .ToDictionary(x => ((Scale)x).ToString().ToLowerInvariant(), x => x);

                return _mappingDict;
            }
        }

        public static implicit operator Scale(MeasureScale scale) => new(scale);

        public static implicit operator Scale(string scale)
        {
            if (Mapping.TryGetValue(scale.Trim().ToLowerInvariant(), out MeasureScale value))
                return new Scale(value);
            return new Scale(MeasureScale.ScaleMax);
        }

        public static implicit operator MeasureScale(Scale scale) => scale.Measure;

        public int CompareTo(Scale? value) => Measure.CompareTo(value?.Measure);

        public int CompareTo(object? value) => Measure.CompareTo((value as Scale)?.Measure ?? value);

        public static implicit operator TimeSpan(Scale scale)
        {
            return scale.Measure switch
            {
                MeasureScale.Scale30Min => new TimeSpan(0, 30, 0),
                MeasureScale.Scale1Hour => new TimeSpan(1, 0, 0),
                MeasureScale.Scale3Hours => new TimeSpan(3, 0, 0),
                MeasureScale.Scale1Day => new TimeSpan(1, 0, 0, 0),
                MeasureScale.Scale1Week => new TimeSpan(7, 0, 0, 0),
                MeasureScale.Scale1Month => new TimeSpan(30, 10, 28, 48),
                _ => new TimeSpan(0, 1, 0),
            };
        }

        public override string ToString()
        {
            return Measure switch
            {
                MeasureScale.Scale30Min => "30min",
                MeasureScale.Scale1Hour => "1hour",
                MeasureScale.Scale3Hours => "3hours",
                MeasureScale.Scale1Day => "1day",
                MeasureScale.Scale1Week => "1week",
                MeasureScale.Scale1Month => "1month",
                _ => "max",
            };
        }
    }
}