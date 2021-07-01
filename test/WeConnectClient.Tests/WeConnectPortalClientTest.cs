using PhilipDaubmeier.WeConnectClient.Model.Core;
using PhilipDaubmeier.WeConnectClient.Network;
using RichardSzalay.MockHttp;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PhilipDaubmeier.WeConnectClient.Tests
{
    public class WeConnectPortalClientTest
    {
        private const string ExpectedSessionToken = "{\"u\":\"https://www.portal.volkswagen-we.com/portal/delegate/dashboard/WVWZZZABCD1234567\"," +
                "\"csrf\":\"agkWdVBw\",\"c\":[{\"n\":\"CARNET_LANGUAGE_ID\",\"v\":\"en_GB\",\"e\":\"2066-12-12T00:26:12+01:00\"}," +
                "{\"n\":\"JSESSIONID\",\"v\":\"ZTVmMz_unittest_session_id_2NTktY2MyYjAzN2I1NzAx\",\"e\":\"0001-01-01T00:00:00\"}]}";

        [Fact]
        public async Task TestAuthSerialization()
        {
            var auth = new WeConnectAuth("john@doe.com", "secretpassword");

            var client = new WeConnectPortalClient(new MockCookieHttpMessageHandler()
                .AddAuthMock()
                .AddEmanager()
                .ToMockProvider(auth));

            await client.GetEManager();

            Assert.Equal(ExpectedSessionToken, auth.AccessToken);
        }

        [Fact]
        public async Task TestAuthDeserialization()
        {
            // explicitly not providing any credentials
            var auth = new WeConnectAuth(string.Empty, string.Empty);

            // emulate that the session was persisted and is now restored.
            // this should give us access without a username and password
            await auth.UpdateTokenAsync(ExpectedSessionToken, DateTime.MinValue, null);

            var mockedHandler = new MockCookieHttpMessageHandler();
            var client = new WeConnectPortalClient(mockedHandler
                .AddAuthMock(out MockedRequest mockedRequest)
                .AddEmanager()
                .ToMockProvider(auth));

            var result = await client.GetEManager();

            // assert we could load data successfully
            Assert.Equal("AVAILABLE", result.Rbc.Status?.ExtPowerSupplyState);

            // assert that no new authentication was needed to be called
            Assert.Equal(0, mockedHandler.GetMatchCount(mockedRequest));
        }

        [Fact]
        public async Task TestGetEManager()
        {
            var client = new WeConnectPortalClient(new MockCookieHttpMessageHandler()
                .AddAuthMock()
                .AddEmanager()
                .ToMockProvider());

            var result = await client.GetEManager();

            Assert.Equal(100, result.Rbc.Status?.BatteryPercentage);
            Assert.Equal("OFF", result.Rbc.Status?.ChargingState);
            Assert.Equal("", result.Rbc.Status?.ChargingRemaningHour);
            Assert.Equal("", result.Rbc.Status?.ChargingRemaningMinute);
            Assert.Equal("INVALID", result.Rbc.Status?.ChargingReason);
            Assert.Equal("CONNECTED", result.Rbc.Status?.PluginState);
            Assert.Equal("LOCKED", result.Rbc.Status?.LockState);
            Assert.Equal("AVAILABLE", result.Rbc.Status?.ExtPowerSupplyState);
            Assert.Equal("7", result.Rbc.Status?.Range);
            Assert.Equal(265, result.Rbc.Status?.ElectricRange);
            Assert.Null(result.Rbc.Status?.CombustionRange);
            Assert.Equal(265, result.Rbc.Status?.CombinedRange);
            Assert.False(result.Rbc.Status?.RlzeUp);

            Assert.Equal(16, result.Rbc.Settings?.ChargerMaxCurrent);
            Assert.Equal(32, result.Rbc.Settings?.MaxAmpere);
            Assert.False(result.Rbc.Settings?.MaxCurrentReduced);

            Assert.Equal("OFF", result.Rpc.Status?.ClimatisationState);
            Assert.Equal(30, result.Rpc.Status?.ClimatisationRemaningTime);
            Assert.Null(result.Rpc.Status?.WindowHeatingStateFront);
            Assert.Null(result.Rpc.Status?.WindowHeatingStateRear);
            Assert.Null(result.Rpc.Status?.ClimatisationReason);
            Assert.True(result.Rpc.Status?.WindowHeatingAvailable);

            Assert.Equal("20.5", result.Rpc.Settings?.TargetTemperature);
            Assert.True(result.Rpc.Settings?.ClimatisationWithoutHVPower);
            Assert.True(result.Rpc.Settings?.Electric);
            Assert.Equal("AVAILABLE", result.Rpc.ClimaterActionState);
            Assert.False(result.Rpc.AuAvailable);

            Assert.Equal(1, result.Rdt.Status?.Timers[0].TimerId);
            Assert.Equal(1, result.Rdt.Status?.Timers[0].TimerProfileId);
            Assert.Equal("NOT_EXPIRED", result.Rdt.Status?.Timers[0].TimerStatus);
            Assert.Equal("IDLE", result.Rdt.Status?.Timers[0].TimerChargeScheduleStatus);
            Assert.Equal("IDLE", result.Rdt.Status?.Timers[0].TimerClimateScheduleStatus);
            Assert.Equal("28.04.2020", result.Rdt.Status?.Timers[0].TimerExpStatusTimestamp);
            Assert.Equal("NOT_PROGRAMMED", result.Rdt.Status?.Timers[0].TimerProgrammedStatus);
            Assert.Equal("28.04.2020", result.Rdt.Status?.Timers[0].StartDateActive);
            Assert.Equal("12:00", result.Rdt.Status?.Timers[0].TimeRangeActive);

            Assert.Equal(2, result.Rdt.Status?.Timers[0].Schedule.Type);
            Assert.Equal(12, result.Rdt.Status?.Timers[0].Schedule.Start.Hours);
            Assert.Equal(0, result.Rdt.Status?.Timers[0].Schedule.Start.Minutes);
            Assert.Null(result.Rdt.Status?.Timers[0].Schedule.End.Hours);
            Assert.Null(result.Rdt.Status?.Timers[0].Schedule.End.Minutes);
            Assert.Null(result.Rdt.Status?.Timers[0].Schedule.Index);
            Assert.Equal("Y", result.Rdt.Status?.Timers[0].Schedule.Daypicker[0]);
            Assert.Equal("28.04.2020", result.Rdt.Status?.Timers[0].Schedule.StartDateActive);
            Assert.Null(result.Rdt.Status?.Timers[0].Schedule.EndDateActive);

            Assert.Equal(1, result.Rdt.Status?.Profiles[0].ProfileId);
            Assert.Equal("Standard", result.Rdt.Status?.Profiles[0].ProfileName);
            Assert.Equal("28.04.2020", result.Rdt.Status?.Profiles[0].TimeStamp);
            Assert.True(result.Rdt.Status?.Profiles[0].Charging);
            Assert.False(result.Rdt.Status?.Profiles[0].Climatisation);
            Assert.Equal(100, result.Rdt.Status?.Profiles[0].TargetChargeLevel);
            Assert.False(result.Rdt.Status?.Profiles[0].NightRateActive);
            Assert.Equal("23:00", result.Rdt.Status?.Profiles[0].NightRateTimeStart);
            Assert.Equal("23:00", result.Rdt.Status?.Profiles[0].NightRateTimeEnd);
            Assert.Equal(16, result.Rdt.Status?.Profiles[0].ChargeMaxCurrent);
            Assert.Equal("ELECTRIC", result.Rdt.Status?.Profiles[0].HeaterSource);

            Assert.Equal(30, result.Rdt.Settings?.MinChargeLimit);
            Assert.Equal(100, result.Rdt.Settings?.LowerLimitMax);

            Assert.False(result.Rdt.AuxHeatingAllowed);
            Assert.False(result.Rdt.AuxHeatingEnabled);

            Assert.False(result.ActionPending);
            Assert.True(result.RdtAvailable);
        }

        [Fact]
        public async Task TestGetLatestTripStatistics()
        {
            var client = new WeConnectPortalClient(new MockCookieHttpMessageHandler()
                .AddAuthMock()
                .AddTripStatistics()
                .ToMockProvider());

            var result = await client.GetLatestTripStatistics();

            Assert.Equal(30, result.DaysInMonth);
            Assert.Equal(2, result.FirstWeekday);
            Assert.Equal(4, result.Month);
            Assert.Equal(2020, result.Year);
            Assert.Equal(2020, result.FirstTripYear);
            Assert.Null(result.CyclicData);
            Assert.False(result.TripFromLastRefuelAvailable);

            Assert.Null(result.TripStatistics[0]);
            Assert.Equal(30, result.TripStatistics.Count);
            Assert.Equal(123412344, result.TripStatistics[27].AggregatedStatistics.TripId);
            Assert.Equal(12.8, result.TripStatistics[27].AggregatedStatistics.AverageElectricConsumption);
            Assert.Null(result.TripStatistics[27].AggregatedStatistics.AverageFuelConsumption);
            Assert.Null(result.TripStatistics[27].AggregatedStatistics.AverageCngConsumption);
            Assert.Equal(50, result.TripStatistics[27].AggregatedStatistics.AverageSpeed);
            Assert.Equal(70, result.TripStatistics[27].AggregatedStatistics.TripDuration);
            Assert.Equal(58, result.TripStatistics[27].AggregatedStatistics.TripLength);
            Assert.Equal("28.04.2020", result.TripStatistics[27].AggregatedStatistics.Timestamp);
            Assert.Equal("1:10", result.TripStatistics[27].AggregatedStatistics.TripDurationFormatted);
            Assert.Null(result.TripStatistics[27].AggregatedStatistics.Recuperation);
            Assert.Null(result.TripStatistics[27].AggregatedStatistics.AverageAuxiliaryConsumption);
            Assert.Equal(12.8, result.TripStatistics[27].AggregatedStatistics.TotalElectricConsumption);
            Assert.Null(result.TripStatistics[27].AggregatedStatistics.LongFormattedTimestamp);

            Assert.Equal(123412344, result.TripStatistics[27].TripStatistics[0].TripId);
            Assert.Equal(12.8, result.TripStatistics[27].TripStatistics[0].AverageElectricConsumption);
            Assert.Null(result.TripStatistics[27].TripStatistics[0].AverageFuelConsumption);
            Assert.Null(result.TripStatistics[27].TripStatistics[0].AverageCngConsumption);
            Assert.Equal(50, result.TripStatistics[27].TripStatistics[0].AverageSpeed);
            Assert.Equal(70, result.TripStatistics[27].TripStatistics[0].TripDuration);
            Assert.Equal(58, result.TripStatistics[27].TripStatistics[0].TripLength);
            Assert.Equal("Today, 14:34", result.TripStatistics[27].TripStatistics[0].Timestamp);
            Assert.Equal("1:10", result.TripStatistics[27].TripStatistics[0].TripDurationFormatted);
            Assert.Null(result.TripStatistics[27].TripStatistics[0].Recuperation);
            Assert.Null(result.TripStatistics[27].TripStatistics[0].AverageAuxiliaryConsumption);
            Assert.Null(result.TripStatistics[27].TripStatistics[0].TotalElectricConsumption);
            Assert.Equal("Trip ended: Tue, 28.04.2020, 14:34", result.TripStatistics[27].TripStatistics[0].LongFormattedTimestamp);

            Assert.Equal(123412344, result.LongTermData.TripId);
            Assert.Equal(13.1, result.LongTermData.AverageElectricConsumption);
            Assert.Null(result.LongTermData.AverageFuelConsumption);
            Assert.Null(result.LongTermData.AverageCngConsumption);
            Assert.Equal(26, result.LongTermData.AverageSpeed);
            Assert.Equal(642, result.LongTermData.TripDuration);
            Assert.Equal(275, result.LongTermData.TripLength);
            Assert.Equal("Heute, 14:34", result.LongTermData.Timestamp);
            Assert.Equal("10:42", result.LongTermData.TripDurationFormatted);
            Assert.Null(result.LongTermData.Recuperation);
            Assert.Null(result.LongTermData.AverageAuxiliaryConsumption);
            Assert.Null(result.LongTermData.TotalElectricConsumption);
            Assert.Equal("Trip ended: Tue, 28.04.2020, 14:34", result.LongTermData.LongFormattedTimestamp);

            Assert.False(result.ServiceConfiguration.ElectricConsumption);
            Assert.True(result.ServiceConfiguration.TriptypeShort);
            Assert.False(result.ServiceConfiguration.AuxiliaryConsumption);
            Assert.False(result.ServiceConfiguration.FuelOverallConsumption);
            Assert.True(result.ServiceConfiguration.TriptypeCyclic);
            Assert.True(result.ServiceConfiguration.ElectricOverallConsumption);
            Assert.True(result.ServiceConfiguration.TriptypeLong);
            Assert.False(result.ServiceConfiguration.CngOverallConsumption);
            Assert.False(result.ServiceConfiguration.Recuperation);
        }

        [Fact]
        public async Task TestGetLastRefuelTripStatistics()
        {
            var client = new WeConnectPortalClient(new MockCookieHttpMessageHandler()
                .AddAuthMock()
                .AddTripStatistics()
                .ToMockProvider());

            var result = await client.GetLastRefuelTripStatistics();

            Assert.Equal(30, result.DaysInMonth);
            Assert.Equal(2, result.FirstWeekday);
            Assert.Equal(4, result.Month);
            Assert.Equal(2020, result.Year);
            Assert.Equal(2020, result.FirstTripYear);
            Assert.Null(result.TripStatistics);
            Assert.Null(result.LongTermData);
            Assert.Null(result.ServiceConfiguration);
            Assert.True(result.TripFromLastRefuelAvailable);

            Assert.Equal(123412344, result.CyclicData.TripId);
            Assert.Equal(12.8, result.CyclicData.AverageElectricConsumption);
            Assert.Null(result.CyclicData.AverageFuelConsumption);
            Assert.Null(result.CyclicData.AverageCngConsumption);
            Assert.Equal(50, result.CyclicData.AverageSpeed);
            Assert.Equal(70, result.CyclicData.TripDuration);
            Assert.Equal(58, result.CyclicData.TripLength);
            Assert.Equal("Today, 14:34", result.CyclicData.Timestamp);
            Assert.Equal("1:10", result.CyclicData.TripDurationFormatted);
            Assert.Null(result.CyclicData.Recuperation);
            Assert.Null(result.CyclicData.AverageAuxiliaryConsumption);
            Assert.Null(result.CyclicData.TotalElectricConsumption);
            Assert.Equal("Trip ended: Tue, 28.04.2020, 14:34", result.CyclicData.LongFormattedTimestamp);
        }

        [Fact]
        public async Task TestGetLatestTripStatisticsAllEntries()
        {
            var client = new WeConnectPortalClient(new MockCookieHttpMessageHandler()
                .AddAuthMock()
                .AddTripStatistics()
                .ToMockProvider());

            var result = await client.GetLatestTripStatistics();

            var allEntries = result.AllEntries.ToList();
            Assert.Equal(new DateTime(2020, 04, 28, 13, 24, 0, DateTimeKind.Local), allEntries[0].start);
            Assert.Equal(new TimeSpan(1, 10, 0), allEntries[0].duration);
            Assert.Equal(50d, allEntries[0].trip.AverageSpeed);
        }

        [Fact]
        public async Task TestGetLastRefuelTripStatisticsAllEntries()
        {
            var client = new WeConnectPortalClient(new MockCookieHttpMessageHandler()
                .AddAuthMock()
                .AddTripStatistics()
                .ToMockProvider());

            var result = await client.GetLastRefuelTripStatistics();

            var allEntries = result.AllEntries.ToList();
            Assert.Equal(new DateTime(2020, 04, 28, 13, 24, 0, DateTimeKind.Local), allEntries[0].start);
            Assert.Equal(new TimeSpan(1, 10, 0), allEntries[0].duration);
            Assert.Equal(50d, allEntries[0].trip.AverageSpeed);
        }

        [Fact]
        public async Task TestGetVehicleDetails()
        {
            var client = new WeConnectPortalClient(new MockCookieHttpMessageHandler()
                .AddAuthMock()
                .AddVehicleDetails()
                .ToMockProvider());

            var result = await client.GetVehicleDetails();

            Assert.Equal("28-04-2020", result.LastConnectionTimeStamp[0]);
            Assert.Equal("14:39", result.LastConnectionTimeStamp[1]);

            Assert.Equal(new DateTime(2020, 04, 28, 12, 39, 0, DateTimeKind.Utc), result.LastConnection);

            Assert.Equal("64.803", result.DistanceCovered);
            Assert.Equal("41", result.Range);
            Assert.Equal("225 Day(s) / 25.400 km", result.ServiceInspectionData);
            Assert.Equal(string.Empty, result.OilInspectionData);
            Assert.False(result.ShowOil);
            Assert.True(result.ShowService);
            Assert.False(result.FlightMode);
        }

        [Fact]
        public async Task TestGetVehicleStatus()
        {
            var client = new WeConnectPortalClient(new MockCookieHttpMessageHandler()
                .AddAuthMock()
                .AddVehicleStatus()
                .ToMockProvider());

            var result = await client.GetVehicleStatus();

            Assert.True(result.WindowStatusSupported);
            Assert.Equal(2, result.CarRenderData.ParkingLights);
            Assert.Equal(3, result.CarRenderData.Hood);
            Assert.Equal(3, result.CarRenderData.Sunroof);

            Assert.Equal(3, result.CarRenderData.Doors.LeftFront);
            Assert.Equal(3, result.CarRenderData.Doors.RightFront);
            Assert.Equal(3, result.CarRenderData.Doors.LeftBack);
            Assert.Equal(3, result.CarRenderData.Doors.RightBack);
            Assert.Equal(3, result.CarRenderData.Doors.Trunk);
            Assert.Equal(4, result.CarRenderData.Doors.NumberOfDoors);

            Assert.Equal(3, result.CarRenderData.Windows.LeftFront);
            Assert.Equal(3, result.CarRenderData.Windows.RightFront);
            Assert.Equal(3, result.CarRenderData.Windows.LeftBack);
            Assert.Equal(3, result.CarRenderData.Windows.RightBack);

            Assert.Equal(2, result.LockData.LeftFront);
            Assert.Equal(2, result.LockData.RightFront);
            Assert.Equal(2, result.LockData.LeftBack);
            Assert.Equal(2, result.LockData.RightBack);
            Assert.Equal(2, result.LockData.Trunk);

            Assert.False(result.LockDisabled);
            Assert.False(result.UnlockDisabled);
            Assert.True(result.RluDisabled);
            Assert.False(result.HideCngFuelLevel);
            Assert.Equal(41, result.TotalRange);
            Assert.Equal(41, result.PrimaryEngineRange);
            Assert.Null(result.FuelRange);
            Assert.Null(result.CngRange);
            Assert.Equal(41, result.BatteryRange);
            Assert.Null(result.FuelLevel);
            Assert.Null(result.CngFuelLevel);
            Assert.Equal(100, result.BatteryLevel);
            Assert.Equal("https://www.portal.volkswagen-we.com/static/slices/phev_golf/phev_golf", result.SliceRootPath);
        }

        [Fact]
        public async Task TestGetLocation()
        {
            var client = new WeConnectPortalClient(new MockCookieHttpMessageHandler()
                .AddAuthMock()
                .AddCarfinder()
                .ToMockProvider());

            var result = await client.GetLastKnownLocation();

            Assert.Equal(37.377166, result.Latitude);
            Assert.Equal(-122.086966, result.Longitude);
        }

        [Fact]
        public async Task TestGetLocationDifferentVins()
        {
            var client = new WeConnectPortalClient(new MockCookieHttpMessageHandler()
                .AddAuthMock()
                .AddCarfinder()
                .ToMockProvider());

            var resultVin1 = await client.GetLastKnownLocation(new Vin("WVWZZZABCD1234567"));
            var resultVin2 = await client.GetLastKnownLocation(new Vin("TESTVHCLE22222222"));

            Assert.Equal(37.377166, resultVin1.Latitude);
            Assert.Equal(-122.086966, resultVin1.Longitude);

            Assert.Equal(52.433921, resultVin2.Latitude);
            Assert.Equal(10.7957444, resultVin2.Longitude);
        }

        [Fact]
        public async Task TestGetVehicleList()
        {
            var client = new WeConnectPortalClient(new MockCookieHttpMessageHandler()
                .AddAuthMock()
                .AddVehicleList()
                .ToMockProvider());

            var result = (await client.GetVehicleList()).ToList();

            Assert.Single(result);

            Assert.Equal("WVWZZZABCD1234567", result[0].Vin);
            Assert.Equal("My Car", result[0].Name);
            Assert.False(result[0].Expired);
            Assert.Null(result[0].Model);
            Assert.Null(result[0].ModelCode);
            Assert.Null(result[0].ModelYear);
            Assert.Null(result[0].ImageUrl);
            Assert.Null(result[0].VehicleSpecificFallbackImageUrl);
            Assert.Null(result[0].ModelSpecificFallbackImageUrl);
            Assert.Equal("/portal/delegate/vehicle-image/WVWZZZABCD1234567", result[0].DefaultImageUrl);
            Assert.Equal("v", result[0].VehicleBrand);
            Assert.Equal("20200101", result[0].EnrollmentDate);
            Assert.Null(result[0].DeviceOCU1);
            Assert.Null(result[0].DeviceOCU2);
            Assert.Null(result[0].DeviceMIB);
            Assert.False(result[0].EngineTypeCombustian);
            Assert.True(result[0].EngineTypeHybridOCU1);
            Assert.False(result[0].EngineTypeHybridOCU2);
            Assert.False(result[0].EngineTypeElectric);
            Assert.False(result[0].EngineTypeCNG);
            Assert.False(result[0].EngineTypeDefault);
            Assert.Equal("UNAVAILABLE", result[0].StpStatus);
            Assert.True(result[0].WindowstateSupported);
            Assert.Equal("/portal/user/12341234-1234-1234-1234-123412341234/abcdefg_unit_test_xprofile_id_12", result[0].DashboardUrl);
            Assert.False(result[0].VhrRequested);
            Assert.False(result[0].VsrRequested);
            Assert.False(result[0].VhrConfigAvailable);
            Assert.False(result[0].VerifiedByDealer);
            Assert.False(result[0].Vhr2);
            Assert.True(result[0].RoleEnabled);
            Assert.True(result[0].IsEL2Vehicle);
            Assert.False(result[0].WorkshopMode);
            Assert.False(result[0].HiddenUserProfiles);
            Assert.Null(result[0].MobileKeyActivated);
            Assert.Equal("MILEAGE", result[0].EnrollmentType);
            Assert.False(result[0].Ocu3Low);

            Assert.Equal("NET.500.010.F", result[0].PackageServices[0].PackageServiceId);
            Assert.Equal("NET.500.010.1", result[0].PackageServices[0].PropertyKeyReference);
            Assert.Equal("e-Remote", result[0].PackageServices[0].PackageServiceName);
            Assert.Equal("e-Remote", result[0].PackageServices[0].TrackingName);
            Assert.Equal("01-01-2020", result[0].PackageServices[0].ActivationDate);
            Assert.Equal("01-01-2021", result[0].PackageServices[0].ExpirationDate);
            Assert.False(result[0].PackageServices[0].Expired);
            Assert.False(result[0].PackageServices[0].ExpireInAMonth);
            Assert.Equal("er", result[0].PackageServices[0].PackageType);
            Assert.Equal("er", result[0].PackageServices[0].EnrollmentPackageType);

            Assert.True(result[0].DefaultCar);
            Assert.False(result[0].VwConnectPowerLayerAvailable);
            Assert.Equal("abcdefg_unit_test_xprofile_id_12", result[0].XprofileId);
            Assert.Null(result[0].SmartCardKeyActivated);
            Assert.True(result[0].FullyEnrolled);
            Assert.False(result[0].SecondaryUser);
            Assert.False(result[0].Fleet);
            Assert.False(result[0].Touareg);
            Assert.False(result[0].IceSupported);
            Assert.False(result[0].FlightMode);
            Assert.False(result[0].EsimCompatible);
            Assert.False(result[0].Dkyenabled);
            Assert.True(result[0].Selected);
        }

        [Fact]
        public async Task TestGetVehicle()
        {
            var client = new WeConnectPortalClient(new MockCookieHttpMessageHandler()
                .AddAuthMock()
                .AddLoadCarDetails("WVWZZZ0THER777777")
                .ToMockProvider());

            var result = await client.GetVehicle("WVWZZZ0THER777777");

            Assert.Equal("WVWZZZ0THER777777", result.Vin);
            Assert.Equal("My Car", result.Name);
            Assert.False(result.Expired);
            Assert.Null(result.Model);
            Assert.Null(result.ModelCode);
            Assert.Null(result.ModelYear);
            Assert.Null(result.ImageUrl);
            Assert.Null(result.VehicleSpecificFallbackImageUrl);
            Assert.Null(result.ModelSpecificFallbackImageUrl);
            Assert.Equal("/portal/delegate/vehicle-image/WVWZZZ0THER777777", result.DefaultImageUrl);
            Assert.Equal("v", result.VehicleBrand);
            Assert.Equal("20200101", result.EnrollmentDate);
            Assert.Null(result.DeviceOCU1);
            Assert.Null(result.DeviceOCU2);
            Assert.Null(result.DeviceMIB);
            Assert.False(result.EngineTypeCombustian);
            Assert.True(result.EngineTypeHybridOCU1);
            Assert.False(result.EngineTypeHybridOCU2);
            Assert.False(result.EngineTypeElectric);
            Assert.False(result.EngineTypeCNG);
            Assert.False(result.EngineTypeDefault);
            Assert.Equal("UNAVAILABLE", result.StpStatus);
            Assert.True(result.WindowstateSupported);
            Assert.Equal("/portal/user/12341234-1234-1234-1234-123412341234/abcdefg_unit_test_xprofile_id_12", result.DashboardUrl);
            Assert.False(result.VhrRequested);
            Assert.False(result.VsrRequested);
            Assert.False(result.VhrConfigAvailable);
            Assert.False(result.VerifiedByDealer);
            Assert.False(result.Vhr2);
            Assert.True(result.RoleEnabled);
            Assert.True(result.IsEL2Vehicle);
            Assert.False(result.WorkshopMode);
            Assert.False(result.HiddenUserProfiles);
            Assert.Null(result.MobileKeyActivated);
            Assert.Equal("MILEAGE", result.EnrollmentType);
            Assert.False(result.Ocu3Low);

            Assert.Equal("NET.500.010.F", result.PackageServices[0].PackageServiceId);
            Assert.Equal("NET.500.010.1", result.PackageServices[0].PropertyKeyReference);
            Assert.Equal("e-Remote", result.PackageServices[0].PackageServiceName);
            Assert.Equal("e-Remote", result.PackageServices[0].TrackingName);
            Assert.Equal("01-01-2020", result.PackageServices[0].ActivationDate);
            Assert.Equal("01-01-2021", result.PackageServices[0].ExpirationDate);
            Assert.False(result.PackageServices[0].Expired);
            Assert.False(result.PackageServices[0].ExpireInAMonth);
            Assert.Equal("er", result.PackageServices[0].PackageType);
            Assert.Equal("er", result.PackageServices[0].EnrollmentPackageType);

            Assert.True(result.DefaultCar);
            Assert.False(result.VwConnectPowerLayerAvailable);
            Assert.Equal("abcdefg_unit_test_xprofile_id_12", result.XprofileId);
            Assert.Null(result.SmartCardKeyActivated);
            Assert.True(result.FullyEnrolled);
            Assert.False(result.SecondaryUser);
            Assert.False(result.Fleet);
            Assert.False(result.Touareg);
            Assert.False(result.IceSupported);
            Assert.False(result.FlightMode);
            Assert.False(result.EsimCompatible);
            Assert.False(result.Dkyenabled);
            Assert.True(result.Selected);
        }

        [Fact]
        public async Task TestGetGeofences()
        {
            var client = new WeConnectPortalClient(new MockCookieHttpMessageHandler()
                .AddAuthMock()
                .AddGeofence()
                .ToMockProvider());

            var result = await client.GetGeofences();

            Assert.Single(result.GeoFenceList);

            Assert.False(result.GeoFenceList[0].Active);
            Assert.Equal("123456", result.GeoFenceList[0].Id);
            Assert.Equal("1607-1601 Miramonte Ave, Mountain View, CA 94040, USA", result.GeoFenceList[0].DefinitionName);
            Assert.Equal("RED", result.GeoFenceList[0].ZoneType);
            Assert.Equal("ELLIPSE", result.GeoFenceList[0].ShapeType);
            Assert.Equal(37.377166, result.GeoFenceList[0].Latitude);
            Assert.Equal(-122.086966, result.GeoFenceList[0].Longitude);
            Assert.Equal(0, result.GeoFenceList[0].RotationAngle);
            Assert.Equal(0, result.GeoFenceList[0].RectHeight);
            Assert.Equal(0, result.GeoFenceList[0].RectWidth);
            Assert.Equal(375, result.GeoFenceList[0].EllipseFirstRadius);
            Assert.Equal(375, result.GeoFenceList[0].EllipseSecondRadius);
            Assert.False(result.GeoFenceList[0].Updated);

            Assert.Equal(2, result.GeoFenceList[0].Schedule.Type);
            Assert.Equal(15, result.GeoFenceList[0].Schedule.Start.Hours);
            Assert.Equal(0, result.GeoFenceList[0].Schedule.Start.Minutes);
            Assert.Equal(20, result.GeoFenceList[0].Schedule.End.Hours);
            Assert.Equal(0, result.GeoFenceList[0].Schedule.End.Minutes);
            Assert.Null(result.GeoFenceList[0].Schedule.Index);
            Assert.Equal("Y", result.GeoFenceList[0].Schedule.Daypicker[0]);
            Assert.Equal("28.04.2020", result.GeoFenceList[0].Schedule.StartDateActive);
            Assert.Equal("28.04.2020", result.GeoFenceList[0].Schedule.EndDateActive);

            Assert.Equal("28.04.2020", result.GeoFenceList[0].StartDateActive);
            Assert.Equal("15:00 - 20:00", result.GeoFenceList[0].TimeRangeActive);
            Assert.Equal(10, result.MaxNumberOfGeos);
            Assert.Equal(4, result.MaxNumberOfActiveGeos);
            Assert.Equal("ACKNOWLEDGED", result.Status);
            Assert.Equal("/portal/web/de/content/-/content/legal/carnet-terms-and-conditions", result.TermAndConditionsURL);
        }

        [Fact]
        public async Task TestGetLatestHealthReports()
        {
            var client = new WeConnectPortalClient(new MockCookieHttpMessageHandler()
                .AddAuthMock()
                .AddHealthReport()
                .ToMockProvider());

            var result = (await client.GetLatestHealthReports()).ToList();

            Assert.Single(result);

            Assert.Equal("23.04.2020", result[0].CreationDate);
            Assert.Equal("21:24", result[0].CreationTime);
            Assert.Empty(result[0].DiagnosticMessages);
            Assert.True(result[0].HasValidData);
            Assert.Equal("123456", result[0].ReportId);
            Assert.False(result[0].IsOlderSixMonths);
            Assert.Equal("150", result[0].MileageValue);
            Assert.Equal(1587669881536, result[0].Timestamp);

            Assert.Equal("", result[0].HeaderData.RangeMileage);
            Assert.Equal("23.04.2020", result[0].HeaderData.LastRefreshTime[0]);
            Assert.Equal("21:24", result[0].HeaderData.LastRefreshTime[1]);
            Assert.False(result[0].HeaderData.ServiceOverdue);
            Assert.False(result[0].HeaderData.OilOverdue);
            Assert.Equal("89", result[0].HeaderData.Mileage);
            Assert.False(result[0].HeaderData.ShowOil);
            Assert.Null(result[0].HeaderData.VhrNoDataText);
            Assert.True(result[0].HeaderData.ShowService);

            Assert.Equal("ACKNOWLEDGED", result[0].JobStatus);
            Assert.False(result[0].FetchFailed);
        }
    }
}