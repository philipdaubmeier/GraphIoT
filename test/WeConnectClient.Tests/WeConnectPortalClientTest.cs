using PhilipDaubmeier.WeConnectClient.Network;
using RichardSzalay.MockHttp;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PhilipDaubmeier.WeConnectClient.Tests
{
    public class WeConnectPortalClientTest
    {
        public static string expectedSessionToken = "{\"u\":\"https://www.portal.volkswagen-we.com/portal/delegate/dashboard/WVWZZZABCD1234567\"," +
                "\"csrf\":\"agkWdVBw\",\"c\":[{\"n\":\"COOKIE_SUPPORT\",\"v\":\"true\",\"e\":\"2021-04-28T22:44:04+02:00\"}," +
                "{\"n\":\"JSESSIONID\",\"v\":\"ZTVmMz_unittest_session_id_2NTktY2MyYjAzN2I1NzAx\",\"e\":\"0001-01-01T00:00:00\"}," +
                "{\"n\":\"GUEST_LANGUAGE_ID\",\"v\":\"en_GB\",\"e\":\"2021-04-28T22:44:04+02:00\"}," +
                "{\"n\":\"CARNET_LANGUAGE_ID\",\"v\":\"en_GB\",\"e\":\"2066-12-12T00:26:12+01:00\"}]}";

        [Fact]
        public async Task TestAuthSerialization()
        {
            var auth = new WeConnectAuth("john@doe.com", "secretpassword");

            var client = new WeConnectPortalClient(new MockCookieHttpMessageHandler()
                .AddAuthMock()
                .AddEmanager()
                .ToMockProvider(auth));

            await client.GetEManager();

            Assert.Equal(expectedSessionToken, auth.AccessToken);
        }

        [Fact]
        public async Task TestAuthDeserialization()
        {
            // explicitly not providing any credentials
            var auth = new WeConnectAuth(string.Empty, string.Empty);

            // emulate that the session was persisted and is now restored.
            // this should give us access without a username and password
            await auth.UpdateTokenAsync(expectedSessionToken, DateTime.MinValue, null);

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
    }
}