using RichardSzalay.MockHttp;
using System.Threading.Tasks;
using Xunit;

namespace PhilipDaubmeier.SonnenClient.Tests
{
    public class SonnenPortalClientTest
    {
        [Fact]
        public async Task TestGetUserSites()
        {
            var mockHttp = new MockHttpMessageHandler();

            mockHttp.When($"{MockSonnenConnection.BaseUri}users/me")
                    .Respond("application/json",
                    @"{
                        ""data"":
                        {
                            ""id"": ""01da887a-4898-46e1-ab49-f45fc7dce4c2"",
                            ""type"": ""users"",
                            ""attributes"":
                            {
                                ""academic_title"": null,
                                ""customer_number"": ""US11122233"",
                                ""first_name"": ""John"",
                                ""last_name"": ""Doe"",
                                ""description"": null,
                                ""email"": ""john@doe.com"",
                                ""phone"": null,
                                ""mobile"": null,
                                ""street"": ""Long test street"",
                                ""postal_code"": ""12345"",
                                ""city"": ""Unittest Town"",
                                ""state"": """",
                                ""country_code"": ""US"",
                                ""latitude"": 37.377166,
                                ""longitude"": -122.086966,
                                ""language"": ""en"",
                                ""newsletter"": true,
                                ""time_zone"": ""America/New_York"",
                                ""privacy_policy"": ""EN-us-4.0"",
                                ""terms_of_service"": ""EN-us-1.0"",
                                ""service_partners_data_access"": true,
                                ""salesforce_user_id"": ""0012340000ABAABBAB"",
                                ""salesforce_contact_id"": ""00123400012ABABAAB"",
                                ""roles"": []
                            },
                            ""relationships"":
                            {
                                ""sites"":
                                {
                                    ""data"":
                                    [
                                        {
                                            ""type"": ""sites"",
                                            ""id"": """ + MockSonnenConnection.SiteId + @"""
                                        }
                                    ]
                                },
                                ""default-site"":
                                {
                                    ""data"":
                                    {
                                        ""type"": ""sites"",
                                        ""id"": """ + MockSonnenConnection.SiteId + @"""
                                    }
                                }
                            }
                        }
                    }");

            using var sonnenClient = new SonnenPortalClient(mockHttp.AddAuthMock().ToMockProvider());

            var result = await sonnenClient.GetUserSites();

            Assert.NotNull(result.DefaultSiteId);
        }
    }
}
