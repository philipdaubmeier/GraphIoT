using System;
using System.Collections.Generic;
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
        NotUsed = 253,
        UnknownType = 255,
    };

    public class Sensor : IComparable, IComparable<Sensor>, IEquatable<Sensor>
    {
        private readonly SensorType _type = SensorType.UnknownType;

        private Sensor(int typeCode)
        {
            int i = Math.Min(Math.Max(typeCode, 0), 255);
            if (i < 4 || i == 7 || i == 8 || i == 23 || i == 24 || (i > 22 && i < 25)
                || (i > 25 && i < 50) || (i > 51 && i < 60) || i == 63 || (i > 65 && i < 253) || i == 254)
                i = 255;
            _type = (SensorType)i;
        }

        public static implicit operator Sensor(SensorType type)
        {
            return new Sensor((int)type);
        }

        public static implicit operator SensorType(Sensor type)
        {
            return type._type;
        }

        public static implicit operator Sensor(long type)
        {
            return (int)type;
        }

        public static implicit operator Sensor(int type)
        {
            return new Sensor(type);
        }

        public static implicit operator int(Sensor type)
        {
            return (int)type._type;
        }

        public static implicit operator Sensor(string type)
        {
            if (!int.TryParse(type, out int t))
                return new Sensor((int)SensorType.UnknownType);

            return new Sensor(t);
        }

        public static IEnumerable<Sensor> GetTypes()
        {
            return ((SensorType[])Enum.GetValues(typeof(SensorType))).Select(x => (Sensor)(int)x);
        }

        public static bool operator !=(Sensor type1, Sensor type2)
        {
            return !(type1 == type2);
        }

        public static bool operator ==(Sensor type1, Sensor type2)
        {
            if (type1 is null || type2 is null)
                return ReferenceEquals(type1, type2);
            return type1._type == type2._type;
        }

        public static bool operator !=(Sensor type1, SensorType type2)
        {
            return !(type1 == type2);
        }

        public static bool operator ==(Sensor type1, SensorType type2)
        {
            if (type1 is null)
                return false;
            return type1._type == type2;
        }

        public int CompareTo(Sensor value)
        {
            return _type.CompareTo(value._type);
        }

        public int CompareTo(object? value)
        {
            return _type.CompareTo((value as Sensor)?._type ?? value);
        }

        public bool Equals(Sensor sensor)
        {
            return this == sensor;
        }

        public override bool Equals(object? obj)
        {
            return obj is Sensor sensor && this == sensor;
        }

        public override int GetHashCode()
        {
            return _type.GetHashCode();
        }

        public override string ToString()
        {
            return _type switch
            {
                SensorType.ActivePower => "SensorType 4: ActivePower",
                SensorType.OutputCurrent => "SensorType 5: OutputCurrent",
                SensorType.ElectricMeter => "SensorType 6: ElectricMeter",
                SensorType.TemperatureIndoors => "SensorType 9: TemperatureIndoors",
                SensorType.TemperatureOutdoors => "SensorType 10: TemperatureOutdoors",
                SensorType.BrightnessIndoors => "SensorType 11: BrightnessIndoors",
                SensorType.BrightnessOutdoors => "SensorType 12: BrightnessOutdoors",
                SensorType.HumidityIndoors => "SensorType 13: HumidityIndoors",
                SensorType.HumidityOutdoors => "SensorType 14: HumidityOutdoors",
                SensorType.AirPressure => "SensorType 15: AirPressure",
                SensorType.GustSpeed => "SensorType 16: GustSpeed",
                SensorType.GustDirection => "SensorType 17: GustDirection",
                SensorType.WindSpeed => "SensorType 18: WindSpeed",
                SensorType.WindDirection => "SensorType 19: WindDirection",
                SensorType.Precipitation => "SensorType 20: Precipitation",
                SensorType.CO2Concentration => "SensorType 21: CO2Concentration",
                SensorType.COConcentration => "SensorType 22: COConcentration",
                SensorType.SoundPressureLevel => "SensorType 25: SoundPressureLevel",
                SensorType.RoomTemperatureSetpoint => "SensorType 50: RoomTemperatureSetpoint",
                SensorType.RoomTemperatureControlVariable => "SensorType 51: RoomTemperatureControlVariable",
                SensorType.Status => "SensorType 60: Status",
                SensorType.Reserved1 => "SensorType 61: Reserved1",
                SensorType.Reserved2 => "SensorType 62: Reserved2",
                SensorType.OutputCurrent16A => "SensorType 64: OutputCurrent16A",
                SensorType.ActivePowerVA => "SensorType 65: ActivePowerVA",
                SensorType.NotUsed => "SensorType 253: NotUsed",
                SensorType.UnknownType => "SensorType 255: UnknownType",
                _ => string.Format("SensorType {0}: Not defined!", _type),
            };
        }
    }
}