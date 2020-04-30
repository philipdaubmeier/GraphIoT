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
        public WeConnectPortalClient(IWeConnectConnectionProvider connectionProvider)
            : base(connectionProvider) { }

        public async Task<Location> GetLastKnownLocation(Vin? vin = null)
        {
            return await CallApi<LocationResponse, Location>("/-/cf/get-location", vin);
        }

        public async Task<IEnumerable<VehicleEntry>> GetVehicleList(Vin? vin = null)
        {
            return await CallApi<VehicleListResponse, IEnumerable<VehicleEntry>>("/-/mainnavigation/get-fully-loaded-cars", vin);
        }

        public async Task<VehicleEntry> GetVehicle(Vin vin)
        {
            return await CallApi<LoadCarDetailsResponse, VehicleEntry>($"/-/mainnavigation/load-car-details/{vin}", vin);
        }

        public async Task<VehicleDetails> GetVehicleDetails(Vin? vin = null)
        {
            return await CallApi<VehicleDetailsResponse, VehicleDetails>("/-/vehicle-info/get-vehicle-details", vin);
        }

        public async Task<VehicleStatus> GetVehicleStatus(Vin? vin = null)
        {
            return await CallApi<VehicleStatusResponse, VehicleStatus>("/-/vsr/get-vsr", vin);
        }

        public async Task<IEnumerable<HealthReport>> GetLatestHealthReports(Vin? vin = null)
        {
            return await CallApi<HealthReportResponse, List<HealthReport>>("/-/vhr/get-latest-report", vin);
        }

        public async Task<GeofenceCollection> GetGeofences(Vin? vin = null)
        {
            return await CallApi<GeofenceResponse, GeofenceCollection>("/-/geofence/get-fences", vin);
        }

        public async Task<Emanager> GetEManager(Vin? vin = null)
        {
            return await CallApi<EmanagerResponse, Emanager>("/-/emanager/get-emanager", vin);
        }

        public async Task<Rts> GetLatestTripStatistics(Vin? vin = null)
        {
            return await LoadTripStatistics("/-/rts/get-latest-trip-statistics", vin);
        }

        public async Task<Rts> GetLastRefuelTripStatistics(Vin? vin = null)
        {
            return await LoadTripStatistics("/-/rts/get-last-refuel-trip-statistics", vin);
        }

        public async Task StartCharge(Vin? vin = null)
        {
            await RequestApi("/-/emanager/charge-battery", vin, new ChargeParams(true, 100));
        }

        public async Task StopCharge(Vin? vin = null)
        {
            await RequestApi("/-/emanager/charge-battery", vin, new ChargeParams(false, 99));
        }

        public async Task StartClimate(Vin? vin = null)
        {
            await RequestApi("/-/emanager/trigger-climatisation", vin, new ClimateParams(true, true));
        }

        public async Task StopClimate(Vin? vin = null)
        {
            await RequestApi("/-/emanager/trigger-climatisation", vin, new ClimateParams(false, true));
        }

        public async Task StartWindowMelt(Vin? vin = null)
        {
            await RequestApi("/-/emanager/trigger-windowheating", vin, new WindowsMeltParams(true));
        }

        public async Task StopWindowMelt(Vin? vin = null)
        {
            await RequestApi("/-/emanager/trigger-windowheating", vin, new WindowsMeltParams(true));
        }

        /// <summary>
        /// Logs out the session on the portal server, clears all state data of this client
        /// and removes the persisted state token in the IWeConnectAuth object.
        /// </summary>
        public async Task Logout()
        {
            await RequestApi("/-/logout/revoke");
            await _connectionProvider.AuthData.UpdateTokenAsync(null, DateTime.MinValue, null);
            _state.Reset();
        }

        private async Task<Rts> LoadTripStatistics(string path, Vin? vin = null)
        {
            var res = await CallApi<TripStatisticsResponse, Rts>(path, vin);
            res.DateTimeLocale = GetCarNetLocale();
            return res;
        }
    }
}