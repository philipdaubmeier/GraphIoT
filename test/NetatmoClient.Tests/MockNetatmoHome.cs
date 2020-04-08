using RichardSzalay.MockHttp;

namespace PhilipDaubmeier.NetatmoClient.Tests
{
    public static class MockNetatmoHome
    {
        public static MockHttpMessageHandler AddHomeData(this MockHttpMessageHandler mockHttp)
        {
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
                                        ""timezone"": ""America/New_York""
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

            return mockHttp;
        }
    }
}