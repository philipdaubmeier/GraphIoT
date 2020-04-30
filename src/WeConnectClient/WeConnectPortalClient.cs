using PhilipDaubmeier.WeConnectClient.Model.Carfinder;
using PhilipDaubmeier.WeConnectClient.Model.Emanager;
using PhilipDaubmeier.WeConnectClient.Model.TripStatistics;
using PhilipDaubmeier.WeConnectClient.Model.VehicleInfo;
using PhilipDaubmeier.WeConnectClient.Model.VehicleList;
using PhilipDaubmeier.WeConnectClient.Model.VehicleStatus;
using PhilipDaubmeier.WeConnectClient.Network;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhilipDaubmeier.WeConnectClient
{
    public class WeConnectPortalClient : WeConnectAuthBase
    {
        public WeConnectPortalClient(IWeConnectConnectionProvider connectionProvider)
            : base(connectionProvider) { }

        public async Task<Location> GetLastKnownLocation()
        {
            return await CallApi<LocationResponse, Location>("/-/cf/get-location");
        }

        public async Task<IEnumerable<VehicleEntry>> GetVehicleList()
        {
            return await CallApi<VehicleListResponse, IEnumerable<VehicleEntry>>("/-/mainnavigation/get-fully-loaded-cars");
        }

        public async Task<VehicleEntry> GetVehicle(string vin)
        {
            return await CallApi<LoadCarDetailsResponse, VehicleEntry>($"/-/mainnavigation/load-car-details/{vin}");
        }

        public async Task<VehicleDetails> GetVehicleDetails()
        {
            return await CallApi<VehicleDetailsResponse, VehicleDetails>("/-/vehicle-info/get-vehicle-details");
        }

        public async Task<VehicleStatus> GetVehicleStatus()
        {
            return await CallApi<VehicleStatusResponse, VehicleStatus>("/-/vsr/get-vsr");
        }

        public async Task<Emanager> GetEManager()
        {
            return await CallApi<EmanagerResponse, Emanager>("/-/emanager/get-emanager");
        }

        public async Task<Rts> GetLatestTripStatistics()
        {
            return await LoadTripStatistics("/-/rts/get-latest-trip-statistics");
        }

        public async Task<Rts> GetLastRefuelTripStatistics()
        {
            return await LoadTripStatistics("/-/rts/get-last-refuel-trip-statistics");
        }

        private async Task<Rts> LoadTripStatistics(string path)
        {
            var res = await CallApi<TripStatisticsResponse, Rts>(path);
            res.DateTimeLocale = GetCarNetLocale();
            return res;
        }
    }
}