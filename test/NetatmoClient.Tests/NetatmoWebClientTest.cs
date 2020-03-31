using RichardSzalay.MockHttp;
using System.Threading.Tasks;
using Xunit;

namespace PhilipDaubmeier.NetatmoClient.Tests
{
    public class NetatmoWebClientTest
    {
        [Fact]
        public async Task TestGetWeatherStationData()
        {
            var mockHttp = new MockHttpMessageHandler();

            mockHttp.When($"{MockNetatmoConnection.BaseUri}/api/getstationsdata")
                    .Respond("application/json",
                    @"{
                        ""body"":
                        {
                            ""devices"":
                            [
                                {
                                    ""_id"": ""12:34:56:78:9a:bc"",
                                    ""station_name"": ""My Netatmo"",
                                    ""date_setup"": 1450976717,
                                    ""last_setup"": 1450976717,
                                    ""type"": ""NAMain"",
                                    ""last_status_store"": 1585688537,
                                    ""module_name"": ""MyMainModule"",
                                    ""firmware"": 137,
                                    ""wifi_status"": 62,
                                    ""reachable"": true,
                                    ""co2_calibrating"": false,
                                    ""data_type"":
                                    [
                                        ""Temperature"",
                                        ""CO2"",
                                        ""Humidity"",
                                        ""Noise"",
                                        ""Pressure""
                                    ],
                                    ""place"":
                                    {
                                        ""altitude"": 321,
                                        ""city"": ""Unittest Town"",
                                        ""country"": ""EN"",
                                        ""timezone"": ""America/New_York"",
                                        ""location"":
                                        [
                                            37.377166,
                                            -122.086966
                                        ]
                                    },
                                    ""dashboard_data"":
                                    {
                                        ""time_utc"": 1585688522,
                                        ""Temperature"": 20.7,
                                        ""CO2"": 1407,
                                        ""Humidity"": 59,
                                        ""Noise"": 35,
                                        ""Pressure"": 1025,
                                        ""AbsolutePressure"": 980.7,
                                        ""min_temp"": 20,
                                        ""max_temp"": 21.3,
                                        ""date_max_temp"": 1585650280,
                                        ""date_min_temp"": 1585618645,
                                        ""temp_trend"": ""stable"",
                                        ""pressure_trend"": ""stable""
                                    },
                                    ""modules"": [ ]
                                }
                            ],
                            ""user"":
                            {
                                ""mail"": ""john@doe.com"",
                                ""administrative"":
                                {
                                    ""country"": ""US"",
                                    ""reg_locale"": ""en-US"",
                                    ""lang"": ""en"",
                                    ""unit"": 0,
                                    ""windunit"": 0,
                                    ""pressureunit"": 0,
                                    ""feel_like_algo"": 0
                                }
                            }
                        },
                        ""status"": ""ok"",
                        ""time_exec"": 0.2,
                        ""time_server"": 1585688937
                    }");

            using var netatmoClient = new NetatmoWebClient(mockHttp.AddAuthMock().ToMockProvider());

            var result = await netatmoClient.GetWeatherStationData();

            Assert.Single(result.Devices);
        }
    }
}
