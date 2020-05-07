using Microsoft.Extensions.Logging;
using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.GraphIoT.Sonnen.Database;
using PhilipDaubmeier.SonnenClient;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PhilipDaubmeier.GraphIoT.Sonnen.Polling
{
    public class SonnenChargerPollingService : ISonnenPollingService
    {
        private readonly ILogger _logger;
        private readonly ISonnenDbContext _dbContext;
        private readonly SonnenPortalClient _sonnenClient;

        private string? _chargerId = null;

        public SonnenChargerPollingService(ILogger<SonnenChargerPollingService> logger, ISonnenDbContext databaseContext, SonnenPortalClient sonnenClient)
        {
            _logger = logger;
            _dbContext = databaseContext;
            _sonnenClient = sonnenClient;
        }

        public async Task PollValues()
        {
            _logger.LogInformation($"{DateTime.Now} SonnenCharger Background Service is polling new values...");

            try
            {
                await PollChargerValues();
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"{DateTime.Now} Exception occurred in SonnenCharger background worker: {ex.Message}");
            }
        }

        public async Task PollChargerValues()
        {
            if (string.IsNullOrEmpty(_chargerId))
            {
                var siteId = (await _sonnenClient.GetUserSites()).DefaultSiteId;
                _chargerId = (await _sonnenClient.GetSiteChargers(siteId)).FirstOrDefault().Id;
                if (string.IsNullOrEmpty(_chargerId))
                    return;
            }

            var chargerState = await _sonnenClient.GetChargerLiveState(_chargerId);
            var time = chargerState.MeasuredAt?.ToUniversalTime() ?? DateTime.UtcNow;

            var chargedEnergyTotal = chargerState.TotalChargedEnergy;
            var activePower = chargerState.ActivePower;
            var current = chargerState.Current;
            var connected = chargerState.Car.Equals("CONNECTED", StringComparison.InvariantCultureIgnoreCase);
            var charging = chargerState.Status.Equals("CHARGING", StringComparison.InvariantCultureIgnoreCase);
            var smartMode = chargerState.Smartmode;

            SaveChargerValues(_dbContext, time, chargedEnergyTotal, activePower, current, connected, charging, smartMode);
        }

        public static void SaveChargerValues(ISonnenDbContext dbContext, DateTime time, double chargedEnergyTotal, double activePower, double current, bool connected, bool charging, bool smartMode)
        {
            var day = time.Date;

            var dbChargerSeries = dbContext.SonnenChargerDataSet.Where(x => x.Key == day).FirstOrDefault();
            if (dbChargerSeries == null)
                dbContext.SonnenChargerDataSet.Add(dbChargerSeries = new SonnenChargerMidresData() { Key = day });

            var oldChargedEnergyTotal = dbChargerSeries.ChargedEnergyTotal;
            var series1 = dbChargerSeries.ChargedEnergySeries;
            series1.Accumulate(time, oldChargedEnergyTotal.HasValue ? Math.Round(chargedEnergyTotal, 1) - Math.Round(oldChargedEnergyTotal.Value, 1) : 0);
            dbChargerSeries.SetSeries(0, series1);
            dbChargerSeries.ChargedEnergyTotal = Math.Round(chargedEnergyTotal, 1);

            var series2 = dbChargerSeries.ActivePowerSeries;
            series2[time] = activePower;
            dbChargerSeries.SetSeries(1, series2);

            var series3 = dbChargerSeries.CurrentSeries;
            series3[time] = current;
            dbChargerSeries.SetSeries(2, series3);

            var series4 = dbChargerSeries.ConnectedSeries;
            series4[time] = connected;
            dbChargerSeries.SetSeries(3, series4);

            var series5 = dbChargerSeries.ChargingSeries;
            series5[time] = charging;
            dbChargerSeries.SetSeries(4, series5);

            var series6 = dbChargerSeries.SmartModeSeries;
            series6[time] = smartMode;
            dbChargerSeries.SetSeries(5, series6);

            SaveLowresChargerValues(dbContext, day, series1, series2, series3, series4, series5, series6);

            dbContext.SaveChanges();
        }

        private static void SaveLowresChargerValues(ISonnenDbContext dbContext, DateTime day, TimeSeries<double> series1Src, TimeSeries<double> series2Src, TimeSeries<double> series3Src, TimeSeries<bool> series4Src, TimeSeries<bool> series5Src, TimeSeries<bool> series6Src)
        {
            static DateTime FirstOfMonth(DateTime date) => date.AddDays(-1 * (date.Day - 1));
            var month = FirstOfMonth(day);
            var dbChargerSeries = dbContext.SonnenChargerLowresDataSet.Where(x => x.Key == month).FirstOrDefault();
            if (dbChargerSeries == null)
                dbContext.SonnenChargerLowresDataSet.Add(dbChargerSeries = new SonnenChargerLowresData() { Key = month });

            var series1 = dbChargerSeries.ChargedEnergySeries;
            var resampler1 = new TimeSeriesResampler<TimeSeries<double>, double>(series1.Span) { Resampled = series1 };
            resampler1.SampleAggregate(series1Src, x => x.Average());
            dbChargerSeries.SetSeries(0, resampler1.Resampled);

            var series2 = dbChargerSeries.ActivePowerSeries;
            var resampler2 = new TimeSeriesResampler<TimeSeries<double>, double>(series2.Span) { Resampled = series2 };
            resampler2.SampleAggregate(series2Src, x => x.Average());
            dbChargerSeries.SetSeries(1, resampler2.Resampled);

            var series3 = dbChargerSeries.CurrentSeries;
            var resampler3 = new TimeSeriesResampler<TimeSeries<double>, double>(series3.Span) { Resampled = series3 };
            resampler3.SampleAggregate(series3Src, x => x.Average());
            dbChargerSeries.SetSeries(2, resampler3.Resampled);

            var series4 = dbChargerSeries.ConnectedSeries;
            var resampler4 = new TimeSeriesResampler<TimeSeries<bool>, bool>(series4.Span) { Resampled = series4 };
            resampler4.SampleAggregate(series4Src, x => x.Any(v => v));
            dbChargerSeries.SetSeries(3, resampler4.Resampled);

            var series5 = dbChargerSeries.ChargingSeries;
            var resampler5 = new TimeSeriesResampler<TimeSeries<bool>, bool>(series5.Span) { Resampled = series5 };
            resampler5.SampleAggregate(series5Src, x => x.Any(v => v));
            dbChargerSeries.SetSeries(4, resampler5.Resampled);

            var series6 = dbChargerSeries.SmartModeSeries;
            var resampler6 = new TimeSeriesResampler<TimeSeries<bool>, bool>(series6.Span) { Resampled = series6 };
            resampler6.SampleAggregate(series6Src, x => x.Any(v => v));
            dbChargerSeries.SetSeries(5, resampler6.Resampled);
        }
    }
}