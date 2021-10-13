using PhilipDaubmeier.WeConnectClient.Model;
using PhilipDaubmeier.WeConnectClient.Model.ActionParams;
using PhilipDaubmeier.WeConnectClient.Model.Capabilities;
using PhilipDaubmeier.WeConnectClient.Model.Carfinder;
using PhilipDaubmeier.WeConnectClient.Model.Core;
using PhilipDaubmeier.WeConnectClient.Model.Emanager;
using PhilipDaubmeier.WeConnectClient.Model.Geofence;
using PhilipDaubmeier.WeConnectClient.Model.HealthReport;
using PhilipDaubmeier.WeConnectClient.Model.TripStatistics;
using PhilipDaubmeier.WeConnectClient.Model.VehicleInfo;
using PhilipDaubmeier.WeConnectClient.Model.VehicleList;
using PhilipDaubmeier.WeConnectClient.Model.VehicleStatus;
using PhilipDaubmeier.WeConnectClient.Network;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhilipDaubmeier.WeConnectClient
{
    public class WeConnectPortalClient : WeConnectAuthBase
    {
        private const string _gvfUri = "https://myvw-gvf-proxy.apps.emea.vwapps.io/";
        private const string _vumUri = "https://vum.apps.emea.vwapps.io/v2/users";
        private const string _carUri = "https://cardata.apps.emea.vwapps.io/vehicles";
        private const string _vdbsUri = "https://vdbs.apps.emea.vwapps.io/v1/vehicles";

        public WeConnectPortalClient(IWeConnectConnectionProvider connectionProvider)
            : base(connectionProvider) { }

        public async Task<Location> GetLastKnownLocation(Vin vin)
        {
            return await CallApi<LocationResponse, Location>(new Uri($"{_carUri}/{vin}/parkingposition"));
        }

        public async Task<IEnumerable<VehicleEntry>> GetVehicleList()
        {
            return await CallApi<VehicleListResponse, List<VehicleEntry>>(new Uri($"{_vumUri}/me/relations"), true);
        }

        public async Task<CapabilityList> GetVehicleCapabilities(Vin vin)
        {
            return await CallApi<CapabilitiesResponse, CapabilityList>(new Uri($"{_vdbsUri}/{vin}/users/{_state.UserId}/capabilities"));
        }

        public async Task<VehicleData> GetVehicleData(Vin vin)
        {
            return await CallApi<VehicleDataResponse, VehicleData>(new Uri($"{_gvfUri}vehicleData/de-DE/{vin}"), true);
        }

        public async Task<VehicleDetails> GetVehicleDetails(Vin vin)
        {
            return await CallApi<VehicleDetailsResponse, VehicleDetails>(new Uri($"{_gvfUri}vehicleDetails/de-DE/{vin}"), true);
        }

        public async Task<VehicleStatus> GetAccessStatus(Vin vin)
        {
            return await CallApi<VehicleStatusResponse, VehicleStatus>(new Uri($"{_carUri}/{vin}/charging/settings"));
        }

        public async Task<VehicleStatus> GetVehicleStatus(Vin? vin = null)
        {
            return await CallApi<VehicleStatusResponse, VehicleStatus>(new Uri($"{_carUri}/{vin}/states"));
        }

        public async Task<IEnumerable<HealthReport>> GetLatestHealthReports(Vin? vin = null)
        {
            return await CallApi<HealthReportResponse, List<HealthReport>>(new Uri($"{_carUri}/{vin}/"));
        }

        public async Task<GeofenceCollection> GetGeofences(Vin? vin = null)
        {
            return await CallApi<GeofenceResponse, GeofenceCollection>(new Uri($"{_carUri}/{vin}/"));
        }

        public async Task<Emanager> GetEManager(Vin? vin = null)
        {
            return await CallApi<EmanagerResponse, Emanager>(new Uri($"{_carUri}/{vin}/"));
        }

        public async Task<IEnumerable<TripStatisticEntry>> GetLongtermTripStatistics(Vin vin)
        {
            return await LoadTripStatistics<TripStatisticsListResponse, List<TripStatisticEntry>>(vin, "longterm", false);
        }

        public async Task<IEnumerable<TripStatisticEntry>> GetShorttermTripStatistics(Vin vin)
        {
            return await LoadTripStatistics<TripStatisticsListResponse, List<TripStatisticEntry>>(vin, "shortterm", false);
        }

        public async Task<IEnumerable<TripStatisticEntry>> GetCyclicTripStatistics(Vin vin)
        {
            return await LoadTripStatistics<TripStatisticsListResponse, List<TripStatisticEntry>>(vin, "cyclic", false);
        }

        public async Task<TripStatisticEntry> GetLastLongtermTrip(Vin vin)
        {
            return await LoadTripStatistics<TripStatisticsSingleResponse, TripStatisticEntry>(vin, "longterm", true);
        }

        public async Task<TripStatisticEntry> GetLastShorttermTrip(Vin vin)
        {
            return await LoadTripStatistics<TripStatisticsSingleResponse, TripStatisticEntry>(vin, "shortterm", true);
        }

        public async Task<TripStatisticEntry> GetLastCyclicTrip(Vin vin)
        {
            return await LoadTripStatistics<TripStatisticsSingleResponse, TripStatisticEntry>(vin, "cyclic", true);
        }

        public async Task StartCharge(Vin? vin = null)
        {
            await RequestApi(new Uri("/-/emanager/charge-battery"), false, new ChargeParams(true, 100));
        }

        public async Task StopCharge(Vin? vin = null)
        {
            await RequestApi(new Uri("/-/emanager/charge-battery"), false, new ChargeParams(false, 99));
        }

        public async Task StartClimate(Vin? vin = null)
        {
            await RequestApi(new Uri("/-/emanager/trigger-climatisation"), false, new ClimateParams(true, true));
        }

        public async Task StopClimate(Vin? vin = null)
        {
            await RequestApi(new Uri("/-/emanager/trigger-climatisation"), false, new ClimateParams(false, true));
        }

        public async Task StartWindowMelt(Vin? vin = null)
        {
            await RequestApi(new Uri("/-/emanager/trigger-windowheating"), false, new WindowsMeltParams(true));
        }

        public async Task StopWindowMelt(Vin? vin = null)
        {
            await RequestApi(new Uri("/-/emanager/trigger-windowheating"), false, new WindowsMeltParams(true));
        }

        /// <summary>
        /// Clears the persisted token in the IWeConnectAuth object.
        /// </summary>
        public async Task Logout()
        {
            await _connectionProvider.AuthData.UpdateTokenAsync(null, DateTime.MinValue, null);
        }

        private async Task<TData> LoadTripStatistics<TWiremessage, TData>(Vin vin, string path, bool onlyLast)
            where TWiremessage : class, IWiremessage<TData> where TData : class
        {
            var last = onlyLast ? "/last" : string.Empty;
            var uri = new Uri($"{_carUri}/{vin}/tripdata/{path}{last}");
            return await CallApi<TWiremessage, TData>(uri);
        }
    }
}