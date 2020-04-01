using RichardSzalay.MockHttp;

namespace PhilipDaubmeier.NetatmoClient.Tests
{
    public static class MockNetatmoWeather
    {
        public static MockHttpMessageHandler AddWeatherStation(this MockHttpMessageHandler mockHttp)
        {
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

            return mockHttp;
        }
    }
}