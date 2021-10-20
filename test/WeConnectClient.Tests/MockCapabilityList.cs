using RichardSzalay.MockHttp;

namespace PhilipDaubmeier.WeConnectClient.Tests
{
    public static class MockCapabilityList
    {
        public static MockHttpMessageHandler AddCapabilityList(this MockHttpMessageHandler mockHttp)
        {
            mockHttp.When($"{MockWeConnectConnection.VdbsBaseUri}/{MockWeConnectConnection.Vin}/users/{MockWeConnectConnection.UserId}/capabilities")
                    .Respond("application/json",
                    @"{
                        ""vin"": """ + MockWeConnectConnection.Vin + @""",
                        ""userId"": """ + MockWeConnectConnection.UserId + @""",
                        ""role"": ""PRIMARY_USER"",
                        ""workshopMode"": ""false"",
                        ""devicePlatform"": ""MBB"",
                        ""capabilities"":
                        [
                            {
                                ""id"": ""fuelStatus"",
                                ""status"": [],
                                ""expirationDate"": ""2031-01-01T12:00:00Z"",
                                ""userDisablingAllowed"": false,
                                ""actions"":
                                [
                                    {
                                        ""id"": ""getFuelStatus"",
                                        ""domain"": ""vcf"",
                                        ""url"": """ + MockWeConnectConnection.VcfBaseUriPattern + @"/fuel/status"",
                                        ""method"": ""GET"",
                                        ""scopes"":
                                        [
                                            ""fuelLevels""
                                        ]
                                    }
                                ],
                                ""mbb"":
                                [
                                    {
                                        ""serviceInfo"":
                                        [
                                            {
                                                ""serviceId"": ""statusreport_v1"",
                                                ""serviceType"": ""mbb"",
                                                ""serviceStatus"":
                                                {
                                                    ""status"": ""Enabled""
                                                },
                                                ""licenseRequired"": true,
                                                ""cumulatedLicense"":
                                                {
                                                    ""status"": ""ACTIVATED"",
                                                    ""warn"": false,
                                                    ""activationBy"": ""automatic"",
                                                    ""activationDate"":
                                                    {
                                                        ""content"": ""2021-01-01T12:00:00Z""
                                                    },
                                                    ""expirationDate"":
                                                    {
                                                        ""content"": ""2031-01-01T12:00:00Z""
                                                    },
                                                    ""licenseProfiles"":
                                                    {
                                                        ""lastUsedProfile"": ""LPBundle00001234"",
                                                        ""licenseProfile"":
                                                        [
                                                            {
                                                                ""content"": ""LPBundle00001234"",
                                                                ""ltype"": ""regular"",
                                                                ""profileType"": ""implicit"",
                                                                ""salesPartNo"": ""NET.123.001.A"",
                                                                ""duration"": ""P10Y""
                                                            }
                                                        ]
                                                    }
                                                },
                                                ""primaryUserRequired"": true,
                                                ""termsAndConditionsRequired"": false,
                                                ""blockDisabling"": false,
                                                ""personalized"": false,
                                                ""configurable"": false,
                                                ""deviceType"": ""TELEMATICS"",
                                                ""serviceEol"": ""2121-01-01T12:00:00Z"",
                                                ""privacyGroups"": null,
                                                ""parameters"":
                                                {
                                                    ""parameter"":
                                                    [
                                                        {
                                                            ""content"": ""4"",
                                                            ""name"": ""doors""
                                                        },
                                                        {
                                                            ""content"": ""false"",
                                                            ""name"": ""oil_serviceinterval""
                                                        }
                                                    ]
                                                },
                                                ""rolesAndRightsRequired"": true,
                                                ""invocationUrl"": null,
                                                ""operation"":
                                                [
                                                    {
                                                        ""id"": ""G_SVDATA"",
                                                        ""version"": 1,
                                                        ""xsd"": null,
                                                        ""permission"": ""granted"",
                                                        ""requiredUserRole"": ""SECONDARY_USER"",
                                                        ""requiredSecurityLevel"": ""HG_1"",
                                                        ""invocationUrl"": null
                                                    }
                                                ]
                                            }
                                        ]
                                    }
                                ]
                            }
                        ],
                        ""parameters"": [],
                        ""articles"":
                        [
                            {
                                ""lpBundleNumber"": ""LPBundle00001234"",
                                ""articleId"": null,
                                ""salesPartNumber"": ""NET.123.001.A"",
                                ""activationDate"": ""2021-01-01T12:00:00Z"",
                                ""expirationDate"": ""2031-01-01T12:00:00Z"",
                                ""serviceIds"":
                                [
                                    ""statusreport_v1""
                                ],
                                ""productIds"": null,
                                ""name"": ""Unittest bundle name"",
                                ""category"": ""MOD"",
                                ""lifetimeLicense"": true,
                                ""available"": false
                            }
                        ]
                    }");

            return mockHttp;
        }
    }
}