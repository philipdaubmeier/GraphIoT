using PhilipDaubmeier.WeConnectClient.Model.Auth;
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
        [Fact]
        public async Task TestAuthSerialization()
        {
            var auth = new WeConnectAuth("john@doe.com", "secretpassword");

            var client = new WeConnectPortalClient(new MockCookieHttpMessageHandler()
                .AddAuthMock()
                .AddFuelStatus()
                .ToMockProvider(new MockCookieHttpMessageHandler(false).AddAuthMock().AddFuelStatus(), auth));

            await client.GetFuelStatus(MockWeConnectConnection.Vin);

            Assert.Equal(MockWeConnectConnection.AccessToken, auth.AccessToken);
        }

        [Fact]
        public async Task TestAuthDeserialization()
        {
            // explicitly not providing any credentials
            var auth = new WeConnectAuth(string.Empty, string.Empty);

            // emulate that the token was persisted and is now restored.
            // this should give us access without a username and password
            var expiry = new ReadableJwt(MockWeConnectConnection.AccessToken).Expiration;
            await auth.UpdateTokenAsync(MockWeConnectConnection.AccessToken, expiry, null);

            var mockedHandler = new MockCookieHttpMessageHandler();
            var client = new WeConnectPortalClient(mockedHandler
                .AddAuthMock(out MockedRequest mockedRequest)
                .AddFuelStatus()
                .ToMockProvider(new MockCookieHttpMessageHandler(false).AddAuthMock().AddFuelStatus(), auth));

            var result = await client.GetFuelStatus(MockWeConnectConnection.Vin);

            // assert we could load data successfully
            Assert.Equal(98d, result.First().CurrentSocPercent);

            // assert that no new authentication was needed to be called
            Assert.Equal(0, mockedHandler.GetMatchCount(mockedRequest));
        }

        [Fact]
        public async Task TestGetFuelStatus()
        {
            var client = new WeConnectPortalClient(new MockCookieHttpMessageHandler()
                .AddAuthMock()
                .AddFuelStatus()
                .ToMockProvider(new MockCookieHttpMessageHandler(false).AddAuthMock().AddFuelStatus()));

            var result = await client.GetFuelStatus(MockWeConnectConnection.Vin);

            Assert.Equal(new DateTime(2021, 01, 01, 12, 0, 0, DateTimeKind.Utc), result.First().CarCapturedTimestamp);
            Assert.Equal("primaryEngine", result.First().Id);
            Assert.Equal("electric", result.First().EngineType);
            Assert.Equal(300d, result.First().RemainingRangeKm);
            Assert.Equal(98d, result.First().CurrentSocPercent);
            Assert.Null(result.First().CurrentFuelLevelPercent);
        }
    }
}