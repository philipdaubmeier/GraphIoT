using SmarthomeApi.FormatParsers;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SmarthomeApi.Model;
using CompactTimeSeries;

namespace SmarthomeApi.Database.Model
{
    public class DigitalstromZoneSensorData
    {
        private const int interval5min = 60 / 5 * 24;
        private const int decimalPlaces = 2;

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public int ZoneId { get; set; }

        [ForeignKey("ZoneId")]
        public DigitalstromZone Zone { get; set; }

        [Required]
        public DateTime Day { get; set; }
        
        [MaxLength(800)]
        public string TemperatureCurve { get; set; }

        [MaxLength(800)]
        public string HumidityCurve { get; set; }

        [NotMapped]
        public TimeSeries<double> TemperatureSeries
        {
            get => TemperatureCurve.ToTimeseries<double>(Day, Day.AddDays(1), interval5min, decimalPlaces);
            set { TemperatureCurve = value.ToBase64(decimalPlaces); }
        }

        [NotMapped]
        public TimeSeries<double> HumiditySeries
        {
            get => HumidityCurve.ToTimeseries<double>(Day, Day.AddDays(1), interval5min, decimalPlaces);
            set { HumidityCurve = value.ToBase64(decimalPlaces); }
        }
    }
}
