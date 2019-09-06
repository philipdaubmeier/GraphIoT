using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace PhilipDaubmeier.NetatmoClient.Model.Core
{
    public enum MeasureType
    {
        Temperature,
        CO2,
        Humidity,
        Pressure,
        Noise,
        Rain,
        WindStrength,
        WindAngle,
        Guststrength,
        GustAngle,
        MinTemp,
        MaxTemp,
        MinHum,
        MaxHum,
        MinPressure,
        MaxPressure,
        MinNoise,
        MaxNoise,
        SumRain,
        MaxGust,
        DateMaxHum,
        DateMinPressure,
        DateMaxPressure,
        DateMinNoise,
        DateMaxNoise,
        DateMinCo2,
        DateMaxCo2,
        DateMaxGust
    }

    [DebuggerDisplay("{_measure,nq}")]
    public class Measure : IComparable, IComparable<Measure>, IEquatable<Measure>
    {
        private static Dictionary<string, MeasureType> _mappingDict = null;
        private static Dictionary<string, MeasureType> Mapping
        {
            get
            {
                if (_mappingDict == null)
                    _mappingDict = ((MeasureType[])Enum.GetValues(typeof(MeasureType)))
                        .ToDictionary(x => ((Measure)x).ToString().ToLowerInvariant(), x => x);

                return _mappingDict;
            }
        }

        private readonly MeasureType _measure;

        public Measure(MeasureType measure)
        {
            _measure = measure;
        }

        public static implicit operator Measure(MeasureType measure)
        {
            return new Measure(measure);
        }

        public static implicit operator Measure(string measure)
        {
            if (Mapping.TryGetValue(measure.Trim().ToLowerInvariant(), out MeasureType value))
                return new Measure(value);
            return null;
        }

        public static implicit operator MeasureType(Measure measure)
        {
            return measure._measure;
        }

        public static bool operator !=(Measure measure1, Measure measure2)
        {
            return !(measure1 == measure2);
        }

        public static bool operator ==(Measure measure1, Measure measure2)
        {
            if (measure1 is null || measure2 is null)
                return ReferenceEquals(measure1, measure2);
            return measure1._measure == measure2._measure;
        }

        public int CompareTo(Measure value)
        {
            return _measure.CompareTo(value._measure);
        }

        public int CompareTo(object value)
        {
            return _measure.CompareTo((value as Measure)?._measure ?? value);
        }

        public bool Equals(Measure measure)
        {
            return this == measure;
        }

        public override bool Equals(object obj)
        {
            return this == (obj as Measure);
        }

        public override int GetHashCode()
        {
            return _measure.GetHashCode();
        }

        public override string ToString()
        {
            switch (_measure)
            {
                case MeasureType.Temperature: return "temperature";
                case MeasureType.CO2: return "co2";
                case MeasureType.Humidity: return "humidity";
                case MeasureType.Pressure: return "pressure";
                case MeasureType.Noise: return "noise";
                case MeasureType.Rain: return "rain";
                case MeasureType.WindStrength: return "windstrength";
                case MeasureType.WindAngle: return "windangle";
                case MeasureType.Guststrength: return "guststrength";
                case MeasureType.GustAngle: return "gustangle";
                case MeasureType.MinTemp: return "min_temp";
                case MeasureType.MaxTemp: return "max_temp";
                case MeasureType.MinHum: return "min_hum";
                case MeasureType.MaxHum: return "max_hum";
                case MeasureType.MinPressure: return "min_pressure";
                case MeasureType.MaxPressure: return "max_pressure";
                case MeasureType.MinNoise: return "min_noise";
                case MeasureType.MaxNoise: return "max_noise";
                case MeasureType.SumRain: return "sum_rain";
                case MeasureType.MaxGust: return "max_gust";
                case MeasureType.DateMaxHum: return "date_max_hum";
                case MeasureType.DateMinPressure: return "date_min_pressure";
                case MeasureType.DateMaxPressure: return "date_max_pressure";
                case MeasureType.DateMinNoise: return "date_min_noise";
                case MeasureType.DateMaxNoise: return "date_max_noise";
                case MeasureType.DateMinCo2: return "date_min_co2";
                case MeasureType.DateMaxCo2: return "date_max_co2";
                case MeasureType.DateMaxGust: return "date_max_gust";
                default: goto case MeasureType.Temperature;
            }
        }
    }
}