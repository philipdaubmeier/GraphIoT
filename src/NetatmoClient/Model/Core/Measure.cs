﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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

    [DebuggerDisplay("{Type,nq}")]
    public record Measure(MeasureType Type) : IComparable, IComparable<Measure>, IEquatable<Measure>
    {
        private static Dictionary<string, MeasureType>? _mappingDict = null;
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

        public static implicit operator Measure(MeasureType measure) => new(measure);

        public static implicit operator Measure(string measure)
        {
            var input = measure.Trim().ToLowerInvariant();
            if (Mapping.TryGetValue(input, out MeasureType value))
                return new Measure(value);
            if (input == "wind")
                return new Measure(MeasureType.WindStrength);
            return new Measure(MeasureType.Temperature);
        }

        public static implicit operator MeasureType(Measure measure) => measure.Type;

        public int CompareTo(Measure? value) => Type.CompareTo(value?.Type);

        public int CompareTo(object? value) => Type.CompareTo((value as Measure)?.Type ?? value);

        public override string ToString()
        {
            return Type switch
            {
                MeasureType.Temperature => "temperature",
                MeasureType.CO2 => "co2",
                MeasureType.Humidity => "humidity",
                MeasureType.Pressure => "pressure",
                MeasureType.Noise => "noise",
                MeasureType.Rain => "rain",
                MeasureType.WindStrength => "windstrength",
                MeasureType.WindAngle => "windangle",
                MeasureType.Guststrength => "guststrength",
                MeasureType.GustAngle => "gustangle",
                MeasureType.MinTemp => "min_temp",
                MeasureType.MaxTemp => "max_temp",
                MeasureType.MinHum => "min_hum",
                MeasureType.MaxHum => "max_hum",
                MeasureType.MinPressure => "min_pressure",
                MeasureType.MaxPressure => "max_pressure",
                MeasureType.MinNoise => "min_noise",
                MeasureType.MaxNoise => "max_noise",
                MeasureType.SumRain => "sum_rain",
                MeasureType.MaxGust => "max_gust",
                MeasureType.DateMaxHum => "date_max_hum",
                MeasureType.DateMinPressure => "date_min_pressure",
                MeasureType.DateMaxPressure => "date_max_pressure",
                MeasureType.DateMinNoise => "date_min_noise",
                MeasureType.DateMaxNoise => "date_max_noise",
                MeasureType.DateMinCo2 => "date_min_co2",
                MeasureType.DateMaxCo2 => "date_max_co2",
                MeasureType.DateMaxGust => "date_max_gust",
                _ => "temperature",
            };
        }
    }
}