using PhilipDaubmeier.NetatmoClient.Model.Core;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhilipDaubmeier.NetatmoHost.Database
{
    public class NetatmoModuleMeasure
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required, MaxLength(17)]
        public string DeviceId { get; set; }

        [Required, MaxLength(17)]
        public string ModuleId { get; set; }

        [Required, MaxLength(20)]
        public string Measure { get; set; }

        [Required]
        public int Decimals { get; set; }

        /// <summary>
        /// From the measurements of the last years and knowhow about plausible min/max values
        /// (decimal precision is given with the written decimals after the point):
        /// 
        ///  measure      minValues maxValues decimalPlaces -> allows for range
        ///  temperature: -50       50        1                -3276.8 to 3276.7
        ///  co2:         350       5000      0                -32768 to 32767
        ///  humidity:    0         100       1                -3276.8 to 3276.7
        ///  noise:       1         75        1                -3276.8 to 3276.7
        ///  pressure:    950.0     1050.0    1                -3276.8 to 3276.7
        ///  rain:        0.000     3.999     3                -32.768 to 32.767
        ///  windstrength 0         6         1                -3276.8 to 3276.7
        ///  windangle    -1        360       1                -3276.8 to 3276.7
        ///  guststrength 1         12        1                -3276.8 to 3276.7
        ///  gustangle    -1        360       1                -3276.8 to 3276.7
        /// </summary>
        public void SetDecimalsByMeasureType(Measure type)
        {
            switch ((MeasureType)type)
            {
                case MeasureType.Temperature: Decimals = 1; break;
                case MeasureType.CO2: Decimals = 0; break;
                case MeasureType.Humidity: Decimals = 1; break;
                case MeasureType.Pressure: Decimals = 1; break;
                case MeasureType.Noise: Decimals = 1; break;
                case MeasureType.Rain: Decimals = 3; break;
                case MeasureType.WindStrength: Decimals = 1; break;
                case MeasureType.WindAngle: Decimals = 1; break;
                case MeasureType.Guststrength: Decimals = 1; break;
                case MeasureType.GustAngle: Decimals = 1; break;
                case MeasureType.MinTemp: Decimals = 1; break;
                case MeasureType.MaxTemp: Decimals = 1; break;
                case MeasureType.MinHum: Decimals = 1; break;
                case MeasureType.MaxHum: Decimals = 1; break;
                case MeasureType.MinPressure: Decimals = 1; break;
                case MeasureType.MaxPressure: Decimals = 1; break;
                case MeasureType.MinNoise: Decimals = 1; break;
                case MeasureType.MaxNoise: Decimals = 1; break;
                case MeasureType.SumRain: Decimals = 0; break;
                case MeasureType.MaxGust: Decimals = 1; break;
                case MeasureType.DateMaxHum: Decimals = 1; break;
                case MeasureType.DateMinPressure: Decimals = 1; break;
                case MeasureType.DateMaxPressure: Decimals = 1; break;
                case MeasureType.DateMinNoise: Decimals = 1; break;
                case MeasureType.DateMaxNoise: Decimals = 1; break;
                case MeasureType.DateMinCo2: Decimals = 0; break;
                case MeasureType.DateMaxCo2: Decimals = 0; break;
                case MeasureType.DateMaxGust: Decimals = 1; break;
                default: goto case MeasureType.Temperature;
            }
        }

        [MaxLength(30)]
        public string StationName { get; set; }

        [MaxLength(30)]
        public string ModuleName { get; set; }
    }
}