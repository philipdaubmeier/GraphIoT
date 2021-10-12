using PhilipDaubmeier.WeConnectClient.Model.ActionParams;
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

        public WeConnectPortalClient(IWeConnectConnectionProvider connectionProvider)
            : base(connectionProvider) { }

        public async Task<Location> GetLastKnownLocation(Vin? vin = null)
        {
            return await CallApi<LocationResponse, Location>(new Uri($"{_carUri}/{vin}/parkingposition"));
        }

        public async Task<IEnumerable<VehicleEntry>> GetVehicleList()
        {
            return await CallApi<VehicleListResponse, List<VehicleEntry>>(new Uri($"{_vumUri}/me/relations"), true);
        }

        public async Task<VehicleEntry> GetVehicle(Vin vin)
        {
            return await CallApi<LoadCarDetailsResponse, VehicleEntry>(new Uri($"{_gvfUri}vehicleData/de-DE/{vin}"), true);
        }

        public async Task<VehicleDetails> GetVehicleDetails(Vin? vin = null)
        {
            return await CallApi<VehicleDetailsResponse, VehicleDetails>(new Uri($"{_gvfUri}vehicleDetails/de-DE/{vin}"), true);
        }

        public async Task<VehicleStatus> GetVehicleStatus(Vin? vin = null)
        {
            return await CallApi<VehicleStatusResponse, VehicleStatus>(new Uri($"{_carUri}/{vin}/"));
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

        public async Task<Rts> GetLongtermTripStatistics(Vin? vin = null)
        {
            return await LoadTripStatistics(new Uri($"{_carUri}/{vin}/tripdata/longterm/last"));
        }

        public async Task<Rts> GetLatestTripStatistics(Vin? vin = null)
        {
            return await LoadTripStatistics(new Uri($"{_carUri}/{vin}/tripdata/shortterm/last"));
        }

        public async Task<Rts> GetLastRefuelTripStatistics(Vin? vin = null)
        {
            return await LoadTripStatistics(new Uri($"{_carUri}/{vin}/tripdata/cyclic/last"));
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

        private async Task<Rts> LoadTripStatistics(Uri uri)
        {
            return await CallApi<TripStatisticsResponse, Rts>(uri);
        }
    }
}