using PhilipDaubmeier.NetatmoClient.Model.Core;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhilipDaubmeier.GraphIoT.Netatmo.Database
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
            Decimals = ((MeasureType)type) switch
            {
                MeasureType.Temperature => 1,
                MeasureType.CO2 => 0,
                MeasureType.Humidity => 1,
                MeasureType.Pressure => 1,
                MeasureType.Noise => 1,
                MeasureType.Rain => 3,
                MeasureType.WindStrength => 1,
                MeasureType.WindAngle => 1,
                MeasureType.Guststrength => 1,
                MeasureType.GustAngle => 1,
                MeasureType.MinTemp => 1,
                MeasureType.MaxTemp => 1,
                MeasureType.MinHum => 1,
                MeasureType.MaxHum => 1,
                MeasureType.MinPressure => 1,
                MeasureType.MaxPressure => 1,
                MeasureType.MinNoise => 1,
                MeasureType.MaxNoise => 1,
                MeasureType.SumRain => 0,
                MeasureType.MaxGust => 1,
                MeasureType.DateMaxHum => 1,
                MeasureType.DateMinPressure => 1,
                MeasureType.DateMaxPressure => 1,
                MeasureType.DateMinNoise => 1,
                MeasureType.DateMaxNoise => 1,
                MeasureType.DateMinCo2 => 0,
                MeasureType.DateMaxCo2 => 0,
                MeasureType.DateMaxGust => 1,
                _ => 1,
            };
        }

        [MaxLength(30)]
        public string StationName { get; set; }

        [MaxLength(30)]
        public string ModuleName { get; set; }
    }
}