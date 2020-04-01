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
                                        },
                                        {
                                            ""_id"": ""bc:de:f9:87:65:43"",
                                            ""type"": ""NAModule2"",
                                            ""module_name"": ""Rain module"",
                                            ""data_type"":
                                            [
                                                ""Rain""
                                            ],
                                            ""last_setup"": 1547021355,
                                            ""battery_percent"": 75,
                                            ""reachable"": true,
                                            ""firmware"": 8,
                                            ""last_message"": 1585722586,
                                            ""last_seen"": 1585722586,
                                            ""rf_status"": 64,
                                            ""battery_vp"": 5452,
                                            ""dashboard_data"":
                                            {
                                                ""time_utc"": 1585722573,
                                                ""Rain"": 0,
                                                ""sum_rain_1"": 0,
                                                ""sum_rain_24"": 0
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
            Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1450976717), result.Devices[0].DateSetup);
            Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1450976717), result.Devices[0].LastSetup);
            Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1585688537), result.Devices[0].LastStatusStore);
            Assert.Equal(137, result.Devices[0].Firmware);
            Assert.Equal(62, result.Devices[0].WifiStatus);
            Assert.False(result.Devices[0].Co2Calibrating);
            Assert.Equal(new List<Measure>() { MeasureType.Temperature, MeasureType.CO2, MeasureType.Humidity, MeasureType.Noise, MeasureType.Pressure }, result.Devices[0].DataType);

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
            Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1450976705), result.Devices[0].Modules[0].LastSetup);
            Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1585722586), result.Devices[0].Modules[0].LastMessage);
            Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1585722547), result.Devices[0].Modules[0].LastSeen);
            Assert.Equal(5376, result.Devices[0].Modules[0].BatteryVp);
            Assert.Equal(74, result.Devices[0].Modules[0].BatteryPercent);
            Assert.Equal(new List<Measure>() { MeasureType.Temperature, MeasureType.Humidity }, result.Devices[0].Modules[0].DataType);

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

            Assert.Equal("bc:de:f9:87:65:43", result.Devices[0].Modules[1].Id);
            Assert.Equal("Rain module", result.Devices[0].Modules[1].ModuleName);
            Assert.Equal("NAModule2", result.Devices[0].Modules[1].Type);
            Assert.Equal(new List<Measure>() { MeasureType.Rain }, result.Devices[0].Modules[1].DataType);

            Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1585722573), result.Devices[0].Modules[1].DashboardData.TimeUtc);
            Assert.Equal(0, result.Devices[0].Modules[1].DashboardData.Rain);
            Assert.Equal(0, result.Devices[0].Modules[1].DashboardData.SumRain1);
            Assert.Equal(0, result.Devices[0].Modules[1].DashboardData.SumRain24);
            Assert.Null(result.Devices[0].Modules[1].DashboardData.Temperature);
            Assert.Null(result.Devices[0].Modules[1].DashboardData.CO2);
            Assert.Null(result.Devices[0].Modules[1].DashboardData.Noise);

            Assert.Equal("john@doe.com", result.User.Mail);
            Assert.Equal("US", result.User.Administrative.Country);
            Assert.Equal("en-US", result.User.Administrative.RegLocale);
            Assert.Equal("en", result.User.Administrative.Lang);
            Assert.Equal(0, result.User.Administrative.Unit);
            Assert.Equal(0, result.User.Administrative.Windunit);
            Assert.Equal(0, result.User.Administrative.Pressureunit);
            Assert.Equal(0, result.User.Administrative.FeelLikeAlgo);
        }

        [Fact]
        public async Task TestGetHomeData()
        {
            var mockHttp = new MockHttpMessageHandler();

            mockHttp.When($"{MockNetatmoConnection.BaseUri}/api/gethomedata")
                    .Respond("application/json",
                    @"{
                        ""body"": {
                            ""homes"": [
                                {
                                    ""id"": ""1234abcd1234abcd1234abcd"",
                                    ""name"": ""My Netatmo Home"",
                                    ""place"": {
                                        ""city"": ""Unittest Town"",
                                        ""country"": ""EN"",
                                        ""timezone"": ""America/New_York"",
                                    },
                                    ""cameras"": [
                                        {
                                            ""id"": ""78:90:ab:cd:ef:01"",
                                            ""type"": ""NOC"",
                                            ""status"": ""on"",
                                            ""vpn_url"": ""https://prodvpn-eu-123.netatmo.net/restricted/10.0.0.1/abcabc123412341234123412341234ab/MTABCDEFG1234ABCDEF1234BACDEF1234ABCDEF123,,"",
                                            ""is_local"": false,
                                            ""sd_status"": ""on"",
                                            ""alim_status"": ""on"",
                                            ""name"": ""My Presence"",
                                            ""last_setup"": 1490468965,
                                            ""light_mode_status"": ""auto""
                                        }
                                    ],
                                    ""smokedetectors"": [],
                                    ""events"": [
                                        {
                                            ""id"": ""1234abcd1234abcd1234abcd"",
                                            ""type"": ""outdoor"",
                                            ""time"": 1585576467,
                                            ""camera_id"": ""78:90:ab:cd:ef:01"",
                                            ""device_id"": ""78:90:ab:cd:ef:01"",
                                            ""video_id"": ""1234d30c-1234-5678-9012-abc35d7a4abc"",
                                            ""video_status"": ""available"",
                                            ""event_list"": [
                                                {
                                                    ""type"": ""vehicle"",
                                                    ""time"": 1585726685,
                                                    ""offset"": 0,
                                                    ""id"": ""1234d30c-1234-5678-9012-ff735af0923f"",
                                                    ""message"": ""Vehicle detected"",
                                                    ""snapshot"": {
                                                        ""id"": ""123e75a5980d8c6c79ef111a"",
                                                        ""version"": 1,
                                                        ""key"": ""123e75a5980d8c6c79ef111a977f2280e9823d3e5ba00adaa94edbe5ba00e5ba"",
                                                        ""url"": ""https://netatmocameraimage.blob.domain.com/path/b178de75e8444df3c9d762bd452bc2d625a44e75a5980d8c6c79ef111a977f2280e9823d3e5ba00adaa94edb""
                                                    },
                                                    ""vignette"": {
                                                        ""id"": ""234e75a5980d8c6c79ef111a"",
                                                        ""version"": 1,
                                                        ""key"": ""234e75a5980d8c6c79ef111a977f2280e9823d3e5ba00adaa94edbe5ba00e5ba"",
                                                        ""url"": ""https://netatmocameraimage.blob.domain.com/path/b178de75e8444df3c9d762bd452bc2d625a44e75a5980d8c6c79ef111a977f2280e9823d3e5ba00adaa94edb""
                                                    }
                                                },
                                                {
                                                    ""time"": 1585576470,
                                                    ""offset"": 1,
                                                    ""id"": ""1234d30c-1234-5678-9012-e5b34ccbd5b3"",
                                                    ""type"": ""human"",
                                                    ""message"": ""Person detected"",
                                                    ""snapshot"": {
                                                        ""filename"": ""vod/1234d30c-1234-5678-9012-abc35d7a4abc/events/1234d30c-1234-5678-9012-e5b34ccbd5b3/snapshot_1234d30c-1234-5678-9012-e5b34ccbd5b3.jpg""
                                                    },
                                                    ""vignette"": {
                                                        ""filename"": ""vod/1234d30c-1234-5678-9012-abc35d7a4abc/events/1234d30c-1234-5678-9012-e5b34ccbd5b3/vignette_1234d30c-1234-5678-9012-e5b34ccbd5b3.jpg""
                                                    }
                                                }
                                            ]
                                        }
                                    ]
                                }
                            ],
                            ""user"": {
                                ""reg_locale"": ""en-US"",
                                ""lang"": ""en"",
                                ""country"": ""US"",
                                ""mail"": ""john@doe.com""
                            },
                            ""global_info"": {
                                ""show_tags"": true
                            }
                        },
                        ""status"": ""ok"",
                        ""time_exec"": 0.02,
                        ""time_server"": 1585739239
                    }");

            using var netatmoClient = new NetatmoWebClient(mockHttp.AddAuthMock().ToMockProvider());

            var result = await netatmoClient.GetHomeData();

            Assert.Single(result.Homes);
            Assert.Equal("1234abcd1234abcd1234abcd", result.Homes[0].Id);
            Assert.Equal("My Netatmo Home", result.Homes[0].Name);
            Assert.Equal("Unittest Town", result.Homes[0].Place.City);
            Assert.Equal("EN", result.Homes[0].Place.Country);
            Assert.Equal("America/New_York", result.Homes[0].Place.Timezone);

            Assert.Single(result.Homes[0].Cameras);
            Assert.Equal("78:90:ab:cd:ef:01", result.Homes[0].Cameras[0].Id);
            Assert.Equal("NOC", result.Homes[0].Cameras[0].Type);
            Assert.Equal("on", result.Homes[0].Cameras[0].Status);
            Assert.Equal("https://prodvpn-eu-123.netatmo.net/restricted/10.0.0.1/abcabc123412341234123412341234ab/MTABCDEFG1234ABCDEF1234BACDEF1234ABCDEF123,,", result.Homes[0].Cameras[0].VpnUrl);
            Assert.False(result.Homes[0].Cameras[0].IsLocal);
            Assert.Equal("on", result.Homes[0].Cameras[0].SdStatus);
            Assert.Equal("on", result.Homes[0].Cameras[0].AlimStatus);
            Assert.Equal("My Presence", result.Homes[0].Cameras[0].Name);
            Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1490468965), result.Homes[0].Cameras[0].LastSetup);
            Assert.Equal("auto", result.Homes[0].Cameras[0].LightModeStatus);

            Assert.Single(result.Homes[0].Events);
            Assert.Equal("1234abcd1234abcd1234abcd", result.Homes[0].Events[0].Id);
            Assert.Equal("outdoor", result.Homes[0].Events[0].Type);
            Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1585576467), result.Homes[0].Events[0].Time);
            Assert.Equal("78:90:ab:cd:ef:01", result.Homes[0].Events[0].CameraId);
            Assert.Equal("78:90:ab:cd:ef:01", result.Homes[0].Events[0].DeviceId);
            Assert.Equal("1234d30c-1234-5678-9012-abc35d7a4abc", result.Homes[0].Events[0].VideoId);
            Assert.Equal("available", result.Homes[0].Events[0].VideoStatus);
            Assert.NotEmpty(result.Homes[0].Events[0].EventList);

            Assert.Equal("vehicle", result.Homes[0].Events[0].EventList[0].Type);
            Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1585726685), result.Homes[0].Events[0].EventList[0].Time);
            Assert.Equal(0, result.Homes[0].Events[0].EventList[0].Offset);
            Assert.Equal("1234d30c-1234-5678-9012-ff735af0923f", result.Homes[0].Events[0].EventList[0].Id);
            Assert.Equal("Vehicle detected", result.Homes[0].Events[0].EventList[0].Message);
            Assert.Equal("123e75a5980d8c6c79ef111a", result.Homes[0].Events[0].EventList[0].Snapshot.Id);
            Assert.Equal(1, result.Homes[0].Events[0].EventList[0].Snapshot.Version);
            Assert.Equal("123e75a5980d8c6c79ef111a977f2280e9823d3e5ba00adaa94edbe5ba00e5ba", result.Homes[0].Events[0].EventList[0].Snapshot.Key);
            Assert.Equal("https://netatmocameraimage.blob.domain.com/path/b178de75e8444df3c9d762bd452bc2d625a44e75a5980d8c6c79ef111a977f2280e9823d3e5ba00adaa94edb", result.Homes[0].Events[0].EventList[0].Snapshot.Url);
            Assert.Equal(string.Empty, result.Homes[0].Events[0].EventList[0].Snapshot.Filename);
            Assert.Equal("234e75a5980d8c6c79ef111a", result.Homes[0].Events[0].EventList[0].Vignette.Id);
            Assert.Equal(1, result.Homes[0].Events[0].EventList[0].Vignette.Version);
            Assert.Equal("234e75a5980d8c6c79ef111a977f2280e9823d3e5ba00adaa94edbe5ba00e5ba", result.Homes[0].Events[0].EventList[0].Vignette.Key);
            Assert.Equal("https://netatmocameraimage.blob.domain.com/path/b178de75e8444df3c9d762bd452bc2d625a44e75a5980d8c6c79ef111a977f2280e9823d3e5ba00adaa94edb", result.Homes[0].Events[0].EventList[0].Vignette.Url);
            Assert.Equal(string.Empty, result.Homes[0].Events[0].EventList[0].Vignette.Filename);

            Assert.Equal("human", result.Homes[0].Events[0].EventList[1].Type);
            Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1585576470), result.Homes[0].Events[0].EventList[1].Time);
            Assert.Equal(1, result.Homes[0].Events[0].EventList[1].Offset);
            Assert.Equal("1234d30c-1234-5678-9012-e5b34ccbd5b3", result.Homes[0].Events[0].EventList[1].Id);
            Assert.Equal("Person detected", result.Homes[0].Events[0].EventList[1].Message);
            Assert.Equal("vod/1234d30c-1234-5678-9012-abc35d7a4abc/events/1234d30c-1234-5678-9012-e5b34ccbd5b3/snapshot_1234d30c-1234-5678-9012-e5b34ccbd5b3.jpg", result.Homes[0].Events[0].EventList[1].Snapshot.Filename);
            Assert.Equal("vod/1234d30c-1234-5678-9012-abc35d7a4abc/events/1234d30c-1234-5678-9012-e5b34ccbd5b3/vignette_1234d30c-1234-5678-9012-e5b34ccbd5b3.jpg", result.Homes[0].Events[0].EventList[1].Vignette.Filename);

            Assert.Equal("en-US", result.User.RegLocale);
            Assert.Equal("en", result.User.Lang);
            Assert.Equal("US", result.User.Country);
            Assert.Equal("john@doe.com", result.User.Mail);
            Assert.True(result.GlobalInfo.ShowTags);
        }
    }
}
