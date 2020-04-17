using RichardSzalay.MockHttp;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace PhilipDaubmeier.NetatmoClient.Tests
{
    public class NetatmoWebClientErrorResponseTest
    {
        /// <summary>
        /// Tests all error codes and messages that are documented in https://dev.netatmo.com/apidocumentation/general
        /// </summary>
        [Theory]
        [InlineData(400, 1, "Access token is missing", false)]
        [InlineData(400, 21, "Invalid argument", false)]
        [InlineData(400, 10, "Missing arguments", false)]
        [InlineData(400, 25, "Invalid date", false)]
        [InlineData(400, 37, "No more space available on the camera", false)]
        [InlineData(401, 2, "Invalid token missing", false)]
        [InlineData(401, 3, "Access token expired", false)]
        [InlineData(403, -2, "Unkown error in Oauth", false)]
        [InlineData(403, -1, "Grant is invalid", false)]
        [InlineData(403, 1, "Access token missing", false)]
        [InlineData(403, 2, "Invalid token missing", false)]
        [InlineData(403, 3, "Access token expired", false)]
        [InlineData(403, 1, "Internal error", false)]
        [InlineData(403, 13, "Operation forbidden", false)]
        [InlineData(403, 26, "Maximum usage reached", true)]
        [InlineData(403, 30, "Invalid refresh token", false)]
        [InlineData(403, 35, "Unable to execute", false)]
        [InlineData(403, 36, "Prohibited string", false)]
        [InlineData(403, 41, "Device is unreachable", false)]
        [InlineData(404, 9, "Device not found", false)]
        [InlineData(404, 19, "IP not found", false)]
        [InlineData(404, 22, "Application not found", false)]
        [InlineData(404, 23, "User not found", false)]
        [InlineData(404, 31, "(Method) Not found", false)]
        [InlineData(406, 5, "Application deactivated", false)]
        [InlineData(406, 7, "Nothing to modify", false)]
        [InlineData(500, 40, "JSON given has an invalid encoding", false)]
        public async Task TestGenericErrorResponse(int httpStatusCode, int errorCode, string errorMessage, bool specialExceptionMessage)
        {
            var mockHandler = new MockHttpMessageHandler().AddAuthMock();
            mockHandler.When($"{MockNetatmoConnection.BaseUri}/api/getstationsdata")
                .Respond((HttpStatusCode)httpStatusCode, "application/json",
                @"{
                      ""error"": {
                          ""code"": " + errorCode + @",
                          ""message"": """ + errorMessage + @"""
                      }
                }");

            using var netatmoClient = new NetatmoWebClient(mockHandler.ToMockProvider());

            await Assert.ThrowsAsync<IOException>(async () => await netatmoClient.GetWeatherStationData());

            if (specialExceptionMessage)
                return;

            try
            {
                await netatmoClient.GetWeatherStationData();
            }
            catch (IOException ex)
            {
                Assert.Contains(httpStatusCode.ToString(), ex.Message);
                Assert.Contains(errorCode.ToString(), ex.Message);
                Assert.Contains(errorMessage, ex.Message);
            }
        }

        [Fact]
        public async Task TestMissingPayload()
        {
            var mockHandler = new MockHttpMessageHandler().AddAuthMock();
            mockHandler.When($"{MockNetatmoConnection.BaseUri}/api/getstationsdata")
                .Respond("application/json",
                @"{
                    ""status"": ""ok"",
                    ""time_exec"": 0.03,
                    ""time_server"": 1585755824
                }");

            using var netatmoClient = new NetatmoWebClient(mockHandler.ToMockProvider());

            await Assert.ThrowsAsync<IOException>(async () => await netatmoClient.GetWeatherStationData());

            try
            {
                await netatmoClient.GetWeatherStationData();
            }
            catch (IOException ex)
            {
                Assert.Contains("missing a payload", ex.Message);
            }
        }

        [Fact]
        public async Task TestMalformedJson()
        {
            var mockHandler = new MockHttpMessageHandler().AddAuthMock();
            mockHandler.When($"{MockNetatmoConnection.BaseUri}/api/getstationsdata")
                .Respond("application/json",
                @"{
                    ""maformed_json"": ""missing closing bracket""
                ");

            using var netatmoClient = new NetatmoWebClient(mockHandler.ToMockProvider());

            await Assert.ThrowsAsync<IOException>(async () => await netatmoClient.GetWeatherStationData());

            try
            {
                await netatmoClient.GetWeatherStationData();
            }
            catch (IOException ex)
            {
                Assert.Contains("could not be deserialized", ex.Message);
            }
        }
    }
}
