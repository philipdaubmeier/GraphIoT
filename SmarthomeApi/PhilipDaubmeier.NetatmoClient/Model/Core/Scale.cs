using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

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

    [DebuggerDisplay("{_scale,nq}")]
    public class Scale : IComparable, IComparable<Scale>, IEquatable<Scale>
    {
        private static Dictionary<string, MeasureScale> _mappingDict = null;
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

        private readonly MeasureScale _scale;

        public Scale(MeasureScale scale)
        {
            _scale = scale;
        }

        public static implicit operator Scale(MeasureScale scale)
        {
            return new Scale(scale);
        }

        public static implicit operator Scale(string scale)
        {
            if (Mapping.TryGetValue(scale.Trim().ToLowerInvariant(), out MeasureScale value))
                return new Scale(value);
            return null;
        }

        public static implicit operator MeasureScale(Scale scale)
        {
            return scale._scale;
        }

        public static implicit operator TimeSpan(Scale scale)
        {
            switch (scale._scale)
            {
                case MeasureScale.Scale30Min: return new TimeSpan(0, 30, 0);
                case MeasureScale.Scale1Hour: return new TimeSpan(1, 0, 0);
                case MeasureScale.Scale3Hours: return new TimeSpan(3, 0, 0);
                case MeasureScale.Scale1Day: return new TimeSpan(1, 0, 0, 0);
                case MeasureScale.Scale1Week: return new TimeSpan(7, 0, 0, 0);
                case MeasureScale.Scale1Month: return new TimeSpan(30, 10, 28, 48);
                case MeasureScale.ScaleMax: goto default;
                default: return new TimeSpan(0, 1, 0);
            }
        }

        public static bool operator !=(Scale scale1, Scale scale2)
        {
            return !(scale1 == scale2);
        }

        public static bool operator ==(Scale scale1, Scale scale2)
        {
            if (scale1 is null || scale2 is null)
                return ReferenceEquals(scale1, scale2);
            return scale1._scale == scale2._scale;
        }

        public int CompareTo(Scale value)
        {
            return _scale.CompareTo(value._scale);
        }

        public int CompareTo(object value)
        {
            return _scale.CompareTo((value as Scale)?._scale ?? value);
        }

        public bool Equals(Scale scale)
        {
            return this == scale;
        }

        public override bool Equals(object obj)
        {
            return this == (obj as Scale);
        }

        public override int GetHashCode()
        {
            return _scale.GetHashCode();
        }

        public override string ToString()
        {
            switch (_scale)
            {
                case MeasureScale.Scale30Min: return "30min";
                case MeasureScale.Scale1Hour: return "1hour";
                case MeasureScale.Scale3Hours: return "3hours";
                case MeasureScale.Scale1Day: return "1day";
                case MeasureScale.Scale1Week: return "1week";
                case MeasureScale.Scale1Month: return "1month";
                case MeasureScale.ScaleMax: goto default;
                default: return "max";
            }
        }
    }
}