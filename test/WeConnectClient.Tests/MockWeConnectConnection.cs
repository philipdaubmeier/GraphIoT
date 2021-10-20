using PhilipDaubmeier.WeConnectClient.Network;
using RichardSzalay.MockHttp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace PhilipDaubmeier.WeConnectClient.Tests
{
    public static class MockWeConnectConnection
    {
        private const string _dummyJwtToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJzdWJpZCIsImF1ZCI6ImF1ZCIsInNjcCI6InNjb3BlcyIsImFhdCI6ImlkZW50aXR5a2l0In0.gVNJi3N8lbJe23s5hcbe4LCpaw0C1-sTpDVUoOPGtpA";

        private const string _requestedScopesVw = "profile,address,phone,dealers,carConfigurations,cars,vin,profession";
        private const string _requestedScopesWeConnect = "openid,mbb";
        private const string _scopesVw = "profession%20cars%20address%20phone%20openid%20profile%20dealers%20vin%20carConfigurations";
        private const string _scopesWeConnect = "openid%20mbb";
        private const string _authFagsPartVw = "\"vw-de\":[\"profession\",\"cars\",\"address\",\"phone\",\"openid\",\"profile\",\"dealers\",\"vin\",\"carConfigurations\"]";
        private const string _authFagsPartWeConnect = "\"vwag-weconnect\":[\"openid\",\"mbb\"]";
        private static readonly string _authFagsVW = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{{{_authFagsPartVw}}}"));
        private static readonly string _authFagsVWAndWeConnect = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{{{_authFagsPartVw},{_authFagsPartWeConnect}}}"));

        private const string _csrfToken1 = "unittest--111-csrf-aa8a-d37eeb633f6c";
        private const string _csrfToken2 = "unittest--222-csrf-bb91-f0bd1921b97e";
        private const string _csrfToken3 = "unittest--333-csrf-ccd2-bf5779e10440";
        private const string _csrfToken4 = "unittest--444-csrf-ddd7-e899108ec304";
        private const string _session1 = "random-unittest-sessionid-1-AWItZjZmNDdiYzA4ZjVm";
        private const string _session2 = "random-unittest-sessionid-2-BTAtNDgyNGVkZDhkMzdh";
        private const string _session3 = "random-unittest-sessionid-3-CjMtNzQxYWRlNGExZjBm";
        private const string _session4 = "random-unittest-sessionid-4-DWQtMGUxNWZjNmY5ZGYw";
        private const string _jsessionId = "random-unittest-jsessionid-735A3";
        private const string _salt = "random-unittest-salt-value-wAfj";

        private const string _state1 = "unittest-state-random-11gLKotJiB0_Sj6zTKH2o%3D";
        private const string _state2 = "unittest-state-random-22Cz2fWje9FH_Iq5XYxOU%3D";
        private const string _relayState1 = "random-unittest-relayState-111187a5d5b80";
        private const string _relayState2 = "random-unittest-relayState-2222f02229d91";
        private const string _nonce1 = "unittest-nonce-random-118AN_GcgvJSGdYI9lKvk";
        private const string _nonce2 = "unittest-nonce-random-22q4XWxSmOKFVKYIcWMHQ";

        private const string _hmac1 = "unittest-hmac-111111-random-4a7b55321-hmac-d067580f287c98fe0e1b6";
        private const string _hmac2 = "unittest-hmac-222222-random-33d009c16-hmac-ce776c8b6334acc18f3c2";
        private const string _hmac3 = "unittest-hmac-333333-random-a96bc8d15-hmac-a3b66e396c1104a153d34";
        private const string _hmac4 = "unittest-hmac-444444-random-614a42be7-hmac-36a07b9a02398a965f3d7";
        private const string _hmac5 = "unittest-hmac-555555-random-8b253bd9c-hmac-69f038e5dff4e499fe617";
        private const string _hmac6 = "unittest-hmac-666666-random-8a07d6c57-hmac-8e7117a16755aa8f48c75";

        private const string _clientId1 = "11111111-d113-4abc-9abc-ffabcdef1234@apps_vw-dilab_com";
        private const string _clientId2 = "22222222-d223-4abc-9abc-ffabcdef1234@apps_vw-dilab_com";

        private const string _userId = "123451c9-d123-4abc-9cde-89abcdef1234";

        public static string Vin => "WVWZZZABCD1234567";
        public static string VwBaseUri => "https://www.volkswagen.de/";
        public static string IdkBaseUri => "https://identity.vwgroup.io/";
        public static string GvfBaseUri => "https://myvw-gvf-proxy.apps.emea.vwapps.io";
        public static string VdbsBaseUri => "https://vdbs.apps.emea.vwapps.io/v1/vehicles";
        public static string VcfBaseUriPattern => "https://cardata.apps.emea.vwapps.io/vehicles/{Vin}";
        public static string VcfBaseUri => $"https://cardata.apps.emea.vwapps.io/vehicles/{Vin}";
        public static string UserId => _userId;
        public static string AccessToken => _dummyJwtToken;

        public class WeConnectMockConnectionProvider : WeConnectConnectionProvider
        {
            public WeConnectMockConnectionProvider(IWeConnectAuth authData, HttpClient mockClient, HttpClient mockAuthClient)
                : base(authData)
            {
                Client = mockClient;
                AuthClient = mockAuthClient;
            }
        }

        private static readonly IWeConnectAuth mockAuth = new WeConnectAuth("john@doe.com", "secretpassword");

        public static WeConnectConnectionProvider ToMockProvider(this MockHttpMessageHandler mockHandler, MockHttpMessageHandler? mockAuthHandler = null, IWeConnectAuth? auth = null)
        {
            var connProvider = new WeConnectMockConnectionProvider(auth ?? mockAuth, new HttpClient(mockHandler), new HttpClient(mockAuthHandler is null ? mockHandler : mockAuthHandler));

            // Hook up cookie container, so that the mock handler fills the cookie container of the connection provider
            if (mockHandler is MockCookieHttpMessageHandler cookieHandler)
                cookieHandler.CookieContainer = connProvider.CookieContainer;
            if (mockAuthHandler is MockCookieHttpMessageHandler cookieAuthHandler)
                cookieAuthHandler.CookieContainer = connProvider.CookieContainer;

            return connProvider;
        }

        public static MockHttpMessageHandler AddAuthMock(this MockHttpMessageHandler mockHttp)
        {
            return mockHttp.AddAuthMock(out MockedRequest _);
        }

        public static MockHttpMessageHandler AddAuthMock(this MockHttpMessageHandler mockHttp, out MockedRequest mocketRequest)
        {
            mocketRequest = mockHttp.When($"{VwBaseUri}app/authproxy/login")
                    .WithExactQueryString($"fag=vw-de,vwag-weconnect&scope-vw-de={_requestedScopesVw}&scope-vwag-weconnect={_requestedScopesWeConnect}&prompt-vw-de=login&prompt-vwag-weconnect=none&redirectUrl={VwBaseUri}")
                    .Respond(HttpStatusCode.Found, new List<KeyValuePair<string, string>>()
                    {
                        new("Location", $"{VwBaseUri}app/authproxy/login/vw-de?scope={_requestedScopesVw}&prompt=login"),
                        new("Set-Cookie", $"csrf_token={_csrfToken1}; Max-Age=604800; Expires=Thu, 01-Jan-2100 00:00:00 GMT; Path=/; Secure"),
                        new("Set-Cookie", $"SESSION={_session1}; Max-Age=604800; Expires=Thu, 01-Jan-2100 00:00:00 GMT; Path=/app/authproxy/; Secure; HttpOnly; SameSite=Lax")
                    }, "text/plain", "");

            mockHttp.When($"{VwBaseUri}app/authproxy/login/vw-de")
                    .WithExactQueryString($"scope={_requestedScopesVw}&prompt=login")
                    .WithCookies(new List<KeyValuePair<string, string>>()
                    {
                        new("SESSION", _session1),
                        new("csrf_token", _csrfToken1)
                    })
                    .Respond(HttpStatusCode.Found, new List<KeyValuePair<string, string>>()
                    {
                        new("Location", $"{IdkBaseUri}oidc/v1/authorize?response_type=code&client_id={_clientId1}&scope={_scopesVw}&state={_state1}&redirect_uri={VwBaseUri}app/authproxy/login/oauth2/code/vw-de&nonce={_nonce1}&prompt=login")
                    }, "text/plain", "");

            mockHttp.When($"{IdkBaseUri}oidc/v1/authorize")
                    .WithExactQueryString($"response_type=code&client_id={_clientId1}&scope={_scopesVw}&state={_state1}&redirect_uri={VwBaseUri}app/authproxy/login/oauth2/code/vw-de&nonce={_nonce1}&prompt=login")
                    .Respond(HttpStatusCode.Found, new List<KeyValuePair<string, string>>()
                    {
                        new("Location", $"{IdkBaseUri}signin-service/v1/signin/{_clientId1}?relayState={_relayState1}"),
                        new("Set-Cookie", $"JSESSIONID={_jsessionId}; Path=/oidc; Secure; HttpOnly")
                    }, "text/plain", "");

            mockHttp.When($"{IdkBaseUri}signin-service/v1/signin/{_clientId1}?relayState={_relayState1}")
                    .Respond(new List<KeyValuePair<string, string>>() {
                        new("Set-Cookie", $"SESSION={_session2}; Path=/signin-service/v1/; Secure; HttpOnly; SameSite=Lax")
                    }, "text/html",
                    @"<!DOCTYPE html>
                    <html>
                        <head />
                        <body>
                            <form method=""POST"" action=""/signin-service/v1/" + _clientId1 + @"/login/identifier"">
                                <input type=""hidden"" id=""csrf"" name=""_csrf"" value=""" + _csrfToken2 + @""" />
                                <input type=""hidden"" id=""input_relayState"" name=""relayState"" value=""" + _relayState1 + @""" />
                                <input type=""hidden"" id=""hmac"" name=""hmac"" value=""" + _hmac1 + @""" />
                                <input id=""input_email"" name=""email"" type=""email"" />
                                <button type=""submit"">Next</button>
                            </form>
                        </body>
                    </html>");

            mockHttp.When(HttpMethod.Post, $"{IdkBaseUri}signin-service/v1/{_clientId1}/login/identifier")
                    .WithExactFormData(new List<KeyValuePair<string, string>>()
                    {
                        new("email", mockAuth.Username),
                        new("relayState", _relayState1),
                        new("hmac", _hmac1),
                        new("_csrf", _csrfToken2)
                    })
                    .WithCookies(new List<KeyValuePair<string, string>>()
                    {
                        new("SESSION", _session2)
                    })
                    .Respond(HttpStatusCode.SeeOther, new List<KeyValuePair<string, string>>()
                    {
                        new("Location", $"/signin-service/v1/{_clientId1}/login/authenticate?relayState={_relayState1}&email={mockAuth.Username}")
                    }, "text/plain", "");

            mockHttp.When($"{IdkBaseUri}signin-service/v1/{_clientId1}/login/authenticate?relayState={_relayState1}&email={mockAuth.Username}")
                    .WithCookies(new List<KeyValuePair<string, string>>()
                    {
                        new("SESSION", _session2)
                    })
                    .Respond("text/html",
                    @"<!DOCTYPE html>
                    <html>
                        <head />
                        <body>
                            <form method=""POST"" action=""/signin-service/v1/" + _clientId1 + @"/login/authenticate"">
                                <input type=""hidden"" id=""csrf"" name=""_csrf"" value=""" + _csrfToken2 + @"""/>
                                <input type=""hidden"" id=""input_relayState"" name=""relayState"" value=""" + _relayState1 + @"""/>
                                <input type=""hidden"" id=""email"" name=""email"" value=""" + mockAuth.Username + @"""/>
                                <input type=""hidden"" id=""hmac"" name=""hmac"" value=""" + _hmac2 + @"""/>
                                <input id=""password"" name=""password"" type=""password"" />
                                <button type=""submit"">Next</button>
                            </form>
						</body>
                    </html>");

            mockHttp.When(HttpMethod.Post, $"{IdkBaseUri}signin-service/v1/{_clientId1}/login/authenticate")
                    .WithExactFormData(new List<KeyValuePair<string, string>>()
                    {
                        new("email", mockAuth.Username),
                        new("password", mockAuth.UserPassword),
                        new("relayState", _relayState1),
                        new("hmac", _hmac2),
                        new("_csrf", _csrfToken2),
                        new("login", "true")
                    })
                    .WithCookies(new List<KeyValuePair<string, string>>()
                    {
                        new("SESSION", _session2)
                    })
                    .Respond(HttpStatusCode.Found, new List<KeyValuePair<string, string>>()
                    {
                        new("Location", $"{IdkBaseUri}oidc/v1/oauth/sso?clientId={_clientId1}&relayState={_relayState1}&userId={_userId}&HMAC={_hmac3}")
                    }, "text/plain", "");

            mockHttp.When($"{IdkBaseUri}oidc/v1/oauth/sso")
                    .WithExactQueryString($"clientId={_clientId1}&relayState={_relayState1}&userId={_userId}&HMAC={_hmac3}")
                    .WithCookies(new List<KeyValuePair<string, string>>()
                    {
                        new("JSESSIONID", _jsessionId)
                    })
                    .Respond(HttpStatusCode.Found, new List<KeyValuePair<string, string>>()
                    {
                        new("Location", $"{IdkBaseUri}signin-service/v1/consent/users/{_userId}/{_clientId1}?scopes={_scopesVw}&relayState={_relayState1}&callback={IdkBaseUri}oidc/v1/oauth/client/callback&hmac={_hmac4}")
                    }, "text/plain", "");

            mockHttp.When($"{IdkBaseUri}signin-service/v1/consent/users/{_userId}/{_clientId1}")
                    .WithExactQueryString($"scopes={_scopesVw}&relayState={_relayState1}&callback={IdkBaseUri}oidc/v1/oauth/client/callback&hmac={_hmac4}")
                    .WithCookies(new List<KeyValuePair<string, string>>()
                    {
                        new("SESSION", _session2)
                    })
                    .Respond(HttpStatusCode.Found, new List<KeyValuePair<string, string>>()
                    {
                        new("Location", $"{IdkBaseUri}oidc/v1/oauth/client/callback/success?user_id={_userId}&client_id={_clientId1}&scopes={_scopesVw}&consentedScopes={_scopesVw}&relayState={_relayState1}&hmac={_hmac5}")
                    }, "text/plain", "");

            mockHttp.When($"{IdkBaseUri}oidc/v1/oauth/client/callback/success")
                    .WithExactQueryString($"user_id={_userId}&client_id={_clientId1}&scopes={_scopesVw}&consentedScopes={_scopesVw}&relayState={_relayState1}&hmac={_hmac5}")
                    .WithCookies(new List<KeyValuePair<string, string>>()
                    {
                        new("JSESSIONID", _jsessionId)
                    })
                    .Respond(HttpStatusCode.Found, new List<KeyValuePair<string, string>>()
                    {
                        new("Location", $"{VwBaseUri}app/authproxy/login/oauth2/code/vw-de?state={_state1}&code={_dummyJwtToken}")
                    }, "text/plain", "");

            mockHttp.When($"{VwBaseUri}app/authproxy/login/oauth2/code/vw-de")
                    .WithExactQueryString($"state={_state1}&code={_dummyJwtToken}")
                    .WithCookies(new List<KeyValuePair<string, string>>()
                    {
                        new("SESSION", _session1),
                        new("csrf_token", _csrfToken1)
                    })
                    .Respond(HttpStatusCode.Found, new List<KeyValuePair<string, string>>()
                    {
                        new("Location", $"{VwBaseUri}app/authproxy/login/vwag-weconnect?scope={_requestedScopesWeConnect}&prompt=none"),
                        new("Set-Cookie", $"salt={_salt}; Max-Age=604800; Expires=Thu, 01-Jan-2100 00:00:00 GMT; Path=/app/authproxy/; Secure; HttpOnly"),
                        new("Set-Cookie", $"auth_fags={_authFagsVW}; Max-Age=604800; Expires=Thu, 01-Jan-2100 00:00:00 GMT; Path=/; Secure"),
                        new("Set-Cookie", $"csrf_token=; Max-Age=0; Expires=Thu, 01-Jan-2100 00:00:00 GMT; Path=/; Secure"),
                        new("Set-Cookie", $"csrf_token={_csrfToken3}; Max-Age=604800; Expires=Thu, 01-Jan-2100 00:00:00 GMT; Path=/; Secure"),
                        new("Set-Cookie", $"SESSION={_session3}; Max-Age=604800; Expires=Thu, 01-Jan-2100 00:00:00 GMT; Path=/app/authproxy/; Secure; HttpOnly; SameSite=Lax")
                    }, "text/plain", "");

            mockHttp.When($"{VwBaseUri}app/authproxy/login/vwag-weconnect")
                    .WithExactQueryString($"scope={_requestedScopesWeConnect}&prompt=none")
                    .WithCookies(new List<KeyValuePair<string, string>>()
                    {
                        new("SESSION", _session3),
                        new("salt", _salt),
                        new("auth_fags", _authFagsVW),
                        new("csrf_token", _csrfToken3)
                    })
                    .Respond(HttpStatusCode.Found, new List<KeyValuePair<string, string>>()
                    {
                        new("Location", $"{IdkBaseUri}oidc/v1/authorize?response_type=code&client_id={_clientId2}&scope={_scopesWeConnect}&state={_state2}&redirect_uri={VwBaseUri}app/authproxy/login/oauth2/code/vwag-weconnect&nonce={_nonce2}&prompt=none")
                    }, "text/plain", "");

            mockHttp.When($"{IdkBaseUri}oidc/v1/authorize")
                    .WithExactQueryString($"response_type=code&client_id={_clientId2}&scope={_scopesWeConnect}&state={_state2}&redirect_uri={VwBaseUri}app/authproxy/login/oauth2/code/vwag-weconnect&nonce={_nonce2}&prompt=none")
                    .WithCookies(new List<KeyValuePair<string, string>>()
                    {
                        new("JSESSIONID", _jsessionId)
                    })
                    .Respond(HttpStatusCode.Found, new List<KeyValuePair<string, string>>()
                    {
                        new("Location", $"{IdkBaseUri}oidc/v1/oauth/sso?clientId={_clientId2}&userId={_userId}&relayState={_relayState2}&prompt=none&HMAC={_hmac6}")
                    }, "text/plain", "");

            mockHttp.When($"{IdkBaseUri}oidc/v1/oauth/sso")
                    .WithExactQueryString($"clientId={_clientId2}&userId={_userId}&relayState={_relayState2}&prompt=none&HMAC={_hmac6}")
                    .WithCookies(new List<KeyValuePair<string, string>>()
                    {
                        new("JSESSIONID", _jsessionId)
                    })
                    .Respond(HttpStatusCode.Found, new List<KeyValuePair<string, string>>()
                    {
                        new("Location", $"{IdkBaseUri}oidc/v1/oauth/client/callback?clientId={_clientId2}&relayState={_relayState2}&userId={_userId}&HMAC={_hmac6}")
                    }, "text/plain", "");

            mockHttp.When($"{IdkBaseUri}oidc/v1/oauth/client/callback")
                    .WithExactQueryString($"clientId={_clientId2}&relayState={_relayState2}&userId={_userId}&HMAC={_hmac6}")
                    .WithCookies(new List<KeyValuePair<string, string>>()
                    {
                        new("JSESSIONID", _jsessionId)
                    })
                    .Respond(HttpStatusCode.Found, new List<KeyValuePair<string, string>>()
                    {
                        new("Location", $"{VwBaseUri}app/authproxy/login/oauth2/code/vwag-weconnect?state={_state2}&code={_dummyJwtToken}")
                    }, "text/plain", "");

            mockHttp.When($"{VwBaseUri}app/authproxy/login/oauth2/code/vwag-weconnect")
                    .WithExactQueryString($"state={_state2}&code={_dummyJwtToken}")
                    .WithCookies(new List<KeyValuePair<string, string>>()
                    {
                        new("SESSION", _session3),
                        new("salt", _salt),
                        new("auth_fags", _authFagsVW),
                        new("csrf_token", _csrfToken3)
                    })
                    .Respond(HttpStatusCode.Found, new List<KeyValuePair<string, string>>()
                    {
                        new("Location", $"{VwBaseUri}"),
                        new("Set-Cookie", $"salt={_salt}; Max-Age=604800; Expires=Thu, 01-Jan-2100 00:00:00 GMT; Path=/app/authproxy/; Secure; HttpOnly"),
                        new("Set-Cookie", $"auth_fags={_authFagsVWAndWeConnect}; Max-Age=604800; Expires=Thu, 01-Jan-2100 00:00:00 GMT; Path=/; Secure"),
                        new("Set-Cookie", $"csrf_token=; Max-Age=0; Expires=Thu, 01-Jan-2100 00:00:00 GMT; Path=/; Secure"),
                        new("Set-Cookie", $"csrf_token={_csrfToken4}; Max-Age=604800; Expires=Thu, 01-Jan-2100 00:00:00 GMT; Path=/; Secure"),
                        new("Set-Cookie", $"SESSION={_session4}; Max-Age=604800; Expires=Thu, 01-Jan-2100 00:00:00 GMT; Path=/app/authproxy/; Secure; HttpOnly; SameSite=Lax")
                    }, "text/plain", "");

            mockHttp.When($"{VwBaseUri}")
                    .WithCookies(new List<KeyValuePair<string, string>>()
                    {
                        new("auth_fags", _authFagsVWAndWeConnect),
                        new("csrf_token", _csrfToken4)
                    })
                    .Respond(HttpStatusCode.MovedPermanently, new List<KeyValuePair<string, string>>()
                    {
                        new("Location", $"{VwBaseUri}de.html")
                    }, "text/plain", "");

            mockHttp.When($"{VwBaseUri}de.html")
                    .WithCookies(new List<KeyValuePair<string, string>>()
                    {
                        new("auth_fags", _authFagsVWAndWeConnect),
                        new("csrf_token", _csrfToken4)
                    })
                    .Respond("text/html",
                    @"<!DOCTYPE html>
                    <html><head /><body /></html>");

            mockHttp.When($"{VwBaseUri}app/authproxy/vw-de/tokens")
                    .WithCookiesAndHeaders(new List<KeyValuePair<string, string>>()
                    {
                        new("X-CSRF-TOKEN", _csrfToken1)
                    },
                    new List<KeyValuePair<string, string>>()
                    {
                        new("SESSION", _session1),
                        new("salt", _salt),
                        new("auth_fags", _authFagsVWAndWeConnect),
                        new("csrf_token", _csrfToken1)
                    })
                    .Respond(new List<KeyValuePair<string, string>>() {
                        new("Set-Cookie", $"salt={_salt}; Max-Age=604800; Expires=Thu, 01-Jan-2100 00:00:00 GMT; Path=/app/authproxy/; Secure; HttpOnly"),
                        new("Set-Cookie", $"auth_fags={_authFagsVWAndWeConnect}; Max-Age=604800; Expires=Thu, 01-Jan-2100 00:00:00 GMT; Path=/; Secure")
                    }, "text/html",
                    @"{
                        ""access_token"": """ + _dummyJwtToken + @""",
                        ""id_token"": """ + _dummyJwtToken + @"""
                    }");

            mockHttp.When($"{VwBaseUri}app/authproxy/vwag-weconnect/tokens")
                    .WithCookiesAndHeaders(new List<KeyValuePair<string, string>>()
                    {
                        new("X-CSRF-TOKEN", _csrfToken1)
                    },
                    new List<KeyValuePair<string, string>>()
                    {
                        new("SESSION", _session1),
                        new("salt", _salt),
                        new("auth_fags", _authFagsVWAndWeConnect),
                        new("csrf_token", _csrfToken1)
                    })
                    .Respond(new List<KeyValuePair<string, string>>() {
                        new("Set-Cookie", $"salt={_salt}; Max-Age=604800; Expires=Thu, 01-Jan-2100 00:00:00 GMT; Path=/app/authproxy/; Secure; HttpOnly"),
                        new("Set-Cookie", $"auth_fags={_authFagsVWAndWeConnect}; Max-Age=604800; Expires=Thu, 01-Jan-2100 00:00:00 GMT; Path=/; Secure")
                    }, "text/html",
                    @"{
                        ""access_token"": """ + _dummyJwtToken + @""",
                        ""id_token"": """ + _dummyJwtToken + @"""
                    }");

            mockHttp.When($"{VwBaseUri}app/authproxy/vw-de/user")
                    .WithCookiesAndHeaders(new List<KeyValuePair<string, string>>()
                    {
                        new("X-CSRF-TOKEN", _csrfToken1)
                    },
                    new List<KeyValuePair<string, string>>()
                    {
                        new("SESSION", _session1),
                        new("salt", _salt),
                        new("auth_fags", _authFagsVWAndWeConnect),
                        new("csrf_token", _csrfToken1)
                    })
                    .Respond("application/json",
                    @"{
                        ""sub"": """ + _userId + @""",
                        ""name"": ""John Doe"",
                        ""given_name"": ""John"",
                        ""family_name"": ""Doe"",
                        ""email"": """ + mockAuth.Username + @""",
                        ""email_verified"": true,
                        ""phone_number"": ""55512345687"",
                        ""phone_number_verified"": true,
                        ""address"":
                        {
                            ""street_address"": ""Mockstreet 1"",
                            ""locality"": ""Mocktropolis"",
                            ""postal_code"": ""12345"",
                            ""country"": ""US"",
                            ""formatted"": ""Mockstreet 1\n12345 Mocktropolis\nUS""
                        },
                        ""updated_at"": 1234567890,
                        ""picture"": ""https://customer-pictures.apps.emea.vwapps.io/v1/" + _userId + @"/profile-picture""
                    }");

            mockHttp.When($"https://myvw-idk-token-exchanger.apps.emea.vwapps.io/token-exchange")
                    .WithExactQueryString($"isWcar=false")
                    .WithHeaders(new List<KeyValuePair<string, string>>()
                    {
                        new("Authorization", $"Bearer {_dummyJwtToken}")
                    })
                    .Respond("application/json",
                    $"{_dummyJwtToken}");

            return mockHttp;
        }
    }
}