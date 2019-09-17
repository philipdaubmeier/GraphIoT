using PhilipDaubmeier.SonnenClient.Model;
using PhilipDaubmeier.SonnenClient.Network;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhilipDaubmeier.SonnenClient
{
    public class SonnenPortalClient : SonnenAuthBase
    {
        private const string _baseUri = "https://my-api.sonnen.de/v1/";

        public SonnenPortalClient(ISonnenConnectionProvider connectionProvider)
            : base(connectionProvider) { }

        /// <summary>
        /// Returns the user profile data and all site ids related to the currently signed in user.
        /// </summary>
        public async Task<UserSites> GetUserSites()
        {
            return (await CallSonnenApi<UserWiremessage, UserSites>(new Uri($"{_baseUri}users/me")));
        }

        /// <summary>
        /// Gets the measurement series for the given site in the given time interval.
        /// </summary>
        public async Task<MeasurementSeries> GetEnergyMeasurements(string siteId, DateTime start, DateTime end)
        {
            var query = $"?filter[start]={start.ToFilterTime()}&filter[end]={end.ToFilterTime()}&";
            var uri = new Uri($"{_baseUri}sites/{siteId}/measurements{query}");
            return await CallSonnenApi<Wiremessage<MeasurementSeries>, MeasurementSeries>(uri);
        }

        /// <summary>
        /// Get details about all battery systems of the given site.
        /// </summary>
        public async Task<List<BatterySystem>> GetBatterySystems(string siteId, int page = 1, int pageLimit = 10)
        {
            var query = $"?page[limit]={pageLimit}&page[number]={page}";
            var uri = new Uri($"{_baseUri}sites/{siteId}/battery-systems{query}");
            return await CallSonnenApi<ListWiremessage<BatterySystem>, List<BatterySystem>>(uri);
        }

        /// <summary>
        /// Get statistics (i.e. aggregated measurement series) with the given resolution and time interval.
        /// </summary>
        public async Task<StatisticsSeries> GetStatistics(string siteId, DateTime start, DateTime end, string resolution = "1-hour")
        {
            var query = $"?filter[start]={start.ToFilterTime()}&filter[end]={end.ToFilterTime()}&filter[resolution]={resolution}&";
            var uri = new Uri($"{_baseUri}sites/{siteId}/statistics{query}");
            return await CallSonnenApi<Wiremessage<StatisticsSeries>, StatisticsSeries>(uri);
        }
    }
}