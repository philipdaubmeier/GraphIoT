using Microsoft.Extensions.Logging;
using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.GraphIoT.WeConnect.Database;
using PhilipDaubmeier.WeConnectClient;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace PhilipDaubmeier.GraphIoT.WeConnect.Polling
{
    public class WeConnectPollingService : IWeConnectPollingService
    {
        private readonly ILogger _logger;
        private readonly IWeConnectDbContext _dbContext;
        private readonly WeConnectPortalClient _weConnectClient;

        public WeConnectPollingService(ILogger<WeConnectPollingService> logger, IWeConnectDbContext databaseContext, WeConnectPortalClient weConnectClient)
        {
            _logger = logger;
            _dbContext = databaseContext;
            _weConnectClient = weConnectClient;
        }

        public async Task PollValues()
        {
            _logger.LogInformation($"{DateTime.Now} WeConnect Background Service is polling new values...");

            try
            {
                await PollVehicleValues();
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"{DateTime.Now} Exception occurred in WeConnect background worker: {ex.Message}");
            }
        }

        public async Task PollVehicleValues()
        {
            var vin = (await _weConnectClient.GetVehicleList()).FirstOrDefault()?.Vin?.ToString() ?? string.Empty;
            var emanager = await _weConnectClient.GetEManager();
            var status = await _weConnectClient.GetVehicleStatus();
            var trips = await _weConnectClient.GetLatestTripStatistics();
            var details = await _weConnectClient.GetVehicleDetails();
            var time = DateTime.UtcNow;

            var mileageRaw = details.DistanceCovered.Replace(".", "").Replace(",", "");
            var mileage = int.Parse(mileageRaw);

            static bool ToBool(string? boolStr) => !string.IsNullOrWhiteSpace(boolStr) && !boolStr.Equals("OFF", StringComparison.InvariantCultureIgnoreCase);

            var batterySoc = status.BatteryLevel ?? emanager.Rbc.Status?.BatteryPercentage ?? 0;

            var length = trips.AllEntries.Select(x => (x.start + x.duration, x.trip.TripLength)).ToList();
            var duration = trips.AllEntries.Select(x => (x.start + x.duration, x.duration.TotalMinutes)).ToList();
            var consumedKwh = trips.AllEntries.Select(x => (x.start + x.duration, (x.trip.AverageElectricConsumption ?? 0d) * x.trip.TripLength / 100)).ToList();
            var averageConsumption = trips.AllEntries.Select(x => (x.start + x.duration, x.trip.AverageElectricConsumption ?? 0d)).ToList();
            var averageSpeed = trips.AllEntries.Select(x => (x.start + x.duration, x.trip.AverageSpeed)).ToList();

            var climateTemp = double.Parse(emanager.Rpc.Settings?.TargetTemperature, CultureInfo.InvariantCulture);
            var chargingState = ToBool(emanager.Rbc.Status?.ChargingState);
            var climateState = ToBool(emanager.Rpc.Status?.ClimatisationState);
            var windowMeltState = ToBool(emanager.Rpc.Status?.WindowHeatingStateFront?.ToString());
            var remoteHeatingState = emanager.Rdt.AuxHeatingEnabled;

            SaveSolarValues(_dbContext, time, vin, mileage, batterySoc, length, duration, averageSpeed, consumedKwh, averageConsumption, climateTemp, chargingState, climateState, windowMeltState, remoteHeatingState);
        }

        public static void SaveSolarValues(IWeConnectDbContext dbContext, DateTime time, string vin, int mileage, int batterySoc, List<(DateTime, double)> length, List<(DateTime, double)> duration, List<(DateTime, double)> averageSpeed, List<(DateTime, double)> consumedKwh, List<(DateTime, double)> averageConsumption, double climateTemp, bool chargingState, bool climateState, bool windowMeltState, bool remoteHeatingState)
        {
            var day = time.Date;

            var dbData = dbContext.WeConnectDataSet.Where(x => x.Key == day && x.Vin == vin).FirstOrDefault();
            if (dbData == null)
                dbContext.WeConnectDataSet.Add(dbData = new WeConnectMidresData() { Key = day, Vin = vin });

            var oldMileage = dbData.Mileage;
            var series1 = dbData.DrivenKilometersSeries;
            series1.Accumulate(time, oldMileage.HasValue ? mileage - oldMileage.Value : 0);
            dbData.SetSeries(0, series1);
            dbData.Mileage = mileage;

            var series2 = dbData.BatterySocSeries;
            series2[time] = batterySoc;
            dbData.SetSeries(1, series2);

            var series3 = dbData.TripLengthKmSeries;
            foreach ((var timestamp, var value) in length)
                series3[timestamp.ToUniversalTime()] = value;
            dbData.SetSeries(2, series3);

            var series4 = dbData.TripDurationSeries;
            foreach ((var timestamp, var value) in duration)
                series4[timestamp.ToUniversalTime()] = value;
            dbData.SetSeries(3, series4);

            var series5 = dbData.TripAverageSpeedSeries;
            foreach ((var timestamp, var value) in averageSpeed)
                series5[timestamp.ToUniversalTime()] = value;
            dbData.SetSeries(4, series5);

            var series6 = dbData.TripConsumedKwhSeries;
            foreach ((var timestamp, var value) in consumedKwh)
                series6[timestamp.ToUniversalTime()] = value;
            dbData.SetSeries(5, series6);

            var series7 = dbData.TripAverageConsumptionSeries;
            foreach ((var timestamp, var value) in averageConsumption)
                series7[timestamp.ToUniversalTime()] = value;
            dbData.SetSeries(6, series7);

            var series8 = dbData.ChargingStateSeries;
            series8[time] = chargingState;
            dbData.SetSeries(7, series8);

            var series9 = dbData.ClimateTempSeries;
            series9[time] = climateTemp;
            dbData.SetSeries(8, series9);

            var series10 = dbData.ClimateStateSeries;
            series10[time] = climateState;
            dbData.SetSeries(9, series10);

            var series11 = dbData.WindowMeltStateSeries;
            series11[time] = windowMeltState;
            dbData.SetSeries(10, series11);

            var series12 = dbData.RemoteHeatingStateSeries;
            series12[time] = remoteHeatingState;
            dbData.SetSeries(11, series12);

            SaveLowresSolarValues(dbContext, day, vin, series1, series2, series3, series4, series5, series6, series7, series8, series9, series10, series11, series12);

            dbContext.SaveChanges();
        }

        private static void SaveLowresSolarValues(IWeConnectDbContext dbContext, DateTime day, string vin, TimeSeries<int> series1Src, TimeSeries<int> series2Src, TimeSeries<double> series3Src, TimeSeries<double> series4Src, TimeSeries<double> series5Src, TimeSeries<double> series6Src, TimeSeries<double> series7Src, TimeSeries<bool> series8Src, TimeSeries<double> series9Src, TimeSeries<bool> series10Src, TimeSeries<bool> series11Src, TimeSeries<bool> series12Src)
        {
            static DateTime FirstOfMonth(DateTime date) => date.AddDays(-1 * (date.Day - 1));
            var month = FirstOfMonth(day);
            var dbSolarSeries = dbContext.WeConnectLowresDataSet.Where(x => x.Key == month && x.Vin == vin).FirstOrDefault();
            if (dbSolarSeries == null)
                dbContext.WeConnectLowresDataSet.Add(dbSolarSeries = new WeConnectLowresData() { Key = month, Vin = vin });

            // Hack: remove first 5 elements due to bug in day-boundaries
            static ITimeSeries<int> PreprocessMileage(ITimeSeries<int> input)
            {
                for (int i = 0; i < 5; i++)
                    input[i] = input[i].HasValue ? (int?)0 : null;
                return input;
            }

            var series1 = dbSolarSeries.DrivenKilometersSeries;
            var resampler1 = new TimeSeriesResampler<TimeSeries<int>, int>(series1.Span) { Resampled = series1 };
            resampler1.SampleAggregate(PreprocessMileage(series1Src), x => x.Sum());
            dbSolarSeries.SetSeries(0, resampler1.Resampled);

            var series2 = dbSolarSeries.BatterySocSeries;
            var resampler2 = new TimeSeriesResampler<TimeSeries<int>, int>(series2.Span) { Resampled = series2 };
            resampler2.SampleAggregate(series2Src, x => (int)x.Average());
            dbSolarSeries.SetSeries(1, resampler2.Resampled);

            var series3 = dbSolarSeries.TripLengthKmSeries;
            var resampler3 = new TimeSeriesResampler<TimeSeries<double>, double>(series3.Span) { Resampled = series3 };
            resampler3.SampleAggregate(series3Src, x => x.Average());
            dbSolarSeries.SetSeries(4, resampler3.Resampled);

            var series4 = dbSolarSeries.TripDurationSeries;
            var resampler4 = new TimeSeriesResampler<TimeSeries<double>, double>(series4.Span) { Resampled = series4 };
            resampler4.SampleAggregate(series4Src, x => x.Average());
            dbSolarSeries.SetSeries(4, resampler4.Resampled);

            var series5 = dbSolarSeries.TripAverageSpeedSeries;
            var resampler5 = new TimeSeriesResampler<TimeSeries<double>, double>(series5.Span) { Resampled = series5 };
            resampler5.SampleAggregate(series5Src, x => x.Average());
            dbSolarSeries.SetSeries(4, resampler5.Resampled);

            var series6 = dbSolarSeries.TripConsumedKwhSeries;
            var resampler6 = new TimeSeriesResampler<TimeSeries<double>, double>(series6.Span) { Resampled = series6 };
            resampler6.SampleAggregate(series6Src, x => x.Average());
            dbSolarSeries.SetSeries(2, resampler6.Resampled);

            var series7 = dbSolarSeries.TripAverageConsumptionSeries;
            var resampler7 = new TimeSeriesResampler<TimeSeries<double>, double>(series7.Span) { Resampled = series7 };
            resampler7.SampleAggregate(series7Src, x => x.Average());
            dbSolarSeries.SetSeries(3, resampler7.Resampled);

            var series8 = dbSolarSeries.ChargingStateSeries;
            var resampler8 = new TimeSeriesResampler<TimeSeries<bool>, bool>(series8.Span) { Resampled = series8 };
            resampler8.SampleAggregate(series8Src, x => x.Any(v => v));
            dbSolarSeries.SetSeries(4, resampler8.Resampled);

            var series9 = dbSolarSeries.ClimateTempSeries;
            var resampler9 = new TimeSeriesResampler<TimeSeries<double>, double>(series9.Span) { Resampled = series9 };
            resampler9.SampleAggregate(series9Src, x => x.Average());
            dbSolarSeries.SetSeries(4, resampler9.Resampled);

            var series10 = dbSolarSeries.ClimateStateSeries;
            var resampler10 = new TimeSeriesResampler<TimeSeries<bool>, bool>(series10.Span) { Resampled = series10 };
            resampler10.SampleAggregate(series10Src, x => x.Any(v => v));
            dbSolarSeries.SetSeries(4, resampler10.Resampled);

            var series11 = dbSolarSeries.WindowMeltStateSeries;
            var resampler11 = new TimeSeriesResampler<TimeSeries<bool>, bool>(series11.Span) { Resampled = series11 };
            resampler11.SampleAggregate(series11Src, x => x.Any(v => v));
            dbSolarSeries.SetSeries(4, resampler11.Resampled);

            var series12 = dbSolarSeries.RemoteHeatingStateSeries;
            var resampler12 = new TimeSeriesResampler<TimeSeries<bool>, bool>(series12.Span) { Resampled = series12 };
            resampler12.SampleAggregate(series12Src, x => x.Any(v => v));
            dbSolarSeries.SetSeries(4, resampler12.Resampled);
        }
    }
}