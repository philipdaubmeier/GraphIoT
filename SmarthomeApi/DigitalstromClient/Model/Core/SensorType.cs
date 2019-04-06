using System;
using System.Collections.Generic;
using System.Linq;

namespace DigitalstromClient.Model.Core
{
    public class SensorType
    {
        /// <summary>
        /// All sensor types used in digitalstrom.
        /// </summary>
        public enum TypeValue
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
        
        private TypeValue _type = TypeValue.UnknownType;

        private SensorType(int typeCode)
        {
            int i = Math.Min(Math.Max(typeCode, 0), 255);
            if (i < 4 || i == 7 || i == 8 || i == 23 || i == 24 || (i > 22 && i < 25)
                || (i > 25 && i < 50) || (i > 51 && i < 60) || i == 63 || (i > 65 && i < 253) || i == 254)
                i = 255;
            _type = (TypeValue)i;
        }

        public static implicit operator int(SensorType type)
        {
            return (int)type._type;
        }

        public static implicit operator SensorType(TypeValue type)
        {
            return new SensorType((int)type);
        }

        public static implicit operator SensorType(int type)
        {
            return new SensorType(type);
        }

        public static implicit operator SensorType(string type)
        {
            int t;
            if (!int.TryParse(type, out t))
                return new SensorType((int)TypeValue.UnknownType);

            return new SensorType(t);
        }

        public static IEnumerable<SensorType> GetTypes()
        {
            return ((TypeValue[])Enum.GetValues(typeof(TypeValue))).Select(x => (SensorType)(int)x);
        }

        public static bool operator !=(SensorType type1, SensorType type2)
        {
            return !(type1 == type2);
        }

        public static bool operator ==(SensorType type1, SensorType type2)
        {
            if ((object)type1 == null || (object)type2 == null)
                return ReferenceEquals(type1, type2);
            return type1._type == type2._type;
        }

        public static bool operator !=(SensorType type1, TypeValue type2)
        {
            return !(type1 == type2);
        }

        public static bool operator ==(SensorType type1, TypeValue type2)
        {
            if ((object)type1 == null)
                return false;
            return type1._type == type2;
        }

        public override bool Equals(object obj)
        {
            return ((SensorType)obj)._type == _type;
        }

        public override int GetHashCode()
        {
            return _type.GetHashCode();
        }

        public override string ToString()
        {
            switch (_type)
            {
                case TypeValue.ActivePower: return "SensorType 4: ActivePower";
                case TypeValue.OutputCurrent: return "SensorType 5: OutputCurrent";
                case TypeValue.ElectricMeter: return "SensorType 6: ElectricMeter";
                case TypeValue.TemperatureIndoors: return "SensorType 9: TemperatureIndoors";
                case TypeValue.TemperatureOutdoors: return "SensorType 10: TemperatureOutdoors";
                case TypeValue.BrightnessIndoors: return "SensorType 11: BrightnessIndoors";
                case TypeValue.BrightnessOutdoors: return "SensorType 12: BrightnessOutdoors";
                case TypeValue.HumidityIndoors: return "SensorType 13: HumidityIndoors";
                case TypeValue.HumidityOutdoors: return "SensorType 14: HumidityOutdoors";
                case TypeValue.AirPressure: return "SensorType 15: AirPressure";
                case TypeValue.GustSpeed: return "SensorType 16: GustSpeed";
                case TypeValue.GustDirection: return "SensorType 17: GustDirection";
                case TypeValue.WindSpeed: return "SensorType 18: WindSpeed";
                case TypeValue.WindDirection: return "SensorType 19: WindDirection";
                case TypeValue.Precipitation: return "SensorType 20: Precipitation";
                case TypeValue.CO2Concentration: return "SensorType 21: CO2Concentration";
                case TypeValue.COConcentration: return "SensorType 22: COConcentration";
                case TypeValue.SoundPressureLevel: return "SensorType 25: SoundPressureLevel";
                case TypeValue.RoomTemperatureSetpoint: return "SensorType 50: RoomTemperatureSetpoint";
                case TypeValue.RoomTemperatureControlVariable: return "SensorType 51: RoomTemperatureControlVariable";
                case TypeValue.Status: return "SensorType 60: Status";
                case TypeValue.Reserved1: return "SensorType 61: Reserved1";
                case TypeValue.Reserved2: return "SensorType 62: Reserved2";
                case TypeValue.OutputCurrent16A: return "SensorType 64: OutputCurrent16A";
                case TypeValue.ActivePowerVA: return "SensorType 65: ActivePowerVA";
                case TypeValue.NotUsed: return "SensorType 253: NotUsed";
                case TypeValue.UnknownType: return "SensorType 255: UnknownType";
                default: return string.Format("SensorType {0}: Not defined!", _type);
            }
        }
    }
}