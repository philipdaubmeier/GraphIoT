using Microsoft.Extensions.Logging;
using PhilipDaubmeier.GraphIoT.Core.Database;
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
            var dbChargerSeries = TimeSeriesDbEntityBase.LoadOrCreateDay(dbContext.SonnenChargerDataSet, time.Date);

            var oldChargedEnergyTotal = dbChargerSeries.ChargedEnergyTotal;
            var series1 = dbChargerSeries.ChargedEnergySeries;
            series1.Accumulate(time, oldChargedEnergyTotal.HasValue ? Math.Round(chargedEnergyTotal, 1) - Math.Round(oldChargedEnergyTotal.Value, 1) : 0);
            dbChargerSeries.SetSeries(0, series1);
            dbChargerSeries.ChargedEnergyTotal = Math.Round(chargedEnergyTotal, 1);

            dbChargerSeries.SetSeriesValue(1, time, activePower);
            dbChargerSeries.SetSeriesValue(2, time, current);
            dbChargerSeries.SetSeriesValue(3, time, connected);
            dbChargerSeries.SetSeriesValue(4, time, charging);
            dbChargerSeries.SetSeriesValue(5, time, smartMode);

            SaveLowresChargerValues(dbContext, time.Date, dbChargerSeries);

            dbContext.SaveChanges();
        }

        private static void SaveLowresChargerValues(ISonnenDbContext dbContext, DateTime day, SonnenChargerMidresData midRes)
        {
            TimeSeriesDbEntityBase.LoadOrCreateMonth(dbContext.SonnenChargerLowresDataSet, day).ResampleFromAll(midRes);
        }
    }
}