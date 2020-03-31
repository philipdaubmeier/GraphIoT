using PhilipDaubmeier.NetatmoClient.Model.Core;
using RichardSzalay.MockHttp;
using System;
using System.Collections.Generic;
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
                                    ""modules"":
                                    [
                                        {
                                            ""_id"": ""98:76:54:32:1a:bc"",
                                            ""type"": ""NAModule1"",
                                            ""module_name"": ""My outdoor module"",
                                            ""data_type"":
                                            [
                                                ""Temperature"",
                                                ""Humidity""
                                            ],
                                            ""last_setup"": 1450976705,
                                            ""battery_percent"": 74,
                                            ""reachable"": true,
                                            ""firmware"": 44,
                                            ""last_message"": 1585722586,
                                            ""last_seen"": 1585722547,
                                            ""rf_status"": 57,
                                            ""battery_vp"": 5376,
                                            ""dashboard_data"":
                                            {
                                                ""time_utc"": 1585722547,
                                                ""Temperature"": 0,
                                                ""Humidity"": 59,
                                                ""min_temp"": -1.4,
                                                ""max_temp"": 2.4,
                                                ""date_max_temp"": 1585692196,
                                                ""date_min_temp"": 1585717163,
                                                ""temp_trend"": ""up""
                                            }
                                        }
                                    ]
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

            Assert.Equal("12:34:56:78:9a:bc", result.Devices[0].Id);
            Assert.Equal("My Netatmo", result.Devices[0].StationName);
            Assert.Equal("MyMainModule", result.Devices[0].ModuleName);
            Assert.Equal("NAMain", result.Devices[0].Type);
            Assert.Equal(137, result.Devices[0].Firmware);
            Assert.Equal(62, result.Devices[0].WifiStatus);
            Assert.False(result.Devices[0].Co2Calibrating);
            Assert.Equal(new List<Measure>() { "Temperature", "CO2", "Humidity", "Noise", "Pressure" }, result.Devices[0].DataType);

            Assert.Equal(321, result.Devices[0].Place.Altitude);
            Assert.Equal("Unittest Town", result.Devices[0].Place.City);
            Assert.Equal("EN", result.Devices[0].Place.Country);
            Assert.Equal("America/New_York", result.Devices[0].Place.Timezone);
            Assert.Equal(37.377166, result.Devices[0].Place.Location[0]);
            Assert.Equal(-122.086966, result.Devices[0].Place.Location[1]);

            Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1585688522), result.Devices[0].DashboardData.TimeUtc);
            Assert.Equal(20.7, result.Devices[0].DashboardData.Temperature);
            Assert.Equal(1407, result.Devices[0].DashboardData.CO2);
            Assert.Equal(59, result.Devices[0].DashboardData.Humidity);
            Assert.Equal(35, result.Devices[0].DashboardData.Noise);
            Assert.Equal(1025, result.Devices[0].DashboardData.Pressure);
            Assert.Equal(980.7, result.Devices[0].DashboardData.AbsolutePressure);
            Assert.Equal(20, result.Devices[0].DashboardData.MinTemp);
            Assert.Equal(21.3, result.Devices[0].DashboardData.MaxTemp);
            Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1585650280), result.Devices[0].DashboardData.DateMaxTemp);
            Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1585618645), result.Devices[0].DashboardData.DateMinTemp);
            Assert.Equal("stable", result.Devices[0].DashboardData.TempTrend);

            Assert.Equal("98:76:54:32:1a:bc", result.Devices[0].Modules[0].Id);
            Assert.Equal("My outdoor module", result.Devices[0].Modules[0].ModuleName);
            Assert.Equal("NAModule1", result.Devices[0].Modules[0].Type);
            Assert.Equal(44, result.Devices[0].Modules[0].Firmware);
            Assert.Equal(57, result.Devices[0].Modules[0].RfStatus);
            Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1585722586), result.Devices[0].Modules[0].LastMessage);
            Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1585722547), result.Devices[0].Modules[0].LastSeen);
            Assert.Equal(5376, result.Devices[0].Modules[0].BatteryVp);
            Assert.Equal(74, result.Devices[0].Modules[0].BatteryPercent);
            Assert.Equal(new List<Measure>() { "Temperature", "Humidity" }, result.Devices[0].Modules[0].DataType);

            Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1585722547), result.Devices[0].Modules[0].DashboardData.TimeUtc);
            Assert.Equal(0, result.Devices[0].Modules[0].DashboardData.Temperature);
            Assert.Equal(59, result.Devices[0].Modules[0].DashboardData.Humidity);
            Assert.Equal(-1.4, result.Devices[0].Modules[0].DashboardData.MinTemp);
            Assert.Equal(2.4, result.Devices[0].Modules[0].DashboardData.MaxTemp);
            Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1585692196), result.Devices[0].Modules[0].DashboardData.DateMaxTemp);
            Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1585717163), result.Devices[0].Modules[0].DashboardData.DateMinTemp);
            Assert.Equal("up", result.Devices[0].Modules[0].DashboardData.TempTrend);
            Assert.Null(result.Devices[0].Modules[0].DashboardData.CO2);
            Assert.Null(result.Devices[0].Modules[0].DashboardData.Noise);
            Assert.Null(result.Devices[0].Modules[0].DashboardData.Pressure);
            Assert.Null(result.Devices[0].Modules[0].DashboardData.AbsolutePressure);
        }
    }
}
