using PhilipDaubmeier.SonnenClient.Model;
using PhilipDaubmeier.SonnenClient.Network;
using System;
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
            var uri = new Uri($"{_baseUri}sites/{siteId}/measurements?{query}");
            return await CallSonnenApi<MeasurementWiremessage<MeasurementSeries>, MeasurementSeries>(uri);
        }
    }
}