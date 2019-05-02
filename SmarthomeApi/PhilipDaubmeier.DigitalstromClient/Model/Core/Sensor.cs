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

    public class Sensor
    {
        private SensorType _type = SensorType.UnknownType;

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
            int t;
            if (!int.TryParse(type, out t))
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
            if ((object)type1 == null || (object)type2 == null)
                return ReferenceEquals(type1, type2);
            return type1._type == type2._type;
        }

        public static bool operator !=(Sensor type1, SensorType type2)
        {
            return !(type1 == type2);
        }

        public static bool operator ==(Sensor type1, SensorType type2)
        {
            if ((object)type1 == null)
                return false;
            return type1._type == type2;
        }

        public override bool Equals(object obj)
        {
            return ((Sensor)obj)._type == _type;
        }

        public override int GetHashCode()
        {
            return _type.GetHashCode();
        }

        public override string ToString()
        {
            switch (_type)
            {
                case SensorType.ActivePower: return "SensorType 4: ActivePower";
                case SensorType.OutputCurrent: return "SensorType 5: OutputCurrent";
                case SensorType.ElectricMeter: return "SensorType 6: ElectricMeter";
                case SensorType.TemperatureIndoors: return "SensorType 9: TemperatureIndoors";
                case SensorType.TemperatureOutdoors: return "SensorType 10: TemperatureOutdoors";
                case SensorType.BrightnessIndoors: return "SensorType 11: BrightnessIndoors";
                case SensorType.BrightnessOutdoors: return "SensorType 12: BrightnessOutdoors";
                case SensorType.HumidityIndoors: return "SensorType 13: HumidityIndoors";
                case SensorType.HumidityOutdoors: return "SensorType 14: HumidityOutdoors";
                case SensorType.AirPressure: return "SensorType 15: AirPressure";
                case SensorType.GustSpeed: return "SensorType 16: GustSpeed";
                case SensorType.GustDirection: return "SensorType 17: GustDirection";
                case SensorType.WindSpeed: return "SensorType 18: WindSpeed";
                case SensorType.WindDirection: return "SensorType 19: WindDirection";
                case SensorType.Precipitation: return "SensorType 20: Precipitation";
                case SensorType.CO2Concentration: return "SensorType 21: CO2Concentration";
                case SensorType.COConcentration: return "SensorType 22: COConcentration";
                case SensorType.SoundPressureLevel: return "SensorType 25: SoundPressureLevel";
                case SensorType.RoomTemperatureSetpoint: return "SensorType 50: RoomTemperatureSetpoint";
                case SensorType.RoomTemperatureControlVariable: return "SensorType 51: RoomTemperatureControlVariable";
                case SensorType.Status: return "SensorType 60: Status";
                case SensorType.Reserved1: return "SensorType 61: Reserved1";
                case SensorType.Reserved2: return "SensorType 62: Reserved2";
                case SensorType.OutputCurrent16A: return "SensorType 64: OutputCurrent16A";
                case SensorType.ActivePowerVA: return "SensorType 65: ActivePowerVA";
                case SensorType.NotUsed: return "SensorType 253: NotUsed";
                case SensorType.UnknownType: return "SensorType 255: UnknownType";
                default: return string.Format("SensorType {0}: Not defined!", _type);
            }
        }
    }
}