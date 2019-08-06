using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.TimeseriesHostCommon.Parsers;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhilipDaubmeier.SonnenHost.Database
{
    public class SonnenEnergyData
    {
        private const int interval1min = 60 * 24;
        private const int decimalPlaces = 2;

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public DateTime Day { get; set; }

        [MaxLength(4000)]
        public string ProductionPowerCurve { get; set; }

        [MaxLength(4000)]
        public string ConsumptionPowerCurve { get; set; }

        [MaxLength(4000)]
        public string DirectUsagePowerCurve { get; set; }

        [MaxLength(4000)]
        public string BatteryChargingCurve { get; set; }

        [MaxLength(4000)]
        public string BatteryDischargingCurve { get; set; }

        [MaxLength(4000)]
        public string GridFeedinCurve { get; set; }

        [MaxLength(4000)]
        public string GridPurchaseCurve { get; set; }

        [MaxLength(4000)]
        public string BatteryUsocCurve { get; set; }

        [NotMapped]
        public TimeSeries<int> ProductionPowerSeries
        {
            get => ProductionPowerCurve.ToTimeseries<int>(new TimeSeriesSpan(Day, Day.AddDays(1), interval1min));
            set { ProductionPowerCurve = value.ToBase64(); }
        }

        [NotMapped]
        public TimeSeries<int> ConsumptionPowerSeries
        {
            get => ConsumptionPowerCurve.ToTimeseries<int>(new TimeSeriesSpan(Day, Day.AddDays(1), interval1min));
            set { ConsumptionPowerCurve = value.ToBase64(); }
        }

        [NotMapped]
        public TimeSeries<int> DirectUsagePowerSeries
        {
            get => DirectUsagePowerCurve.ToTimeseries<int>(new TimeSeriesSpan(Day, Day.AddDays(1), interval1min));
            set { DirectUsagePowerCurve = value.ToBase64(); }
        }

        [NotMapped]
        public TimeSeries<int> BatteryChargingSeries
        {
            get => BatteryChargingCurve.ToTimeseries<int>(new TimeSeriesSpan(Day, Day.AddDays(1), interval1min));
            set { BatteryChargingCurve = value.ToBase64(); }
        }

        [NotMapped]
        public TimeSeries<int> BatteryDischargingSeries
        {
            get => BatteryDischargingCurve.ToTimeseries<int>(new TimeSeriesSpan(Day, Day.AddDays(1), interval1min));
            set { BatteryDischargingCurve = value.ToBase64(); }
        }

        [NotMapped]
        public TimeSeries<int> GridFeedinSeries
        {
            get => GridFeedinCurve.ToTimeseries<int>(new TimeSeriesSpan(Day, Day.AddDays(1), interval1min));
            set { GridFeedinCurve = value.ToBase64(); }
        }

        [NotMapped]
        public TimeSeries<int> GridPurchaseSeries
        {
            get => GridPurchaseCurve.ToTimeseries<int>(new TimeSeriesSpan(Day, Day.AddDays(1), interval1min));
            set { GridPurchaseCurve = value.ToBase64(); }
        }

        [NotMapped]
        public TimeSeries<double> BatteryUsocSeries
        {
            get => BatteryUsocCurve.ToTimeseries<double>(new TimeSeriesSpan(Day, Day.AddDays(1), interval1min), decimalPlaces);
            set { BatteryUsocCurve = value.ToBase64(decimalPlaces); }
        }
    }
}