using Microsoft.Extensions.Logging;
using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.GraphIoT.Core.Database;
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

            static TimeSeries<T> ToTimeSeries<T>(DateTime time, IEnumerable<(DateTime, T)> source) where T : struct
            {
                var series = new TimeSeries<T>(new TimeSeriesSpan(time.Date, time.Date.AddDays(1), TimeSeriesSpan.Spacing.Spacing5Min));
                foreach ((var timestamp, var value) in source)
                    series[timestamp.ToUniversalTime()] = value;
                return series;
            }

            static bool ToBool(string? boolStr) => !string.IsNullOrWhiteSpace(boolStr) && !boolStr.Equals("OFF", StringComparison.InvariantCultureIgnoreCase);

            var batterySoc = status.BatteryLevel ?? emanager.Rbc.Status?.BatteryPercentage ?? 0;

            var length = ToTimeSeries(time, trips.AllEntries.Select(x => (x.start + x.duration, x.trip.TripLength)));
            var duration = ToTimeSeries(time, trips.AllEntries.Select(x => (x.start + x.duration, x.duration.TotalMinutes)));
            var consumedKwh = ToTimeSeries(time, trips.AllEntries.Select(x => (x.start + x.duration, (x.trip.AverageElectricConsumption ?? 0d) * x.trip.TripLength / 100)));
            var averageConsumption = ToTimeSeries(time, trips.AllEntries.Select(x => (x.start + x.duration, x.trip.AverageElectricConsumption ?? 0d)));
            var averageSpeed = ToTimeSeries(time, trips.AllEntries.Select(x => (x.start + x.duration, x.trip.AverageSpeed)));

            var climateTemp = double.Parse(emanager.Rpc.Settings?.TargetTemperature, CultureInfo.InvariantCulture);
            var chargingState = ToBool(emanager.Rbc.Status?.ChargingState);
            var climateState = ToBool(emanager.Rpc.Status?.ClimatisationState);
            var windowMeltState = ToBool(emanager.Rpc.Status?.WindowHeatingStateFront?.ToString());
            var remoteHeatingState = emanager.Rdt.AuxHeatingEnabled;

            SaveSolarValues(_dbContext, time, vin, mileage, batterySoc, length, duration, averageSpeed, consumedKwh, averageConsumption, climateTemp, chargingState, climateState, windowMeltState, remoteHeatingState);
        }

        public static void SaveSolarValues(IWeConnectDbContext dbContext, DateTime time, string vin, int mileage, int batterySoc, TimeSeries<double> length, TimeSeries<double> duration, TimeSeries<double> averageSpeed, TimeSeries<double> consumedKwh, TimeSeries<double> averageConsumption, double climateTemp, bool chargingState, bool climateState, bool windowMeltState, bool remoteHeatingState)
        {
            var dbData = TimeSeriesDbEntityBase.LoadOrCreateDay(dbContext.WeConnectDataSet, time.Date, x => x.Vin == vin);
            dbData.Vin = vin;

            var oldMileage = dbData.Mileage;
            var series1 = dbData.DrivenKilometersSeries;
            series1.Accumulate(time, oldMileage.HasValue ? mileage - oldMileage.Value : 0);
            dbData.SetSeries(0, series1);
            dbData.Mileage = mileage;

            dbData.SetSeriesValue(1, time, batterySoc);
            dbData.CopyInto(2, length);
            dbData.CopyInto(3, duration);
            dbData.CopyInto(4, averageSpeed);
            dbData.CopyInto(5, consumedKwh);
            dbData.CopyInto(6, averageConsumption);
            dbData.SetSeriesValue(7, time, chargingState);
            dbData.SetSeriesValue(8, time, climateTemp);
            dbData.SetSeriesValue(9, time, climateState);
            dbData.SetSeriesValue(10, time, windowMeltState);
            dbData.SetSeriesValue(11, time, remoteHeatingState);

            SaveLowresSolarValues(dbContext, time.Date, vin, dbData);

            dbContext.SaveChanges();
        }

        private static void SaveLowresSolarValues(IWeConnectDbContext dbContext, DateTime day, string vin, WeConnectMidresData midRes)
        {
            var dbData = TimeSeriesDbEntityBase.LoadOrCreateMonth(dbContext.WeConnectLowresDataSet, day, x => x.Vin == vin);
            dbData.Vin = vin;

            // Hack: remove first 5 elements due to bug in day-boundaries
            static ITimeSeries<int> PreprocessMileage(ITimeSeries<int> input)
            {
                for (int i = 0; i < 5; i++)
                    input[i] = input[i].HasValue ? (int?)0 : null;
                return input;
            }

            dbData.ResampleFrom<int>(midRes, 0, x => x.Sum(), PreprocessMileage);
            dbData.ResampleFromAll(midRes, 0);
        }
    }
}