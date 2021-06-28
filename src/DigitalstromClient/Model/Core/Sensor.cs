using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace PhilipDaubmeier.DigitalstromClient.Model.Core
{
    /// <summary>
    /// All sensor types used in digitalstrom.
    /// </summary>
    public enum SensorType
    {
        ActivePower = 4,
        OutputCurrent = 5,
        ElectricMeter = 6,
        TemperatureIndoors = 9,
        TemperatureOutdoors = 10,
        BrightnessIndoors = 11,
        BrightnessOutdoors = 12,
        HumidityIndoors = 13,
        HumidityOutdoors = 14,
        AirPressure = 15,
        GustSpeed = 16,
        GustDirection = 17,
        WindSpeed = 18,
        WindDirection = 19,
        Precipitation = 20,
        CO2Concentration = 21,
        COConcentration = 22,
        SoundPressureLevel = 25,
        RoomTemperatureSetpoint = 50,
        RoomTemperatureControlVariable = 51,
        /// <summary>Status sensor value. Value bits as defined in ds-basics.pdf (Malfunction = 1, Service = 2)</summary>
        Status = 60,
        Reserved1 = 61,
        Reserved2 = 62,
        OutputCurrent16A = 64,
        ActivePowerVA = 65,
        Temperature = 66,
        Brightness = 67,
        Humidity = 68,
        WaterQuantity = 71,
        WaterFlowRate = 72,
        SunAzimuth = 76,
        SunElevation = 77,
        NotUsed = 253,
        UnknownType = 255,
    };

    public record Sensor(SensorType Type) : IComparable, IComparable<Sensor>, IEquatable<Sensor>, IFormattable
    {
        public SensorType Type { get; init; } = NormalizeSensorType((int)Type);

        private static SensorType NormalizeSensorType(int type)
        {
            int i = Math.Min(Math.Max(type, 0), 255);
            if (i < 4 || i == 7 || i == 8 || i == 23 || i == 24 || (i > 22 && i < 25)
                || (i > 25 && i < 50) || (i > 51 && i < 60) || i == 63 || (i > 68 && i < 71)
                || (i > 72 && i < 76) || (i > 77 && i < 253) || i == 254)
                i = 255;
            return (SensorType)i;
        }

        public static implicit operator Sensor(SensorType type) => new((int)type);

        public static implicit operator SensorType(Sensor type) => type.Type;

        public static implicit operator Sensor(long type) => (int)type;

        public static implicit operator Sensor(int type) => new(NormalizeSensorType(type));

        public static implicit operator int(Sensor type) => (int)type.Type;

        public static implicit operator Sensor(string type)
        {
            if (!int.TryParse(type, out int t))
                return new Sensor((int)SensorType.UnknownType);

            return new Sensor(t);
        }

        public int CompareTo(Sensor? value) => Type.CompareTo(value?.Type);

        public int CompareTo(object? value) => Type.CompareTo((value as Sensor)?.Type ?? value);

        public override string ToString() => ToString(null, null);

        public static IEnumerable<Sensor> GetTypes()
        {
            return ((SensorType[])Enum.GetValues(typeof(SensorType))).Select(x => (Sensor)(int)x);
        }

        /// <summary>
        /// Converts the numeric value of this instance to its equivalent string representation
        /// using the specified format and culture-specific format information.
        /// </summary>
        /// <param name="format">
        /// Null for an invariant default format 'Sensor {sensor-type-num}: {sensor-type-name}'.
        /// "D" or "d" for a localized displayable string of the sensor type,
        /// if available for the given language of the format provider.
        /// </param>
        /// <param name="formatProvider">An object that supplies culture-specific formatting information.</param>
        /// <returns>
        /// The string representation of the value of this instance as specified by format and provider.
        /// </returns>
        public string ToString(string? format = null, IFormatProvider? formatProvider = null)
        {
            if (format is null)
                return $"Sensor {(int)Type}: {Enum.GetName(typeof(SensorType), Type) ?? string.Empty}";

            if (!format.Equals("d", StringComparison.InvariantCultureIgnoreCase))
                throw new FormatException($"Did not recognize format '{format}'");

            if (formatProvider is CultureInfo culture)
                Locale.Sensor.Culture = culture;

            return Type switch
            {
                SensorType.ActivePower => Locale.Sensor.ActivePower,
                SensorType.OutputCurrent => Locale.Sensor.OutputCurrent,
                SensorType.ElectricMeter => Locale.Sensor.ElectricMeter,
                SensorType.TemperatureIndoors => Locale.Sensor.TemperatureIndoors,
                SensorType.TemperatureOutdoors => Locale.Sensor.TemperatureOutdoors,
                SensorType.BrightnessIndoors => Locale.Sensor.BrightnessIndoors,
                SensorType.BrightnessOutdoors => Locale.Sensor.BrightnessOutdoors,
                SensorType.HumidityIndoors => Locale.Sensor.RelativeHumidityIndoors,
                SensorType.HumidityOutdoors => Locale.Sensor.RelativeHumidityOutdoors,
                SensorType.AirPressure => Locale.Sensor.AirPressure,
                SensorType.GustSpeed => Locale.Sensor.WindGustspeed,
                SensorType.GustDirection => Locale.Sensor.WindGustdirection,
                SensorType.WindSpeed => Locale.Sensor.WindSpeed,
                SensorType.WindDirection => Locale.Sensor.WindDirection,
                SensorType.Precipitation => Locale.Sensor.Precipitation,
                SensorType.CO2Concentration => Locale.Sensor.CarbonDioxideConcentration,
                SensorType.COConcentration => Locale.Sensor.CarbonMonoxideConcentration,
                SensorType.SoundPressureLevel => Locale.Sensor.SoundPressureLevel,
                SensorType.RoomTemperatureSetpoint => Locale.Sensor.RoomTemperatureSetpoint,
                SensorType.RoomTemperatureControlVariable => Locale.Sensor.RoomTemperatureControl,
                SensorType.Status => Locale.Sensor.Reserved,
                SensorType.Reserved1 => Locale.Sensor.Reserved,
                SensorType.Reserved2 => Locale.Sensor.Reserved,
                SensorType.OutputCurrent16A => Locale.Sensor.OutputCurrentH,
                SensorType.ActivePowerVA => Locale.Sensor.PowerConsumption,
                SensorType.Temperature => Locale.Sensor.Temperature,
                SensorType.Brightness => Locale.Sensor.Brightness,
                SensorType.Humidity => Locale.Sensor.RelativeHumidity,
                SensorType.WaterQuantity => Locale.Sensor.Reserved,
                SensorType.WaterFlowRate => Locale.Sensor.Reserved,
                SensorType.SunAzimuth => Locale.Sensor.SunAzimuth,
                SensorType.SunElevation => Locale.Sensor.SunElevation,
                SensorType.NotUsed => Locale.Sensor.NotUsed,
                SensorType.UnknownType => Locale.Sensor.Unknown,
                _ => Locale.Sensor.Unknown,
            };
        }
    }
}