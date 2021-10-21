using Microsoft.Extensions.Logging;
using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.GraphIoT.Core.Database;
using PhilipDaubmeier.GraphIoT.WeConnect.Database;
using PhilipDaubmeier.WeConnectClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                ex = ex.Demystify();
                _logger.LogInformation($"{DateTime.Now} Exception occurred in WeConnect background worker: {ex}");
            }
        }

        public async Task PollVehicleValues()
        {
            var vin = (await _weConnectClient.GetVehicleList()).FirstOrDefault()?.Vin;
            if (vin is null)
                return;

            var fuelStatus = await _weConnectClient.GetFuelStatus(vin);
            var trip = await _weConnectClient.GetLastShorttermTrip(vin);
            var time = DateTime.UtcNow;

            static TimeSeries<T> ToTimeSeries<T>(DateTime time, DateTime timestamp, T value) where T : struct
            {
                var series = new TimeSeries<T>(new TimeSeriesSpan(time.Date, time.Date.AddDays(1), TimeSeriesSpan.Spacing.Spacing5Min));
                series[timestamp.ToUniversalTime()] = value;
                return series;
            }

            var mileage = (int)trip.OverallMileageKm;
            var batterySoc = (int)(fuelStatus.FirstOrDefault()?.CurrentSocPercent ?? 0d);

            var length = ToTimeSeries(time, trip.TripEndTimestamp, trip.MileageKm);
            var duration = ToTimeSeries(time, trip.TripEndTimestamp, trip.TravelTime);
            var consumedKwh = ToTimeSeries(time, trip.TripEndTimestamp, (trip.AverageElectricConsumption ?? 0d) * trip.MileageKm / 100);
            var averageConsumption = ToTimeSeries(time, trip.TripEndTimestamp, trip.AverageElectricConsumption ?? 0d);
            var averageSpeed = ToTimeSeries(time, trip.TripEndTimestamp, trip.AverageSpeedKmph);

            // fixed values, as they are no longer accessible in new api
            var climateTemp = 20d;
            var chargingState = false;
            var climateState = false;
            var windowMeltState = false;
            var remoteHeatingState = false;

            SaveVehicleValues(_dbContext, time, vin, mileage, batterySoc, length, duration, averageSpeed, consumedKwh, averageConsumption, climateTemp, chargingState, climateState, windowMeltState, remoteHeatingState);
        }

        public static void SaveVehicleValues(IWeConnectDbContext dbContext, DateTime time, string vin, int mileage, int batterySoc, TimeSeries<double> length, TimeSeries<double> duration, TimeSeries<double> averageSpeed, TimeSeries<double> consumedKwh, TimeSeries<double> averageConsumption, double climateTemp, bool chargingState, bool climateState, bool windowMeltState, bool remoteHeatingState)
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

            SaveLowresVehicleValues(dbContext, time.Date, vin, dbData);

            dbContext.SaveChanges();
        }

        private static void SaveLowresVehicleValues(IWeConnectDbContext dbContext, DateTime day, string vin, WeConnectMidresData midRes)
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